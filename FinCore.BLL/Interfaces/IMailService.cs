using FinCore.Entities.DTOs;

namespace FinCore.BLL.Interfaces;

public interface IMailService
{
    Task SendAsync(MailMessageDto message, CancellationToken ct = default);
}

public interface IMailTemplateRenderer
{
    Task<string> RenderAsync(string templateKey, object model, CancellationToken ct = default);
}