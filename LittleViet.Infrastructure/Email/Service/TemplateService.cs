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

    public async Task<string> FillTemplate(EmailTemplates.EmailTemplate template, Dictionary<string, string> values)
    {
        if (template.Keys.Except(values.Keys) is var except && except.Any())
            throw new InvalidOperationException(
                $"The following values are missing for the template {template.TemplateName}: {except}");

        var templateString = await GetTemplateEmail(template);

        return template.Keys.Aggregate(templateString, 
            (current, key) => current.Replace($"{{{key}}}", values.GetValueOrDefault(key), StringComparison.InvariantCulture));
    }
}