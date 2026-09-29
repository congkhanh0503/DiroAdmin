namespace DiroAdmin.Api.Models;

public class Customer
{
    public int Id { get; set; }
    public string ShopCode { get; set; } = string.Empty; // VD: DP-CONG-01
    public string ShopName { get; set; } = string.Empty; // Tên quán
    public string OwnerName { get; set; } = string.Empty; // Tên chủ tiệm
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string BusinessModel { get; set; } = "Barber"; // Barber, Salon, Spa, Retail, Cafe
    public string CurrentPlan { get; set; } = "Trial"; // Trial, Monthly, Yearly, Lifetime
    public DateTime ActivatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(30);
    public string Status { get; set; } = "Active"; // Active, Suspended, Expired
    public string? HardwareId { get; set; } // Mã máy tính tại quán
    public string? ActiveLicenseKey { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastPingAt { get; set; }
}

public class LicenseRecord
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public string LicenseKey { get; set; } = string.Empty;
    public string PlanType { get; set; } = "Yearly"; // Monthly, Yearly, Lifetime
    public int Months { get; set; } = 12;
    public decimal Price { get; set; } = 0;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public string CreatedBy { get; set; } = "Admin";
}

public class AdminUser
{
    public int Id { get; set; }
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = "admin@123";
    public string FullName { get; set; } = "Quản Trị Viên DiroAdmin";
    public string Role { get; set; } = "SuperAdmin";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
