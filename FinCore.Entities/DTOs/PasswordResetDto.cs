using System.ComponentModel.DataAnnotations;

namespace FinCore.Entities.DTOs;

public class VerifyPasswordResetDto
{
    [Required]
    [EmailAddress]
    public string email { get; set; }
    [Required]
    public string code { get; set; }
}
public class PasswordResetDto
{
    [Required]
    [EmailAddress]
    public string email { get; set; }
    [Required]
    public string password { get; set; }
    [Required]
    public string code { get; set; }
}