using FinCore.BLL.Helpers;
using FinCore.BLL.Interfaces;
using FinCore.Entities.DTOs;
using FinCore.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace FinCore.BLL.Services;

public class BudgetService(
    IRepository<Budget> budgetRepo,
    IRepository<Expense> expenseRepo,
    IRepository<Saving> savingRepo
) : IBudgetService
{
    public async Task<List<Budget>> GetAllAsync()
        => (await budgetRepo.GetAllAsync())
            .OrderBy(b => b.Month)
            .ToList();

    public async Task<Budget?> GetByIdAsync(int id)
        => await budgetRepo.GetByIdAsync(id);

    public async Task<BudgetInfoDto> GetInfoByIdAsync(int id)
    {
        var budget = await budgetRepo.GetByIdAsync(id);
        if (budget is null)
            throw new Exception("Bütçe Bulunamadı!");

        var expenseAmount = await expenseRepo.Query()
            .Where(b => b.BudgetId == id)
            .SumAsync(b => b.Amount);

        var savingAmount = await savingRepo.Query()
            .Where(b => b.BudgetId == id)
            .SumAsync(b => b.Amount);


        return new BudgetInfoDto
        {
            BudgetId = budget.Id,
            Year = budget.Year,
            Month = budget.Month,
            TotalAmount = budget.TotalAmount,
            Amount = budget.Amount,
            Saving = savingAmount,
            Expense = expenseAmount
        };
    }

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

        await budgetRepo.AddAsync(budget);
        await budgetRepo.SaveChangesAsync();
    }

    public async Task<BudgetInfoDto> UpdateAsync(Budget budget, int budgetId)
    {
        var entity = await budgetRepo.GetByIdAsync(budgetId);
        if (entity is null)
            throw new Exception("Bütçe Bulunamadı!");

        if (budget.Month < 1 || budget.Month > 12)
            throw new ArgumentException("Ay değeri 1-12 arasinda olmalidir!");

        if (budget.TotalAmount <= 0)
            throw new ArgumentException("Bütce pozitif bir sayı olmalıdır!");

        if (await ExistsAsync(budget.Year, budget.Month) && budget.Month != entity.Month)
            throw new ArgumentException("Aynı ay ve yıl için yalnızca bir bütçe kaydedilebilir!");

        var expenseAmount = await expenseRepo.Query()
            .Where(b => b.BudgetId == budgetId)
            .SumAsync(b => b.Amount);

        var savingAmount = await savingRepo.Query()
            .Where(b => b.BudgetId == budgetId)
            .SumAsync(b => b.Amount);

        var totalExpense = expenseAmount + savingAmount;

        if (budget.TotalAmount < totalExpense)
            throw new ArgumentException("Harcamalarınızdan daha düşük bütçe kaydedemezsiniz!");


        entity.TotalAmount = budget.TotalAmount;
        entity.Amount = budget.TotalAmount - totalExpense;
        entity.Month = budget.Month;
        entity.Year = budget.Year;

        budgetRepo.Update(entity);
        await budgetRepo.SaveChangesAsync();


        return new BudgetInfoDto
        {
            BudgetId = entity.Id,
            Year = entity.Year,
            Month = entity.Month,
            TotalAmount = entity.TotalAmount,
            Amount = entity.Amount,
            Saving = savingAmount,
            Expense = expenseAmount
        };
    }

    public async Task<List<BudgetListResponseDto>> GetListAsync(int budgetId, BudgetListRequestDto budgetListDto, int page = 1)
    {
        var pageSize = 15;
        var entity = await budgetRepo.GetByIdAsync(budgetId);

        if (entity is null)
            throw new Exception("Bütçe Bulunamadı!");

        List<ExpenseListDto> savingList = new();
        List<ExpenseListDto> expenseList = new();

        // --- SAVINGS ---
        if (budgetListDto.ExpenseType is null || budgetListDto.ExpenseType != ExpenseType.Expense)
        {
            var savings = savingRepo.Query()
                .Where(b => b.BudgetId == budgetId && b.Budget.Year == budgetListDto.BudgetYear);

            if (budgetListDto.StartDate is not null)
            {
                var startDate = BudgetHelper.ParseAndValidateSearchDate(budgetListDto.StartDate);
                savings = savings.Where(b => b.CreatedDate.Date >= startDate.Date);
            }

            if (budgetListDto.EndDate is not null)
            {
                var endDate = BudgetHelper.ParseAndValidateSearchDate(budgetListDto.EndDate);
                savings = savings.Where(b => b.CreatedDate.Date <= endDate.Date);
            }

            var totalSavings = await savings.CountAsync();

            savingList = await savings
                .OrderByDescending(s => s.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new ExpenseListDto
                {
                    Id = s.Id,
                    BudgetId = s.BudgetId,
                    Amount = s.Amount,
                    Description = s.Description,
                    CreatedDate = s.CreatedDate,
                    UpdatedDate = s.UpdatedDate != null ? s.UpdatedDate : null,
                    ExpenseType = ExpenseType.Saving
                })
                .ToListAsync();
        }

        // --- EXPENSES ---
        if (budgetListDto.ExpenseType is null || budgetListDto.ExpenseType != ExpenseType.Saving)
        {
            var expenses = expenseRepo.Query()
                .Where(b => b.BudgetId == budgetId && b.Budget.Year == budgetListDto.BudgetYear);

            if (budgetListDto.StartDate is not null)
            {
                var startDate = BudgetHelper.ParseAndValidateSearchDate(budgetListDto.StartDate);
                expenses = expenses.Where(b => b.CreatedDate.Date >= startDate.Date);
            }

            if (budgetListDto.EndDate is not null)
            {
                var endDate = BudgetHelper.ParseAndValidateSearchDate(budgetListDto.EndDate);
                expenses = expenses.Where(b => b.CreatedDate.Date <= endDate.Date);
            }

            var totalExpenses = await expenses.CountAsync();

            expenseList = await expenses
                .OrderByDescending(e => e.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new ExpenseListDto
                {
                    Id = e.Id,
                    BudgetId = e.BudgetId,
                    Amount = e.Amount,
                    Description = e.Description,
                    CreatedDate = e.CreatedDate,
                    UpdatedDate = e.UpdatedDate != null ? e.UpdatedDate : null,
                    ExpenseType = ExpenseType.Expense
                })
                .ToListAsync();
        }

        return new List<BudgetListResponseDto>
        {
            new BudgetListResponseDto
            {
                TotalCount = savingList.Count + expenseList.Count,
                Page = page,
                PageSize = pageSize * 2,
                Savings = savingList,
                Expenses = expenseList
            }
        };
    }


    public async Task Delete(Budget budget)
    {
        var item = await budgetRepo.GetByIdAsync(budget.Id);
        if (item is not null)
        {
            budgetRepo.Delete(item);
            await budgetRepo.SaveChangesAsync();
        }

        return;
    }

    public async Task<bool> ExistsAsync(int year, int month)
    {
        return await budgetRepo.Query()
            .AnyAsync(b => b.Year == year && b.Month == month);
    }
}