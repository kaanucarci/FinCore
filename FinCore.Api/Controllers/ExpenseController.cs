using AutoMapper;
using FinCore.Api.Hubs;
using FinCore.Entities.DTOs;
using FinCore.BLL.Interfaces;
using FinCore.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FinCore.Api.Controllers;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExpenseController(IExpenseService expenseService, IMapper mapper, IBudgetService budgetService, IHubContext<FinanceHub> hubContext) : ControllerBase
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
    
    [HttpGet("search")]
    public async Task<List<Expense>> Search([FromQuery] string key)
    {
        var expenses = await expenseService.SearchAsync(key);
        return expenses ?? new List<Expense>();
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto.ExpenseReadDto>> Create(ExpenseDto.ExpenseCreateDto dto)
    {
        var entity = mapper.Map<Expense>(dto);
        await expenseService.AddAsync(entity);
        
        var read = mapper.Map<ExpenseDto.ExpenseReadDto>(entity);
        await hubContext.Clients.All.SendAsync("ExpenseCreated", read);
        return CreatedAtAction(nameof(Get), new { expenseId = read.Id }, read);
    }
    
    [HttpPut("{expenseId}")]
    public async Task<ActionResult<ExpenseDto.ExpenseReadDto>> Update([FromRoute] int expenseId, ExpenseDto.ExpenseUpdateDto dto)
    {
        var entity = mapper.Map<Expense>(dto);
        await expenseService.UpdateAsync(expenseId, entity);
        
        var updated = await expenseService.GetByIdAsync(expenseId);
        if (updated == null)
            return NotFound();

        var read = mapper.Map<ExpenseDto.ExpenseReadDto>(updated);
        await hubContext.Clients.All.SendAsync("ExpenseUpdated", read);
        return Ok(read);
    }

    
    [HttpDelete("{expenseId}")]
    public async Task<OkObjectResult> Delete([FromRoute] int expenseId)
    {
        var expense = await expenseService.GetByIdAsync(expenseId);
        await expenseService.DeleteAsync(expenseId);
        await hubContext.Clients.All.SendAsync("ExpenseDeleted", expense);
        return Ok(expense);
    }
}