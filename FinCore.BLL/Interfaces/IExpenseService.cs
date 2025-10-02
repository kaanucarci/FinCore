using FinCore.Entities.DTOs;
using FinCore.Entities.Models;

namespace FinCore.BLL.Interfaces;

public interface IExpenseService
{
    Task<List<Expense>> GetAllAsync(int budgetId);
    Task<Expense?> GetByIdAsync(int id);
    Task AddAsync(Expense expense);
    Task UpdateAsync(int expenseId, Expense expense);
    Task DeleteAsync(int expenseId);
    Task<List<Expense>> SearchAsync(string key);
}