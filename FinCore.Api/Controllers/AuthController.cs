using FinCore.BLL.Interfaces;
using FinCore.Entities.DTOs;
using FinCore.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
        var token = await service.Authenticate(dto.Username, dto.Password);
        return Ok(new TokenDto { Token = token });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<TokenDto>> Register(RegisterDto dto)
    {
        var token = await service.Register(dto);
        return Ok(new TokenDto { Token = token });
    } 
}