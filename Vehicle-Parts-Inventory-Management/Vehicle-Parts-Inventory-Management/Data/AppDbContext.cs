
using Microsoft.EntityFrameworkCore;
using Vehicle_Parts_Inventory_Management.Entities;

namespace Vehicle_Parts_Inventory_Management.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Staff> Staff => Set<Staff>();
        public DbSet<Vendor> Vendors => Set<Vendor>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<PartRequestEntity> PartRequests => Set<PartRequestEntity>();
        public DbSet<ServiceReview> ServiceReviews => Set<ServiceReview>();
        public DbSet<PurchaseHistory> PurchaseHistories => Set<PurchaseHistory>();
        public DbSet<ServiceHistory> ServiceHistories => Set<ServiceHistory>();
        public DbSet<CustomerPartOrder> CustomerPartOrders => Set<CustomerPartOrder>();
        public DbSet<CustomerPartOrderItem> CustomerPartOrderItems => Set<CustomerPartOrderItem>();
        public DbSet<Part> Parts => Set<Part>();
        public DbSet<PurchaseInvoice> PurchaseInvoices => Set<PurchaseInvoice>();
        public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems => Set<PurchaseInvoiceItem>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=VehiclePartsDBNew;Username=postgres;Password=root");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Staff>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Customer)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerPartOrder>()
                .HasOne(order => order.Customer)
                .WithMany()
                .HasForeignKey(order => order.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerPartOrderItem>()
                .HasOne(item => item.CustomerPartOrder)
                .WithMany(order => order.Items)
                .HasForeignKey(item => item.CustomerPartOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerPartOrderItem>()
                .HasOne(item => item.Part)
                .WithMany()
                .HasForeignKey(item => item.PartId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
