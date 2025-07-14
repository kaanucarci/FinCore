using AutoMapper;
using FinCore.Api.DTOs;
using FinCore.BLL.Interfaces;
using FinCore.Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinCore.Api.Controllers;

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

    [HttpGet("id:{id}")]
    public async Task<ActionResult<BudgetDto.BudgetReadDto>> Get(int id)
    {
        var entity = await service.GetByIdAsync(id);
        return entity is null
            ? NotFound()
            : Ok(mapper.Map<BudgetDto.BudgetReadDto>(entity));
    }

    [HttpPost]
    public async Task<ActionResult<BudgetDto.BudgetReadDto>> Create(BudgetDto.BudgetCreateDto dto)
    {
        var entity = mapper.Map<Budget>(dto);
        await service.AddAsync(entity);

        var read = mapper.Map<BudgetDto.BudgetReadDto>(entity);
        return CreatedAtAction(nameof(Get), new { id = read.Id }, read);
    }
}