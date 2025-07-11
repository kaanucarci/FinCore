using FinCore.BLL.Interfaces;
using FinCore.Entities.Models;

namespace FinCore.BLL.Services;

public class ExpenseService: IExpenseService
{
    private readonly IRepository<Expense> _repo;
    private readonly IRepository<Budget> _budgetRepo;

    public ExpenseService( IRepository<Expense> repo, IRepository<Budget> budgetRepo)
    {
        _repo = repo;
        _budgetRepo = budgetRepo;
    }

    public async Task<List<Expense>> GetAllAsync(int budgetId)
        => (await _repo.GetAllAsync()).Where(e => e.BudgetId == budgetId).ToList();

    public async Task<Expense?> GetByIdAsync(int id)
        => await _repo.GetByIdAsync(id);

    public async Task AddAsync(Expense expense)
    {
        if (expense.Amount <= 0)
            throw new ArgumentException("Miktar 0'dan buyuk olmalidir!");
        
        var budget = await _budgetRepo.GetByIdAsync(expense.BudgetId);
        
        if (budget is null)
            throw new ArgumentException("Bütçe bulunamadi!");
        
        if (expense.Amount > budget.Amount)
            throw new ArgumentException("Miktar bütçenin miktardan buyuk olamaz!");
        
        await _repo.AddAsync(expense);
        await _repo.SaveChangesAsync();
    }

    public async Task UpdateAsync(Expense expense)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(Expense expense)
    {
        throw new NotImplementedException();
    }
}