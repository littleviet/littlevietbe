namespace LittleViet.Infrastructure.Security;

public class AppSettings
{
    public const string ConfigSection = "AppSettings";
    public string JwtSecret { get; set; }
    public List<string> AllowedOrigins { get; set; }
}