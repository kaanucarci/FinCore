using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Text;
using FinCore.BLL.Interfaces;
using FinCore.DAL.Context;
using FinCore.Entities.DTOs;
using FinCore.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinCore.BLL.Services;

public class AuthService(AppDbContext context, IConfiguration config, IRepository<User> userRepo, IBudgetService budgetService) : IAuthService
{
    public async Task<string> Authenticate(string email, string password)
    {
        var users = await context.Users.ToListAsync();
        
        var user = users.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.Ordinal));

        
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            throw new Exception("Invalid email or password");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("userId", user.Id.ToString())
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

    public async Task<string> Register(RegisterDto dto)
    {
        bool validateUser = await userRepo.Query()
            .AnyAsync(x => x.Email == dto.Email);

        if (validateUser)
            throw new Exception("User alreay exist!");
        
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Name = dto.Name,
            Surname = dto.Surname,
            Email = dto.Email,
            Password = hashedPassword
        };

        await userRepo.AddAsync(user);
        await userRepo.SaveChangesAsync();
        await budgetService.CreateYearAsync(DateTime.Now.Year, user.Id);

        var token = await Authenticate(dto.Email, dto.Password);
        return token;
    }
}