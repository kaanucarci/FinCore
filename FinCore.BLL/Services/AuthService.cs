using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinCore.BLL.Interfaces;
using FinCore.DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinCore.BLL.Services;

public class AuthService(AppDbContext context, IConfiguration config) : IAuthService
{
    public async Task<string> Authenticate(string username, string password)
    {
        var users = await context.Users.ToListAsync();
        var user = users.FirstOrDefault(u =>
            string.Equals(u.UserName, username, StringComparison.Ordinal)); // case-sensitive

        if (user == null || user.Password != password)
            throw new Exception("Invalid username or password");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("UserId", user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(int.Parse(config["Jwt:ExpireMinutes"] ?? "30")),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}