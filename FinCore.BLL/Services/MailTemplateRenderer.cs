using FinCore.BLL.Interfaces;

namespace FinCore.BLL.Services;

public sealed class FileMailTemplateRenderer : IMailTemplateRenderer
{
    private readonly string _basePath;

    public FileMailTemplateRenderer(string basePath)
    {
        _basePath = basePath;
    }

    public async Task<string> RenderAsync(string templateKey, object model, CancellationToken ct = default)
    {
        var filePath = Path.Combine(_basePath, $"{templateKey}.html");
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Mail template not found: {templateKey}");

        var template = await File.ReadAllTextAsync(filePath, ct);

        foreach (var prop in model.GetType().GetProperties())
        {
            var placeholder = $"{{{{{prop.Name}}}}}";
            template = template.Replace(placeholder, prop.GetValue(model)?.ToString());
        }

        return template;
    }
}
