using AutoMapper;
using FinCore.Entities.Models;
using FinCore.Entities.DTOs;

namespace FinCore.Api.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Budget
        CreateMap<BudgetDto.BudgetCreateDto, Budget>()
            .ForMember(d => d.Amount,
                o => o.MapFrom(s => s.TotalAmount));
        CreateMap<Budget, BudgetDto.BudgetReadDto>();
        CreateMap<BudgetDto.BudgetUpdateDto, Budget>();
        
        // Expense
        CreateMap<ExpenseDto.ExpenseCreateDto, Expense>();
        CreateMap<Expense, ExpenseDto.ExpenseReadDto>();

        // Saving
        CreateMap<SavingDto.SavingCreateDto, Saving>();
        CreateMap<Saving, SavingDto.SavingReadDto>();
    }
}
