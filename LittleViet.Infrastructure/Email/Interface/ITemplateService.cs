using LittleViet.Infrastructure.Email.Models;

namespace LittleViet.Infrastructure.Email.Interface;

public interface ITemplateService
{
    Task<string> GetTemplateEmail(EmailTemplates.EmailTemplate templateName);
}