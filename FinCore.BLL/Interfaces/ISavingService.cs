using FinCore.Entities.Models;

namespace FinCore.BLL.Interfaces;

public interface ISavingService
{
    Task<List<Saving>> GetAllAsync(int budgetId);
    Task<Saving?> GetByIdAsync(int id);
    Task AddAsync(Saving saving);
    Task UpdateAsync(Saving saving, int budgetId);
    Task DeleteAsync(Saving saving);
}