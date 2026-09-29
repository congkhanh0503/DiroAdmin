using DiroAdmin.Api.Data;
using DiroAdmin.Api.Models;
using DiroAdmin.Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Cổng chạy độc lập cho DiroAdmin (5020)
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
{
    builder.WebHost.UseUrls("http://0.0.0.0:5020");
}

string dbPath = Environment.GetEnvironmentVariable("DB_PATH") 
    ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "diroadmin.db");

string? dbDir = Path.GetDirectoryName(dbPath);
if (!string.IsNullOrEmpty(dbDir) && !Directory.Exists(dbDir))
{
    Directory.CreateDirectory(dbDir);
}

// 1. Cấu hình SQLite cho DiroAdmin
builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// 2. Đăng ký Services
builder.Services.AddScoped<ILicenseGeneratorService, LicenseGeneratorService>();

// 3. Controllers & JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// 4. Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Tự động khởi tạo DB & Seed dữ liệu mẫu
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
    context.Database.EnsureCreated();

    // Bật WAL mode cho SQLite
    context.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");

    // Seed Admin User
    if (!context.AdminUsers.Any())
    {
        context.AdminUsers.Add(new AdminUser
        {
            Username = "admin",
            Password = "password123",
            FullName = "Founder & SuperAdmin DiroPos"
        });
        context.SaveChanges();
    }

    // Seed 2 khách hàng mẫu nếu chưa có
    if (!context.Customers.Any())
    {
        var licenseService = scope.ServiceProvider.GetRequiredService<ILicenseGeneratorService>();

        var exp1 = DateTime.UtcNow.AddMonths(12);
        string key1 = licenseService.GenerateKey("DP-CONG-01", "Cộng Barber Shop", "Yearly", exp1);

        var exp2 = DateTime.UtcNow.AddDays(7); // Sắp hết hạn để test
        string key2 = licenseService.GenerateKey("DP-SALON-02", "Minh Salon & Spa", "Monthly", exp2);

        context.Customers.AddRange(
            new Customer
            {
                ShopCode = "DP-CONG-01",
                ShopName = "Cộng Barber Shop",
                OwnerName = "Nguyễn Thành Công",
                Phone = "0987.654.321",
                Address = "128 Nguyễn Văn Cừ, Quận 5, TP.HCM",
                BusinessModel = "Barber",
                CurrentPlan = "Yearly",
                ActivatedAt = DateTime.UtcNow.AddMonths(-1),
                ExpiresAt = exp1,
                Status = "Active",
                ActiveLicenseKey = key1,
                Notes = "Khách thanh toán chuyển khoản gói 1 năm 1.500.000đ"
            },
            new Customer
            {
                ShopCode = "DP-SALON-02",
                ShopName = "Minh Salon & Spa",
                OwnerName = "Trần Quang Minh",
                Phone = "0912.345.678",
                Address = "45 Lê Lợi, Quận 1, TP.HCM",
                BusinessModel = "Salon",
                CurrentPlan = "Monthly",
                ActivatedAt = DateTime.UtcNow.AddDays(-23),
                ExpiresAt = exp2,
                Status = "Active",
                ActiveLicenseKey = key2,
                Notes = "Khách đang dùng gói tháng, sắp đến kỳ gia hạn"
            }
        );
        context.SaveChanges();

        // Ghi nhận doanh thu mẫu
        context.LicenseRecords.AddRange(
            new LicenseRecord
            {
                CustomerId = 1,
                LicenseKey = key1,
                PlanType = "Yearly",
                Months = 12,
                Price = 1500000,
                IssuedAt = DateTime.UtcNow.AddMonths(-1),
                ExpiresAt = exp1
            },
            new LicenseRecord
            {
                CustomerId = 2,
                LicenseKey = key2,
                PlanType = "Monthly",
                Months = 1,
                Price = 150000,
                IssuedAt = DateTime.UtcNow.AddDays(-23),
                ExpiresAt = exp2
            }
        );
        context.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "DiroAdmin Central API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
