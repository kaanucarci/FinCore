using FinCore.Entities.DTOs;
using FinCore.Entities.Models;

namespace FinCore.BLL.Interfaces;

public interface IBudgetService
{
       Task<List<Budget>> GetAllAsync();
       Task<Budget?> GetByIdAsync(int id);
       Task<BudgetInfoDto> GetInfoByIdAsync(int id);
       Task AddAsync(Budget budget);
       Task<BudgetInfoDto> UpdateAsync(Budget budget, int id);
       Task<List<BudgetListResponseDto>> GetListAsync(int id, BudgetListRequestDto budgetListDto, int page = 1);
       Task Delete(Budget budget);
}