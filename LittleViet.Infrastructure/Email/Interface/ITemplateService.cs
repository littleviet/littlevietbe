using LittleViet.Infrastructure.Email.Models;
using LittleViet.Infrastructure.Email.Templates;

namespace LittleViet.Infrastructure.Email.Interface;

public interface ITemplateService
{
    Task<string> GetTemplateEmail(EmailTemplates.EmailTemplate templateName);
    Task<string> FillTemplate(EmailTemplates.EmailTemplate template, Dictionary<string, string> values);

}