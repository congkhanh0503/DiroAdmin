using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DiroAdmin.Api.Services;

public class LicensePayload
{
    public string ShopCode { get; set; } = string.Empty;
    public string ShopName { get; set; } = string.Empty;
    public string PlanType { get; set; } = "Yearly";
    public DateTime ExpiresAt { get; set; }
    public string HardwareId { get; set; } = string.Empty;
    public long IssuedTimestamp { get; set; }
}

public interface ILicenseGeneratorService
{
    string GenerateKey(string shopCode, string shopName, string planType, DateTime expiresAt, string? hardwareId = null);
    bool VerifyKey(string licenseKey, out LicensePayload? payload);
}

public class LicenseGeneratorService : ILicenseGeneratorService
{
    // Cùng Master Secret với DiroPos client
    private const string MASTER_SECRET = "DiroPos_Master_Secret_Key_@2026_Secure_License_System!";

    public string GenerateKey(string shopCode, string shopName, string planType, DateTime expiresAt, string? hardwareId = null)
    {
        var payload = new LicensePayload
        {
            ShopCode = shopCode.Trim(),
            ShopName = shopName.Trim(),
            PlanType = planType,
            ExpiresAt = expiresAt,
            HardwareId = hardwareId?.Trim() ?? string.Empty,
            IssuedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        string json = JsonSerializer.Serialize(payload);
        string payloadB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(MASTER_SECRET));
        byte[] sigBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payloadB64));
        string sigB64 = Convert.ToBase64String(sigBytes)
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        return $"{payloadB64}.{sigB64}";
    }

    public bool VerifyKey(string licenseKey, out LicensePayload? payload)
    {
        payload = null;
        if (string.IsNullOrWhiteSpace(licenseKey)) return false;

        string[] parts = licenseKey.Trim().Split('.');
        if (parts.Length != 2) return false;

        string payloadB64 = parts[0];
        string sigB64 = parts[1];

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(MASTER_SECRET));
        byte[] expectedSig = hmac.ComputeHash(Encoding.UTF8.GetBytes(payloadB64));
        string expectedSigB64 = Convert.ToBase64String(expectedSig)
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');

        if (!CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(sigB64), 
            Encoding.UTF8.GetBytes(expectedSigB64)))
        {
            return false;
        }

        try
        {
            string padded = payloadB64.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            byte[] jsonBytes = Convert.FromBase64String(padded);
            string json = Encoding.UTF8.GetString(jsonBytes);
            payload = JsonSerializer.Deserialize<LicensePayload>(json);
            return payload != null;
        }
        catch
        {
            return false;
        }
    }
}
