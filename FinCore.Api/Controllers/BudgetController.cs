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
    [HttpGet("get/{year}")]
    public async Task<ActionResult<BudgetDto.BudgetReadDto>> GetAll(int year)
    {
        var list = await service.GetAllAsync(year);
        return Ok(mapper.Map<List<BudgetDto.BudgetReadDto>>(list));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BudgetDto.BudgetReadDto>> Get(int id, [FromQuery] int budgetYear)
    {
        var entity = await service.GetInfoByIdAsync(id, budgetYear);
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
    
    [HttpGet("list/{budgetId}")]
    public async Task<ActionResult<BudgetDto.BudgetReadDto>> List( [FromRoute] int budgetId, [FromQuery] int page, [FromQuery] BudgetListRequestDto dto)
    {
        if (page < 1) 
            page = 1;
        
        var list = await service.GetListAsync(budgetId, dto, page);
        return Ok(list); 
    }
    
    [HttpGet("year")]
    public async Task<List<int>> Year()
    {
        return await service.GetYearsAsync();
    }
    
    [HttpPost("year/{year}")]
    public async Task<ActionResult> Year(int year)
    {
        await service.CreateYearAsync(year);
        return StatusCode(201, new 
        {
            status = 201,
            message = "Bütçe Yılı Oluşturuldu!"
        });
    }   
}