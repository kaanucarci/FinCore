using AutoMapper;
using FinCore.Api.DTOs;
using FinCore.BLL.Interfaces;
using FinCore.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinCore.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SavingController(ISavingService savingService, IMapper mapper, IBudgetService budgetService): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SavingDto.SavingCreateDto>> GetAll(int budgetId)
    {
        var list = await savingService.GetAllAsync(budgetId);
        return list is null
            ? NotFound()
            : Ok(mapper.Map<List<SavingDto.SavingReadDto>>(list));
    }

    [HttpGet("id:{id}")]
    public async Task<ActionResult<SavingDto.SavingReadDto>> Get(int id)
    {
        var saving = await savingService.GetByIdAsync(id);
        return saving is null
            ? NotFound()
            : Ok(mapper.Map<SavingDto.SavingReadDto>(saving));
    }

    [HttpPost]
    public async Task<ActionResult<SavingDto.SavingReadDto>> Create(SavingDto.SavingCreateDto dto)
    {
        var entity = mapper.Map<Saving>(dto);
        await savingService.AddAsync(entity);
        
        var read = mapper.Map<SavingDto.SavingReadDto>(entity);
        return CreatedAtAction(nameof(Get), new { id = read.Id }, read);
    }   
}