using FinCore.Entities.Models;

namespace FinCore.BLL.Interfaces;

public interface IBudgetService
{
       Task<List<Budget>> GetAllAsync();
       Task<Budget?> GetByIdAsync(int id);
       
       Task AddAsync(Budget budget);
       Task UpdateAsync(Budget budget);
       Task Delete(Budget budget);
}