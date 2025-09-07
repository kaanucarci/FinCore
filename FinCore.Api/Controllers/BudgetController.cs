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
public class BudgetController(IBudgetService service, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<BudgetDto.BudgetReadDto>> GetAll()
    {
        var list = await service.GetAllAsync();
        return Ok(mapper.Map<List<BudgetDto.BudgetReadDto>>(list));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BudgetDto.BudgetReadDto>> Get(int id)
    {
        var entity = await service.GetInfoByIdAsync(id);
        return Ok(mapper.Map<BudgetInfoDto>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<BudgetDto.BudgetReadDto>> Create(BudgetDto.BudgetCreateDto dto)
    {
        var entity = mapper.Map<Budget>(dto);
        await service.AddAsync(entity);

        var read = mapper.Map<BudgetDto.BudgetReadDto>(entity);
        return CreatedAtAction(nameof(Get), new { id = read.Id }, read);
    }
    
    [HttpPut("{budgetId}")]
    public async Task<ActionResult<BudgetDto.BudgetReadDto>> Update( [FromRoute] int budgetId, BudgetDto.BudgetUpdateDto dto)
    {
        var entity = mapper.Map<Budget>(dto);
        var updatedEntity = await service.UpdateAsync(entity, budgetId);

        var read = mapper.Map<BudgetInfoDto>(updatedEntity);
        return Ok(read); 
    }
}