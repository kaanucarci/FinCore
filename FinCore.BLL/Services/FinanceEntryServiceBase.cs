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
        var budget = await _budgetRepo.GetByIdAsync(budgetId)
                     ?? throw new ArgumentException("Budget not found.");
        return budget;
    }
    
    protected async Task FinanceEntryValidateAsync(decimal amount, int budgetId)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        var budget = ValidateBudgetAsync(budgetId).Result;

        if (amount > budget.Amount)
            throw new ArgumentException("Amount cannot exceed the budget total.");
    }
}