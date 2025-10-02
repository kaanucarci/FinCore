using FinCore.BLL.Interfaces;
using FinCore.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace FinCore.BLL.Services;

public class ExpenseService :
    FinanceEntryServiceBase<Expense>, IExpenseService
{
    private readonly IRepository<Expense> _repo;
    private readonly IRepository<Budget> _budgetRepo;

    public ExpenseService(IRepository<Expense> repo, IRepository<Budget> budgetRepo) : base(repo, budgetRepo)
    {
        _repo = repo;
        _budgetRepo = budgetRepo;
    }

    public async Task<List<Expense>> GetAllAsync(int budgetId)
    {
        await ValidateBudgetAsync(budgetId);
        return (await _repo.GetAllAsync()).Where(e => e.BudgetId == budgetId).ToList();
    }


    public async Task<Expense?> GetByIdAsync(int id)
        => await _repo.GetByIdAsync(id);

    public async Task AddAsync(Expense expense)
    {
        await FinanceEntryValidateAsync(expense.Amount, expense.BudgetId);
        await _repo.AddAsync(expense);
        await _repo.SaveChangesAsync();
    }

    public async Task UpdateAsync(int expenseId, Expense expense)
    {
        var budget = await _budgetRepo.GetByIdAsync(expense.BudgetId);
        if (budget == null)
            throw new Exception("Budget not found");
        
        var existing = await _repo.GetByIdAsync(expenseId);
        if (existing == null)
            throw new Exception("Expense not found");

        if (budget.Amount < expense.Amount)
            throw new Exception("Budget amount is not enough");
        
        var diffrance = existing.Amount - expense.Amount;
        budget.Amount += diffrance;
        await _budgetRepo.SaveChangesAsync();
        
        existing.Amount = expense.Amount;
        existing.Description = expense.Description;
        existing.ExpenseType = expense.ExpenseType;
        existing.UpdatedDate = DateTime.UtcNow;
        await _repo.SaveChangesAsync();
    }

    public async Task<List<Expense>> SearchAsync(string key)
    {
        List<Expense> result =  await _repo.Query()
            .Where(e => e.Description.Contains(key))
            .ToListAsync();
        
        return result;
    }

    public async Task DeleteAsync(int expenseId)
    {
        var entity = await _repo.GetByIdAsync(expenseId);

        if (entity is not null)
        {
            var budget = await _budgetRepo.GetByIdAsync(entity.BudgetId);
            budget.Amount += entity.Amount;
            await _budgetRepo.SaveChangesAsync();
            
            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Expense Not Found");
        }
    }
}