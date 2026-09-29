using DiroAdmin.Api.Data;
using DiroAdmin.Api.Models;
using DiroAdmin.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiroAdmin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly AdminDbContext _context;
    private readonly ILicenseGeneratorService _licenseService;

    public CustomersController(AdminDbContext context, ILicenseGeneratorService licenseService)
    {
        _context = context;
        _licenseService = licenseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers(
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? plan = null)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string s = search.Trim().ToLower();
            query = query.Where(c => c.ShopName.ToLower().Contains(s) || 
                                     c.ShopCode.ToLower().Contains(s) || 
                                     c.OwnerName.ToLower().Contains(s) || 
                                     c.Phone.Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(c => c.Status.ToLower() == status.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(plan))
        {
            query = query.Where(c => c.CurrentPlan.ToLower() == plan.Trim().ToLower());
        }

        var list = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();

        // Tự động đánh dấu hết hạn nếu ngày hiện tại vượt quá ExpiresAt
        var now = DateTime.UtcNow;
        foreach (var c in list)
        {
            if (c.Status == "Active" && c.ExpiresAt <= now)
            {
                c.Status = "Expired";
            }
        }

        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound(new { Message = "Không tìm thấy khách hàng." });

        var records = await _context.LicenseRecords
            .Where(r => r.CustomerId == id)
            .OrderByDescending(r => r.IssuedAt)
            .ToListAsync();

        return Ok(new
        {
            Customer = customer,
            LicenseHistory = records
        });
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer([FromBody] CreateCustomerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ShopName) || string.IsNullOrWhiteSpace(dto.Phone))
        {
            return BadRequest(new { Message = "Tên quán và Số điện thoại không được để trống." });
        }

        // Tự sinh ShopCode nếu chưa có
        string shopCode = dto.ShopCode?.Trim().ToUpper() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(shopCode))
        {
            string rand = Random.Shared.Next(1000, 9999).ToString();
            shopCode = $"DP-{rand}";
        }

        if (await _context.Customers.AnyAsync(c => c.ShopCode == shopCode))
        {
            return BadRequest(new { Message = $"Mã quán {shopCode} đã tồn tại. Vui lòng chọn mã khác." });
        }

        int months = dto.InitialMonths > 0 ? dto.InitialMonths : 1;
        var expires = DateTime.UtcNow.AddMonths(months);
        string plan = dto.PlanType ?? "Trial";

        // Tự sinh key bản quyền đầu tiên
        string key = _licenseService.GenerateKey(shopCode, dto.ShopName, plan, expires, dto.HardwareId);

        var customer = new Customer
        {
            ShopCode = shopCode,
            ShopName = dto.ShopName.Trim(),
            OwnerName = dto.OwnerName?.Trim() ?? "Chủ tiệm",
            Phone = dto.Phone.Trim(),
            Address = dto.Address?.Trim(),
            BusinessModel = dto.BusinessModel ?? "Barber",
            CurrentPlan = plan,
            ActivatedAt = DateTime.UtcNow,
            ExpiresAt = expires,
            Status = "Active",
            HardwareId = dto.HardwareId?.Trim(),
            ActiveLicenseKey = key,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        // Ghi lịch sử cấp key
        _context.LicenseRecords.Add(new LicenseRecord
        {
            CustomerId = customer.Id,
            LicenseKey = key,
            PlanType = plan,
            Months = months,
            Price = dto.Price,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = expires,
            CreatedBy = "Admin"
        });
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerDto dto)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        customer.ShopName = dto.ShopName?.Trim() ?? customer.ShopName;
        customer.OwnerName = dto.OwnerName?.Trim() ?? customer.OwnerName;
        customer.Phone = dto.Phone?.Trim() ?? customer.Phone;
        customer.Address = dto.Address?.Trim() ?? customer.Address;
        customer.BusinessModel = dto.BusinessModel ?? customer.BusinessModel;
        customer.Notes = dto.Notes ?? customer.Notes;

        await _context.SaveChangesAsync();
        return Ok(customer);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/generate-license")]
    public async Task<ActionResult> GenerateNewLicense(int id, [FromBody] GenerateLicenseDto dto)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound(new { Message = "Không tìm thấy khách hàng." });

        int months = dto.Months > 0 ? dto.Months : 12;
        // Nếu gói chưa hết hạn, cộng dồn tiếp từ ngày hết hạn cũ. Nếu đã hết hạn, tính từ hôm nay.
        DateTime baseDate = customer.ExpiresAt > DateTime.UtcNow ? customer.ExpiresAt : DateTime.UtcNow;
        DateTime newExpires = baseDate.AddMonths(months);

        string plan = dto.PlanType ?? "Yearly";
        string hwId = !string.IsNullOrWhiteSpace(dto.HardwareId) ? dto.HardwareId : (customer.HardwareId ?? string.Empty);

        string key = _licenseService.GenerateKey(customer.ShopCode, customer.ShopName, plan, newExpires, hwId);

        customer.ActiveLicenseKey = key;
        customer.CurrentPlan = plan;
        customer.ExpiresAt = newExpires;
        customer.Status = "Active";
        if (!string.IsNullOrWhiteSpace(hwId)) customer.HardwareId = hwId;

        _context.LicenseRecords.Add(new LicenseRecord
        {
            CustomerId = customer.Id,
            LicenseKey = key,
            PlanType = plan,
            Months = months,
            Price = dto.Price,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = newExpires,
            CreatedBy = "Admin"
        });

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = $"Đã cấp gia hạn thành công {months} tháng! Hạn mới đến: {newExpires:dd/MM/yyyy}",
            LicenseKey = key,
            ExpiresAt = newExpires,
            PlanType = plan
        });
    }

    [HttpPost("{id}/quick-extend")]
    public async Task<ActionResult> QuickExtend(int id, [FromBody] QuickExtendDto dto)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound(new { Message = "Không tìm thấy khách hàng." });

        int months = dto.Months > 0 ? dto.Months : 1;
        DateTime baseDate = customer.ExpiresAt > DateTime.UtcNow ? customer.ExpiresAt : DateTime.UtcNow;
        DateTime newExpires = baseDate.AddMonths(months);

        string plan = months switch
        {
            1 => "Monthly",
            3 => "Monthly",
            6 => "Monthly",
            >= 12 and < 60 => "Yearly",
            >= 60 => "Lifetime",
            _ => "Monthly"
        };

        string hwId = customer.HardwareId ?? string.Empty;
        string key = _licenseService.GenerateKey(customer.ShopCode, customer.ShopName, plan, newExpires, hwId);

        customer.ActiveLicenseKey = key;
        customer.CurrentPlan = plan;
        customer.ExpiresAt = newExpires;
        customer.Status = "Active"; // Tự động kích hoạt lại nếu đang bị khóa

        _context.LicenseRecords.Add(new LicenseRecord
        {
            CustomerId = customer.Id,
            LicenseKey = key,
            PlanType = plan,
            Months = months,
            Price = dto.Price,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = newExpires,
            CreatedBy = "Admin (QuickExtend)"
        });

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = $"Đã tự động gia hạn thêm {months} tháng cho quán '{customer.ShopName}'! Hạn mới: {newExpires:dd/MM/yyyy}.",
            ExpiresAt = newExpires,
            Status = customer.Status,
            PlanType = plan,
            LicenseKey = key
        });
    }

    [HttpPost("{id}/toggle-lock")]
    public async Task<ActionResult> ToggleLock(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        customer.Status = customer.Status == "Suspended" ? "Active" : "Suspended";
        await _context.SaveChangesAsync();

        string msg = customer.Status == "Suspended" ? "Đã khóa quán thành công." : "Đã mở khóa quán thành công.";
        return Ok(new { Message = msg, Status = customer.Status });
    }

    [HttpGet("dashboard-stats")]
    public async Task<ActionResult> GetDashboardStats()
    {
        var now = DateTime.UtcNow;
        var soonDate = now.AddDays(7);

        int total = await _context.Customers.CountAsync();
        int active = await _context.Customers.CountAsync(c => c.Status == "Active" && c.ExpiresAt > now);
        int expiringSoon = await _context.Customers.CountAsync(c => c.Status == "Active" && c.ExpiresAt > now && c.ExpiresAt <= soonDate);
        int suspendedOrExpired = await _context.Customers.CountAsync(c => c.Status == "Suspended" || c.ExpiresAt <= now);
        decimal totalRevenue = await _context.LicenseRecords.SumAsync(r => r.Price);

        return Ok(new
        {
            TotalCustomers = total,
            ActiveCount = active,
            ExpiringSoonCount = expiringSoon,
            LockedOrExpiredCount = suspendedOrExpired,
            TotalRevenue = totalRevenue
        });
    }
}

public class CreateCustomerDto
{
    public string? ShopCode { get; set; }
    public string ShopName { get; set; } = string.Empty;
    public string? OwnerName { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? BusinessModel { get; set; }
    public string? PlanType { get; set; } = "Trial";
    public int InitialMonths { get; set; } = 1;
    public decimal Price { get; set; } = 0;
    public string? HardwareId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateCustomerDto
{
    public string? ShopName { get; set; }
    public string? OwnerName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? BusinessModel { get; set; }
    public string? Notes { get; set; }
}

public class GenerateLicenseDto
{
    public string? PlanType { get; set; } = "Yearly";
    public int Months { get; set; } = 12;
    public decimal Price { get; set; } = 0;
    public string? HardwareId { get; set; }
}

public class QuickExtendDto
{
    public int Months { get; set; } = 1;
    public decimal Price { get; set; } = 0;
}

