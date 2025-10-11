using FinCore.Api.Mapping;
using FinCore.DAL.Context;
using Microsoft.EntityFrameworkCore;
using FinCore.BLL.Interfaces;
using FinCore.BLL.Services;
using FinCore.BLL.Repositories;         
using FinCore.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.SignalR;
using FinCore.Api.Hubs;
using FinCore.Entities.Interfaces;
using FinCore.BLL.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,                     
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>)); 
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();   
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));
builder.Services.AddScoped<IMailService, MailService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();


builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

var templatePath = Path.Combine(builder.Environment.ContentRootPath, "Templates");
builder.Services.AddSingleton<IMailTemplateRenderer>(new FileMailTemplateRenderer(templatePath));



builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FinCore API",
        Version = "v1"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securityScheme,
            new string[] { }
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins(
                "https://finjs.vercel.app", 
                "http://localhost:3000",
                "capacitor://localhost"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
});


app.MapControllers();
app.MapHub<FinanceHub>("/hubs/finance");

app.Run();
