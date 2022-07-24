using LittleViet.Infrastructure.Email.Interface;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace LittleViet.Infrastructure.Email.Service;

internal class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value ?? throw new ArgumentNullException(nameof(emailSettings));
    }

    public async Task<bool> SendEmailAsync(
        string toName,
        string toAddress,
        string subject,
        string body
    )
    {
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_emailSettings.Email, _emailSettings.Password);
        var message = ComposeEmail(toName, toAddress, subject, body);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
        return true;
    }
    
    private MimeMessage ComposeEmail(
        string toName,
        string toAddress,
        string subject,
        string body
        )
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(
            _emailSettings.Encoding,
            _emailSettings.FromName,
            _emailSettings.FromAddress
        ));

        email.To.Add(new MailboxAddress(
            _emailSettings.Encoding,
            toName,
            toAddress
        ));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = body };
        return email;
    }
}