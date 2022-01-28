using System.Net.Mail;
using System.Text;

namespace LittleViet.Infrastructure.Email;

public class EmailSettings
{
    public const string ConfigSection = "EmailSettings";
    public static readonly string TemplateFolder = $"Email{Path.DirectorySeparatorChar}Templates";
    public string Host { get; set; }
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public SmtpDeliveryMethod DeliveryMethod { get; set; }
    public string Email { get; set; }
    public string FromAddress { get; set; }
    public System.Text.Encoding Encoding { get; set; } = Encoding.UTF8;
    public string FromName { get; set; }
    public string Password { get; set; }
    public int Timeout { get; set; } = 20000;
}