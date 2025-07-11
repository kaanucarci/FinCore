using FinCore.Entities.Models;

namespace FinCore.BLL.Interfaces;

public interface IExpenseService
{
    Task<List<Expense>> GetAllAsync(int budgetId);
    Task<Expense?> GetByIdAsync(int id);
    Task AddAsync(Expense expense);
    Task UpdateAsync(Expense expense);
    Task Delete(Expense expense);
}