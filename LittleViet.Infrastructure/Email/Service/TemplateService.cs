using System.Reflection;
using System.IO;
using LittleViet.Infrastructure.Email.Interface;
using LittleViet.Infrastructure.Email.Models;

namespace LittleViet.Infrastructure.Email.Service;

public class TemplateService : ITemplateService
{
    public async Task<string> GetTemplateEmail(EmailTemplates.EmailTemplate template)
    {
        var location = Assembly.GetExecutingAssembly().Location;
        return await File.ReadAllTextAsync(Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            EmailSettings.TemplateFolder,
            template.TemplateName));
    }

    public async Task<string> FillTemplate(EmailTemplates.EmailTemplate template)
    {
        return await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "Email", "Templates",
            template.TemplateName));
    }
}