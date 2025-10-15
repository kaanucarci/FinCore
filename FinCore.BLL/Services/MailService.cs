using FinCore.BLL.Interfaces;
using FinCore.BLL.Options;
using FinCore.Entities.DTOs;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace FinCore.BLL.Services;


public sealed class MailService : IMailService
{
    private readonly SmtpOptions _smtp;
    private readonly ILogger<MailService> _logger;

    public MailService(IOptions<SmtpOptions> smtpOptions, ILogger<MailService> logger)
    {
        _smtp = smtpOptions.Value;
        _logger = logger;
    }

    public async Task SendAsync(MailMessageDto message, CancellationToken ct = default)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(message.FromName ?? _smtp.SenderName, message.From ?? _smtp.SenderEmail));
        //foreach (var to in message.To)
            email.To.Add(MailboxAddress.Parse(message.To));

        email.Subject = message.Subject;

        var builder = new BodyBuilder
        {
            HtmlBody = message.HtmlBody,
            TextBody = message.TextBody ?? "Bu e-posta yalnızca HTML olarak görüntülenebilir."
        };

        email.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(_smtp.Username, _smtp.Password, ct);
            await client.SendAsync(email, ct);
            Console.WriteLine("Mail sent successfully to :" + string.Join(", ", message.To));
            _logger.LogInformation("Mail sent successfully to {Recipients}", string.Join(", ", message.To));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Mail sending failed : " + ex);
            _logger.LogError(ex, "Mail sending failed.");
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true, ct);
        }
    }
}