using DiroAdmin.Api.Data;
using DiroAdmin.Api.Models;
using DiroAdmin.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiroAdmin.Api.Controllers;

[ApiController]
[Route("api/remote-license")]
public class RemoteLicenseController : ControllerBase
{
    private readonly AdminDbContext _context;
    private readonly ILicenseGeneratorService _licenseService;

    public RemoteLicenseController(AdminDbContext context, ILicenseGeneratorService licenseService)
    {
        _context = context;
        _licenseService = licenseService;
    }

    [HttpGet("check")]
    public async Task<ActionResult> CheckLicense(
        [FromQuery] string? shopCode = null, 
        [FromQuery] string? key = null, 
        [FromQuery] string? hw = null,
        [FromQuery] string? shopName = null)
    {
        Customer? customer = null;

        // 1. Ưu tiên tìm kiếm chính xác theo Hardware ID của máy
        if (!string.IsNullOrWhiteSpace(hw))
        {
            customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.HardwareId != null && c.HardwareId.ToLower() == hw.Trim().ToLower());
        }

        // 2. Nếu chưa tìm thấy theo HW, tìm theo ShopCode
        if (customer == null && !string.IsNullOrWhiteSpace(shopCode))
        {
            customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.ShopCode.ToLower() == shopCode.Trim().ToLower());
        }

        // 3. Nếu là máy mới hoàn toàn kết nối lần đầu (Auto-Discovery)
        if (customer == null)
        {
            if (string.IsNullOrWhiteSpace(hw))
            {
                return Ok(new
                {
                    IsAllowed = false,
                    Status = "NotFound",
                    Message = "Thiếu thông tin mã máy để nhận diện."
                });
            }

            // Tự động thêm máy mới này vào DiroAdmin để quản trị viên thấy và bấm duyệt
            string newShopCode = $"DP-{Random.Shared.Next(1000, 9999)}";
            string name = !string.IsNullOrWhiteSpace(shopName) ? shopName.Trim() : $"Quán Mới ({hw[..Math.Min(12, hw.Length)]})";
            var defaultExpires = DateTime.UtcNow.AddDays(7); // Cho dùng thử 7 ngày hoặc chờ duyệt
            string initKey = _licenseService.GenerateKey(newShopCode, name, "Trial", defaultExpires, hw);

            customer = new Customer
            {
                ShopCode = newShopCode,
                ShopName = name,
                OwnerName = "Chưa cập nhật",
                Phone = "Chưa có",
                CurrentPlan = "Trial",
                ActivatedAt = DateTime.UtcNow,
                ExpiresAt = defaultExpires,
                Status = "Active",
                HardwareId = hw.Trim(),
                ActiveLicenseKey = initKey,
                LastPingAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                Notes = "Máy POS tự động kết nối lần đầu"
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        // Cập nhật trạng thái Online và Hardware ID nếu chưa có
        customer.LastPingAt = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(hw) && string.IsNullOrEmpty(customer.HardwareId))
        {
            customer.HardwareId = hw.Trim();
        }
        await _context.SaveChangesAsync();

        var now = DateTime.UtcNow;

        if (customer.Status == "Suspended")
        {
            return Ok(new
            {
                IsAllowed = false,
                Status = "Suspended",
                ShopCode = customer.ShopCode,
                ShopName = customer.ShopName,
                PlanType = customer.CurrentPlan,
                ExpiresAt = customer.ExpiresAt,
                Message = "Quán của bạn đang bị tạm khóa bản quyền. Vui lòng liên hệ Admin để mở lại."
            });
        }

        if (customer.ExpiresAt <= now)
        {
            return Ok(new
            {
                IsAllowed = false,
                Status = "Expired",
                ShopCode = customer.ShopCode,
                ShopName = customer.ShopName,
                PlanType = customer.CurrentPlan,
                ExpiresAt = customer.ExpiresAt,
                Message = "Bản quyền phần mềm đã hết hạn sử dụng. Vui lòng liên hệ Admin để gia hạn."
            });
        }

        // Đảm bảo luôn có key hợp lệ tương ứng với hạn dùng mới nhất
        if (string.IsNullOrEmpty(customer.ActiveLicenseKey))
        {
            customer.ActiveLicenseKey = _licenseService.GenerateKey(
                customer.ShopCode, customer.ShopName, customer.CurrentPlan, customer.ExpiresAt, customer.HardwareId);
            await _context.SaveChangesAsync();
        }

        return Ok(new
        {
            IsAllowed = true,
            Status = "Active",
            ShopCode = customer.ShopCode,
            ShopName = customer.ShopName,
            PlanType = customer.CurrentPlan,
            ExpiresAt = customer.ExpiresAt,
            LicenseKey = customer.ActiveLicenseKey,
            Message = "Bản quyền hợp lệ."
        });
    }
}
