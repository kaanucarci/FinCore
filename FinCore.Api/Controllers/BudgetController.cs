using AutoMapper;
using FinCore.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetController : ControllerBase
{
    private readonly IBudgetService _service;
    private readonly IMapper _mapper;

    public BudgetController(IBudgetService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }
    
    
}