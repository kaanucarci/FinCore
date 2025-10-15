using FinCore.BLL.Interfaces;
using FinCore.Entities.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinCore.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login(LoginDto dto)
    {
        var token = await service.Authenticate(dto.Email, dto.Password);
        return Ok(new TokenDto { Token = token });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<TokenDto>> Register(RegisterDto request)
    {
        var token = await service.Register(request);
        return Ok(new TokenDto { Token = token });
    } 
    
    [AllowAnonymous]
    [HttpPost("send-reset-password-code")]
    public async Task<ActionResult> SendResetPasswordCode([FromBody] string email)
    {
        var status = await service.SendResetPasswordCodeAsync(email);

        if (status)
            return Ok();

        return BadRequest();
    } 
    
    [AllowAnonymous]
    [HttpPost("verify-reset-password-code")]
    public async Task<ActionResult> VerifyResetPasswordCode([FromBody] VerifyPasswordResetDto request)
    {
        var status = await service.VerifyResetPasswordCode(request);

        if (status)
            return Ok();

        return BadRequest();
    } 
    
    [AllowAnonymous]
    [HttpPatch("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] PasswordResetDto request)
    {
        var status = await service.ResetPasswordAsync(request);

        if (status)
            return Ok();

        return BadRequest();
    } 
}