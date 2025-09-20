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

    public async Task AddAsync(Saving saving)
    {
        await FinanceEntryValidateAsync(saving.Amount, saving.BudgetId);
        await _repo.AddAsync(saving);
        await _repo.SaveChangesAsync();
    }

    public async Task UpdateAsync(int savingId, Saving saving)
    {
        var existing = await _repo.GetByIdAsync(savingId);
        if (existing == null)
            throw new Exception("Saving not found");
        
        existing.Amount = saving.Amount;
        existing.Description = saving.Description;
        existing.UpdatedDate = DateTime.UtcNow;

        await _repo.SaveChangesAsync();
    }


    public async Task DeleteAsync(int savingId)
    {
        var entity = await _repo.GetByIdAsync(savingId);

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