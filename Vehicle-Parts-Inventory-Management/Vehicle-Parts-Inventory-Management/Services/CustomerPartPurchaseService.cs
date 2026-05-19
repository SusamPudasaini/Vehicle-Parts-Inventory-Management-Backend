using Microsoft.EntityFrameworkCore;
using System.Text;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.DTOs.Requests;
using Vehicle_Parts_Inventory_Management.DTOs.Responses;
using Vehicle_Parts_Inventory_Management.Entities;
using Vehicle_Parts_Inventory_Management.Interfaces;

namespace Vehicle_Parts_Inventory_Management.Services
{
    public class CustomerPartPurchaseService : ICustomerPartPurchaseService
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _emailService;
        private readonly ILogger<CustomerPartPurchaseService> _logger;

        public CustomerPartPurchaseService(
            AppDbContext db,
            IEmailService emailService,
            ILogger<CustomerPartPurchaseService> logger)
        {
            _db = db;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<List<CustomerPartCatalogResponse>> GetCatalogAsync()
        {
            return await _db.Parts
                .AsNoTracking()
                .Where(part => part.IsActive)
                .OrderBy(part => part.Name)
                .Select(part => new CustomerPartCatalogResponse
                {
                    Id = part.Id,
                    Name = part.Name,
                    PartNumber = part.PartNumber,
                    Category = part.Category,
                    Description = part.Description,
                    SellingPrice = part.SellingPrice,
                    StockQuantity = part.StockQuantity,
                    IsAvailable = part.StockQuantity > 0
                })
                .ToListAsync();
        }

        public async Task<CustomerPurchaseCheckoutResponse> SubmitOrderAsync(int customerId, CustomerPurchaseCheckoutRequest request)
        {
            var customer = await _db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == customerId)
                ?? throw new InvalidOperationException("Customer not found.");

            var mergedItems = request.Items
                .GroupBy(item => item.PartId)
                .Select(group => new CustomerPurchaseCheckoutItemRequest
                {
                    PartId = group.Key,
                    Quantity = group.Sum(item => item.Quantity)
                })
                .ToList();

            if (mergedItems.Count == 0)
                throw new InvalidOperationException("Add at least one part to continue.");

            var partIds = mergedItems.Select(item => item.PartId).ToList();
            var parts = await _db.Parts
                .Where(part => partIds.Contains(part.Id))
                .ToDictionaryAsync(part => part.Id);

            var order = new CustomerPartOrder
            {
                CustomerId = customerId,
                Status = CustomerPartOrderStatus.Pending,
                RequestedAtUtc = DateTime.UtcNow
            };

            foreach (var item in mergedItems)
            {
                if (!parts.TryGetValue(item.PartId, out var part))
                    throw new InvalidOperationException($"Part with ID {item.PartId} was not found.");

                if (!part.IsActive)
                    throw new InvalidOperationException($"Part '{part.Name}' is not available for purchase.");

                if (part.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Only {part.StockQuantity} unit(s) of '{part.Name}' are currently available.");

                var lineTotal = item.Quantity * part.SellingPrice;

                order.Items.Add(new CustomerPartOrderItem
                {
                    PartId = part.Id,
                    PartName = part.Name,
                    PartNumber = part.PartNumber,
                    Quantity = item.Quantity,
                    UnitPrice = part.SellingPrice,
                    LineTotal = lineTotal
                });
            }

            order.TotalAmount = order.Items.Sum(item => item.LineTotal);

            _db.CustomerPartOrders.Add(order);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Customer part order {OrderId} submitted by customer {CustomerId} for total {TotalAmount}.",
                order.Id,
                customerId,
                order.TotalAmount);

            return new CustomerPurchaseCheckoutResponse
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                RequestedAt = order.RequestedAtUtc,
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(MapCheckoutItem).ToList()
            };
        }

        public async Task<List<CustomerPartOrderResponse>> GetCustomerOrdersAsync(int customerId)
        {
            var orders = await _db.CustomerPartOrders
                .AsNoTracking()
                .Where(order => order.CustomerId == customerId)
                .Include(order => order.Customer)
                .Include(order => order.Items)
                .OrderByDescending(order => order.RequestedAtUtc)
                .ToListAsync();

            return orders.Select(MapOrderResponse).ToList();
        }

        public async Task<List<CustomerPartOrderResponse>> GetAllOrdersAsync()
        {
            var orders = await _db.CustomerPartOrders
                .AsNoTracking()
                .Include(order => order.Customer)
                .Include(order => order.Items)
                .OrderBy(order => order.Status)
                .ThenByDescending(order => order.RequestedAtUtc)
                .ToListAsync();

            return orders.Select(MapOrderResponse).ToList();
        }

        public async Task<CustomerPartOrderResponse> ApproveOrderAsync(int orderId, string? staffNotes)
        {
            var order = await _db.CustomerPartOrders
                .Include(entry => entry.Customer)
                .Include(entry => entry.Items)
                .FirstOrDefaultAsync(entry => entry.Id == orderId)
                ?? throw new InvalidOperationException("Order not found.");

            if (order.Status != CustomerPartOrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be approved.");

            var partIds = order.Items.Select(item => item.PartId).Distinct().ToList();
            var parts = await _db.Parts
                .Where(part => partIds.Contains(part.Id))
                .ToDictionaryAsync(part => part.Id);

            foreach (var item in order.Items)
            {
                if (!parts.TryGetValue(item.PartId, out var part))
                    throw new InvalidOperationException($"Part '{item.PartName}' is no longer available.");

                if (!part.IsActive)
                    throw new InvalidOperationException($"Part '{part.Name}' is inactive and cannot be approved.");

                if (part.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for '{part.Name}'. Available: {part.StockQuantity}.");
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                order.Status = CustomerPartOrderStatus.Approved;
                order.ProcessedAtUtc = DateTime.UtcNow;
                order.StaffNotes = string.IsNullOrWhiteSpace(staffNotes) ? null : staffNotes.Trim();
                order.InvoiceNumber = $"CINV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

                foreach (var item in order.Items)
                {
                    var part = parts[item.PartId];
                    part.StockQuantity -= item.Quantity;
                    part.UpdatedAt = DateTime.UtcNow;

                    _db.PurchaseHistories.Add(new PurchaseHistory
                    {
                        CustomerId = order.CustomerId,
                        PartName = item.PartName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.LineTotal,
                        InvoiceNumber = order.InvoiceNumber,
                        PurchasedAt = order.ProcessedAtUtc.Value
                    });
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            var emailBody = BuildInvoiceEmailBody(order.Customer.FullName, order);
            await _emailService.SendAsync(order.Customer.Email, $"Your parts invoice {order.InvoiceNumber}", emailBody);

            _logger.LogInformation(
                "Customer part order {OrderId} approved and emailed to {CustomerEmail}.",
                order.Id,
                order.Customer.Email);

            return MapOrderResponse(order);
        }

        public async Task<CustomerPartOrderResponse> RejectOrderAsync(int orderId, string? staffNotes)
        {
            var order = await _db.CustomerPartOrders
                .Include(entry => entry.Customer)
                .Include(entry => entry.Items)
                .FirstOrDefaultAsync(entry => entry.Id == orderId)
                ?? throw new InvalidOperationException("Order not found.");

            if (order.Status != CustomerPartOrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be rejected.");

            order.Status = CustomerPartOrderStatus.Rejected;
            order.ProcessedAtUtc = DateTime.UtcNow;
            order.StaffNotes = string.IsNullOrWhiteSpace(staffNotes) ? null : staffNotes.Trim();

            await _db.SaveChangesAsync();

            _logger.LogInformation("Customer part order {OrderId} rejected.", order.Id);

            return MapOrderResponse(order);
        }

        private static CustomerPurchaseCheckoutItemResponse MapCheckoutItem(CustomerPartOrderItem item) => new()
        {
            PartId = item.PartId,
            PartName = item.PartName,
            PartNumber = item.PartNumber,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            LineTotal = item.LineTotal
        };

        private static CustomerPartOrderResponse MapOrderResponse(CustomerPartOrder order) => new()
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.FullName ?? string.Empty,
            CustomerEmail = order.Customer?.Email ?? string.Empty,
            Status = order.Status.ToString(),
            InvoiceNumber = order.InvoiceNumber,
            StaffNotes = order.StaffNotes,
            TotalAmount = order.TotalAmount,
            RequestedAtUtc = order.RequestedAtUtc,
            ProcessedAtUtc = order.ProcessedAtUtc,
            Items = order.Items.Select(item => new CustomerPartOrderItemResponse
            {
                Id = item.Id,
                PartId = item.PartId,
                PartName = item.PartName,
                PartNumber = item.PartNumber,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.LineTotal
            }).ToList()
        };

        private static string BuildInvoiceEmailBody(string customerName, CustomerPartOrder order)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Hello {customerName},");
            builder.AppendLine();
            builder.AppendLine("Your parts order has been confirmed by our staff.");
            builder.AppendLine($"Invoice Number: {order.InvoiceNumber}");
            builder.AppendLine($"Confirmed At: {order.ProcessedAtUtc:yyyy-MM-dd HH:mm} UTC");
            builder.AppendLine();
            builder.AppendLine("Items:");

            foreach (var item in order.Items)
            {
                builder.AppendLine(
                    $"- {item.PartName} ({item.PartNumber}) x {item.Quantity} @ Rs. {item.UnitPrice:0.00} = Rs. {item.LineTotal:0.00}");
            }

            builder.AppendLine();
            builder.AppendLine($"Total Amount: Rs. {order.TotalAmount:0.00}");

            if (!string.IsNullOrWhiteSpace(order.StaffNotes))
            {
                builder.AppendLine();
                builder.AppendLine($"Staff Note: {order.StaffNotes}");
            }

            builder.AppendLine();
            builder.AppendLine("Thank you for your purchase.");

            return builder.ToString();
        }
    }
}
