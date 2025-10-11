using System.ComponentModel.DataAnnotations;

namespace FinCore.Entities.DTOs;

public class RegisterDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Surname { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}