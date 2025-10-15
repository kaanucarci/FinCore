using FinCore.Entities.DTOs;
using FinCore.Entities.Models;

namespace FinCore.BLL.Interfaces;

public interface IAuthService
{
    Task<string> Authenticate(string username, string password);
    Task<string> Register(RegisterDto dto);
    Task<bool> SendResetPasswordCodeAsync(string email);
    Task<bool> VerifyResetPasswordCode(VerifyPasswordResetDto request);
    Task<bool> ResetPasswordAsync(PasswordResetDto request);
}