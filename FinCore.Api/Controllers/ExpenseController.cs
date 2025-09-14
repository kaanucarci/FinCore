using AutoMapper;
using FinCore.Entities.DTOs;
using FinCore.BLL.Interfaces;
using FinCore.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinCore.Api.Controllers;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExpenseController(IExpenseService expenseService, IMapper mapper, IBudgetService budgetService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ExpenseDto.ExpenseCreateDto>> GetAll(int budgetId)
    {
        var list = await expenseService.GetAllAsync(budgetId);
        return list is null
            ? NotFound()
            : Ok(mapper.Map<List<ExpenseDto.ExpenseReadDto>>(list));
    }

    [HttpGet("{expenseId}")]
    public async Task<ActionResult<ExpenseDto.ExpenseReadDto>> Get(int expenseId)
    {
        var expense = await expenseService.GetByIdAsync(expenseId);
        return expense is null
            ? NotFound()
            : Ok(mapper.Map<ExpenseDto.ExpenseReadDto>(expense));
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto.ExpenseReadDto>> Create(ExpenseDto.ExpenseCreateDto dto)
    {
        var entity = mapper.Map<Expense>(dto);
        await expenseService.AddAsync(entity);
        
        var read = mapper.Map<ExpenseDto.ExpenseReadDto>(entity);
        return CreatedAtAction(nameof(Get), new { id = read.Id }, read);
    }
    
    [HttpPut("{expenseId}")]
    public async Task<ActionResult<ExpenseDto.ExpenseReadDto>> Update([FromRoute] int expenseId, ExpenseDto.ExpenseUpdateDto dto)
    {
        var entity = mapper.Map<Expense>(dto);
        await expenseService.UpdateAsync(expenseId, entity);
        
        var read = mapper.Map<ExpenseDto.ExpenseReadDto>(entity);
        return read;
    }
}