using FinCore.BLL.Interfaces;
using FinCore.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace FinCore.BLL.Services;

public class BudgetService : IBudgetService
{
    private readonly IRepository<Budget> _repo;

    public BudgetService(IRepository<Budget> repo) => _repo = repo;

    public async Task<List<Budget>> GetAllAsync()
        => await _repo.GetAllAsync();


    public async Task<Budget?> GetByIdAsync(int id)
        => await _repo.GetByIdAsync(id);

    public async Task AddAsync(Budget budget)
    {
        if (budget.Year < DateTime.Now.Year)
            throw new ArgumentException("Gecmis yıl bütçe ekleyemezsiniz!");
        
        if (budget.Month < 1 || budget.Month > 12)
            throw new ArgumentException("Ay değeri 1-12 arasinda olmalidir!");
        
        if (budget.TotalAmount <= 0)
            throw new ArgumentException("Bütce pozitif bir sayı olmalıdır!");
        
        if (await ExistsAsync(budget.Year, budget.Month))
            throw new ArgumentException("Aynı ay ve yıl için yalnızca bir bütçe kaydedilebilir!");
        
        await _repo.AddAsync(budget);
        await _repo.SaveChangesAsync();
    }

    public async Task UpdateAsync(Budget budget)
    {
        
        if (budget.Month < 1 || budget.Month > 12)
            throw new ArgumentException("Ay değeri 1-12 arasinda olmalidir!");
        
        
        if (await ExistsAsync(budget.Year, budget.Month))
            throw new ArgumentException("Aynı ay ve yıl için yalnızca bir bütçe kaydedilebilir!");
        
        _repo.Update(budget); 
        await _repo.SaveChangesAsync();
    }

    public async Task Delete(Budget budget)
    {
        var item = await _repo.GetByIdAsync(budget.Id);
        if (item is not null)
        {
            _repo.Delete(item);
            await _repo.SaveChangesAsync();
        }
        
        return;
    }
    
    public async Task<bool> ExistsAsync(int year, int month)
    {
        return await _repo.Query()
            .AnyAsync(b => b.Year == year && b.Month == month);
    }
}