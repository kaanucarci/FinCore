using FinCore.BLL.Interfaces;
using FinCore.Entities.Models;

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

    public async Task UpdateAsync(Expense expense, int budgetId)
    {
        await FinanceEntryValidateAsync(expense.Amount, budgetId);
        _repo.Update(expense);
        await _repo.SaveChangesAsync();
    }

    public async Task DeleteAsync(Expense expense)
    {
        var exp = await _repo.GetByIdAsync(expense.Id);

        if (exp is not null)
        {
            _repo.Delete(exp);
            await _repo.SaveChangesAsync();
        }
    }
}