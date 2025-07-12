using FinCore.BLL.Interfaces;
using FinCore.Entities.Models;

namespace FinCore.BLL.Services;

public class SavingService :
    FinanceEntryServiceBase<Saving>, ISavingService
{
    private readonly IRepository<Saving> _repo;
    private readonly IRepository<Budget> _budgetRepo;

    public SavingService(IRepository<Saving> repo, IRepository<Budget> budgetRepo) : base(repo, budgetRepo)
    {
        _repo = repo;
        _budgetRepo = budgetRepo;
    }

    public async Task<List<Saving>> GetAllAsync(int budgetId)
    {
        await ValidateBudgetAsync(budgetId);
        return (await _repo.GetAllAsync()).Where(e => e.BudgetId == budgetId).ToList();
    }


    public async Task<Saving?> GetByIdAsync(int id)
        => await _repo.GetByIdAsync(id);

    public async Task AddAsync(Saving saving, int budgetId)
    {
        await FinanceEntryValidateAsync(saving.Amount, budgetId);
        await _repo.AddAsync(saving);
        await _repo.SaveChangesAsync();
    }

    public async Task UpdateAsync(Saving saving, int budgetId)
    {
        await FinanceEntryValidateAsync(saving.Amount, budgetId);
        _repo.Update(saving);
        await _repo.SaveChangesAsync();
    }

    public async Task DeleteAsync(Saving saving)
    {
        var exp = await _repo.GetByIdAsync(saving.Id);

        if (exp is not null)
        {
            _repo.Delete(exp);
            await _repo.SaveChangesAsync();
        }
    }
}