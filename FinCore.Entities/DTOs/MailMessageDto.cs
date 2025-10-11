using System.ComponentModel.DataAnnotations;

namespace FinCore.Entities.DTOs;

public sealed class MailMessageDto
{
    public string From { get; set; } = default!;
    public string FromName { get; set; } = "FinCore";
    [Required]
    public string To { get; set; }
    public string Subject { get; set; } = "";
    public string HtmlBody { get; set; } = "";
    public string? TextBody { get; set; }
}