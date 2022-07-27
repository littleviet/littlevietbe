using System.Reflection;
using LittleViet.Infrastructure.Email.Interface;
using LittleViet.Infrastructure.Email.Templates;

namespace LittleViet.Infrastructure.Email.Service;

internal class TemplateService : ITemplateService
{
    public async Task<string> GetTemplateEmail(EmailTemplates.EmailTemplate template) =>
        await File.ReadAllTextAsync(Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            EmailSettings.TemplateFolder,
            template.TemplateName));

    public async Task<string> FillTemplate(EmailTemplates.EmailTemplate template, Dictionary<string, string> values)
    {
        if (template.Keys.Except(values.Keys) is var except && except.Any())
            throw new InvalidOperationException(
                $"The following values are missing for the template {template.TemplateName}: {string.Join(", ", except)}");

        var templateString = await GetTemplateEmail(template);

        return template.Keys.Aggregate(templateString, 
            (current, key) => current.Replace($"{{{key}}}", values.GetValueOrDefault(key), StringComparison.InvariantCulture));
    }
}