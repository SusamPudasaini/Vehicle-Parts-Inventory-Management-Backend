using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Vehicle_Parts_Inventory_Management.Data;
using Vehicle_Parts_Inventory_Management.Models;
using Vehicle_Parts_Inventory_Management.Interfaces;
using Vehicle_Parts_Inventory_Management.Services;

var builder = WebApplication.CreateBuilder(args);

// Database 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection 
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<ICustomerAuthService, CustomerAuthService>();
builder.Services.AddScoped<ICustomerPartPurchaseService, CustomerPartPurchaseService>();

// Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;


});

builder.Services.AddScoped<IPartService, PartService>();
builder.Services.AddScoped<IPurchaseInvoiceService, PurchaseInvoiceService>();

// DI for services
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IPartRequestService, PartRequestService>();
builder.Services.AddScoped<IServiceReviewService, ServiceReviewService>();

// API  
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger gropued dropdown
builder.Services.AddSwaggerGen(c =>
{
    // dropdown items ("Select a definition")
    c.SwaggerDoc("00-Auth", new OpenApiInfo { Title = "00-Auth APIs", Version = "v1" });
    c.SwaggerDoc("01-Customers", new OpenApiInfo { Title = "01-Customer APIs", Version = "v1" });
    c.SwaggerDoc("02-Staff", new OpenApiInfo { Title = "02-Staff APIs", Version = "v1" });
    c.SwaggerDoc("03-Vendors", new OpenApiInfo { Title = "03-Vendor APIs", Version = "v1" });
    c.SwaggerDoc("04-Vehicles", new OpenApiInfo { Title = "04-Vehicle APIs", Version = "v1" });
    c.SwaggerDoc("05-Inventory", new OpenApiInfo { Title = "05-Inventory/Parts APIs", Version = "v1" });
    c.SwaggerDoc("06-Appointments", new OpenApiInfo { Title = "06-Appointment APIs", Version = "v1" });
    c.SwaggerDoc("07-Reviews", new OpenApiInfo { Title = "07-Review APIs", Version = "v1" });
    c.SwaggerDoc("08-Invoices-Email", new OpenApiInfo { Title = "08-Invoice Email APIs", Version = "v1" });

    // anything without a group will still appear here
    c.SwaggerDoc("99-Other", new OpenApiInfo { Title = "99-Other APIs", Version = "v1" });

    // Put endpoints into the correct swagger document based on ApiExplorer GroupName
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        var groupName = apiDesc.GroupName;

        // If the controller has [ApiExplorerSettings(GroupName="...")], it goes there
        if (!string.IsNullOrWhiteSpace(groupName))
            return string.Equals(groupName, docName, StringComparison.OrdinalIgnoreCase);

        // If no group is assigned, show it under 99-Other so nothing disappears just to be safe
        return docName == "99-Other";
    });
});

// CORS (allow React dev server)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
        policy.WithOrigins(
            "http://localhost:5175",
            "http://localhost:5173",
            "http://localhost:3000",
            "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

// Middleware 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Dropdown entries (this match the SwaggerDoc names above)
        c.SwaggerEndpoint("/swagger/00-Auth/swagger.json", "00-Auth");
        c.SwaggerEndpoint("/swagger/01-Customers/swagger.json", "01-Customers");
        c.SwaggerEndpoint("/swagger/02-Staff/swagger.json", "02-Staff");
        c.SwaggerEndpoint("/swagger/03-Vendors/swagger.json", "03-Vendors");
        c.SwaggerEndpoint("/swagger/04-Vehicles/swagger.json", "04-Vehicles");
        c.SwaggerEndpoint("/swagger/05-Inventory/swagger.json", "05-Inventory");
        c.SwaggerEndpoint("/swagger/06-Appointments/swagger.json", "06-Appointments");
        c.SwaggerEndpoint("/swagger/07-Reviews/swagger.json", "07-Reviews");
        c.SwaggerEndpoint("/swagger/08-Invoices-Email/swagger.json", "08-Invoices-Email");
        c.SwaggerEndpoint("/swagger/99-Other/swagger.json", "99-Other");

        // Make Swagger easier to read
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // collapse endpoints by default
        c.DefaultModelsExpandDepth(-1); // hide schemas/models panel
        c.DisplayRequestDuration();
    });
}

app.UseCors("AllowReact");
app.UseSession();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
