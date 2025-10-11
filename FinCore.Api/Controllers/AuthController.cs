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
    public async Task<ActionResult<TokenDto>> Register(RegisterDto dto)
    {
        var token = await service.Register(dto);
        return Ok(new TokenDto { Token = token });
    } 
    
    [AllowAnonymous]
    [HttpPost("send-reset-password-code")]
    public async Task<ActionResult> SendResetPasswordCode(List<string> email)
    {
        var status = await service.SendResetPasswordCodeAsync(email.First());

        if (status)
            return Ok();

        return BadRequest();
    } 
    
    [AllowAnonymous]
    [HttpPost("verify-reset-password-code")]
    public async Task<ActionResult> VerifyResetPasswordCode(List<string> email, string code)
    {
        var status = await service.VerifyResetPasswordCode(code, email.First());

        if (status)
            return Ok();

        return BadRequest();
    } 
}