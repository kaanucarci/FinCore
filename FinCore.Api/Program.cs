using FinCore.Api.Mapping;
using FinCore.DAL.Context;
using Microsoft.EntityFrameworkCore;
using FinCore.BLL.Interfaces;
using FinCore.BLL.Services;
using FinCore.BLL.Repositories;         
using FinCore.Api.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));


builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>)); 
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();   
builder.Services.AddScoped<ISavingService,  SavingService>();    
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();