namespace LibraryManagement.Infrastructure.Security;

/// <summary>Binding cho section "JwtSettings" trong appsettings.json.</summary>
public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "LibraryManagement";
    public string Audience { get; set; } = "LibraryManagementClient";
    public int ExpiryMinutes { get; set; } = 480; // 8 giờ, đủ cho một phiên làm việc
}
