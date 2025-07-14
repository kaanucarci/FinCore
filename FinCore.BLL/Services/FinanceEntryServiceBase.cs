using FinCore.BLL.Interfaces;
using FinCore.Entities.Models;

namespace FinCore.BLL.Services;

public abstract class FinanceEntryServiceBase<T> where T : BaseEntity
{
    private readonly IRepository<T> _repo;
    private readonly IRepository<Budget> _budgetRepo;

    protected FinanceEntryServiceBase(IRepository<T> repo,
        IRepository<Budget> budgetRepo)
    {
        _repo       = repo;
        _budgetRepo = budgetRepo;
    }

    protected async Task<Budget> ValidateBudgetAsync(int budgetId)
    {
        var budget = await _budgetRepo.GetByIdAsync(budgetId);
                     
        return budget is null ? throw new ArgumentException("Budget not found.") : budget;
    }
    
    protected async Task FinanceEntryValidateAsync(decimal amount, int budgetId)
    {
        if (amount <= 0)
            throw new ArgumentException("Sıfırdan büyük bir miktar girmelisiniz.");

        var budget =  await ValidateBudgetAsync(budgetId);

        if (amount > budget.Amount)
            throw new ArgumentException("Girilen miktar kalan bütçeden fazla olamaz.");
    }
}