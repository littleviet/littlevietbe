namespace LittleViet.Infrastructure.Email.Interface;

public interface IEmailService
{
    Task<bool> SendEmailAsync(
        string toName,
        string toAddress,
        string subject,
        string body
    );
}