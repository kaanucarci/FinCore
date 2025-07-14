using AutoMapper;
using FinCore.Api.DTOs;
using FinCore.BLL.Interfaces;
using FinCore.Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpenseController(IExpenseService service, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ExpenseDto.ExpenseCreateDto>> GetAll(int budgetId)
    {
        var list = await service.GetAllAsync(budgetId);
        return list is null
            ? NotFound()
            : Ok(mapper.Map<List<ExpenseDto.ExpenseReadDto>>(list));
    }

    [HttpGet("id:{id}")]
    public async Task<ActionResult<ExpenseDto.ExpenseReadDto>> Get(int id)
    {
        var expense = await service.GetByIdAsync(id);
        return expense is null
            ? NotFound()
            : Ok(mapper.Map<ExpenseDto.ExpenseReadDto>(expense));
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto.ExpenseReadDto>> Create(ExpenseDto.ExpenseCreateDto dto)
    {
        var entity = mapper.Map<Expense>(dto);
        await service.AddAsync(entity);
        
        var read = mapper.Map<ExpenseDto.ExpenseReadDto>(entity);
        return CreatedAtAction(nameof(Get), new { id = read.Id }, read);
    }
}