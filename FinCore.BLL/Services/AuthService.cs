using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinCore.BLL.Interfaces;
using FinCore.Core.Helpers;
using FinCore.DAL.Context;
using FinCore.Entities.DTOs;
using FinCore.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FinCore.BLL.Services;

public class AuthService(
    AppDbContext context, 
    IConfiguration config, 
    IRepository<User> userRepo, 
    IRepository<UserResetCode> userResetRepo, 
    IBudgetService budgetService,
    IMailTemplateRenderer templateRenderer,
    IMailService mailService
    ) : IAuthService
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

    public async Task<bool> SendResetPasswordCodeAsync(string email)
    {
        var user = await userRepo.Query()
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user == null)
            throw new Exception("Kullanıcı Bulunamadı!");

        var isCodeExist = await userResetRepo.Query()
            .AnyAsync(x => x.UserId == user.Id 
                           && !x.IsUsed
                           && x.ExpiresAt > DateTime.UtcNow);

        if (isCodeExist)
            throw new Exception("120 saniyede bir kod gönderebilirsiniz!");
        
        var resetCode = CodeGenerate.GenerateCode();


        await userResetRepo.AddAsync(new UserResetCode{
            Code = resetCode,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddMinutes(2),
            IsUsed = false
        });
        await userResetRepo.SaveChangesAsync();
        
        var template = templateRenderer.RenderAsync("reset-password", new { reset_code = resetCode });

        await mailService.SendAsync(new()
        {
            To = email,
            Subject = "Şifre Sıfırlama Kodu",
            HtmlBody = await template,
        });
        
        return true;
    }

    public async Task<bool> VerifyResetPasswordCode(VerifyPasswordResetDto request)
    {
        var user = await userRepo.Query()
            .FirstOrDefaultAsync(x => x.Email == request.email);

        if (user is null)
            throw new Exception("Kullanıcı Bulunamadı!");

        var resetCode = await userResetRepo.Query()
            .Where(x => x.UserId == user.Id && x.Code == request.code && !x.IsUsed)
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefaultAsync();
        
        if (resetCode is null)
            throw new Exception("Geçersiz Kod!");

        if (resetCode.ExpiresAt < DateTime.UtcNow)
            throw new Exception("Kodun geçerlilik süresi dolmuştur!");
        
        resetCode.IsUsed = true;
        resetCode.IsVerified = true;
        resetCode.UpdatedDate = DateTime.UtcNow;
        await userResetRepo.SaveChangesAsync();

        return true;
    }
    
    public async Task<bool> ResetPasswordAsync(PasswordResetDto request)
    {
        var user = await userRepo.Query()
            .FirstOrDefaultAsync(x => x.Email == request.email);

        if (user is null)
            throw new Exception("Kullanıcı Bulunamadı!");

        var resetCode = await userResetRepo.Query()
            .FirstOrDefaultAsync(x => x.UserId == user.Id
                      && x.IsUsed
                      && x.IsVerified
                      && x.Code == request.code);
        
        if (resetCode is null)
            throw new Exception("Doğrulama Geçersiz!");    
        
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.password);

        if (hashedPassword == user.Password)
        {
            throw new Exception("Yeni şifre eski şifrenizle aynı olamaz!");
        }
        
        user.Password = hashedPassword;
        user.UpdatedDate = DateTime.UtcNow;
        await userRepo.SaveChangesAsync();

        resetCode.IsVerified = false;
        await userResetRepo.SaveChangesAsync();

        
        return true;
    }
}