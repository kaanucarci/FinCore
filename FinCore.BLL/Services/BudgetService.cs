using FinCore.BLL.Helpers;
using FinCore.BLL.Interfaces;
using FinCore.Entities.DTOs;
using FinCore.Entities.Interfaces;
using FinCore.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace FinCore.BLL.Services;

public class BudgetService(
    IRepository<Budget> budgetRepo,
    IRepository<Expense> expenseRepo,
    IUserContext userContext
) : IBudgetService
{
    public async Task<List<Budget>> GetAllAsync(int year)
        => (await budgetRepo.GetAllAsync())
            .Where(b => b.Year == year)
            .OrderBy(b => b.Month)
            .ToList();

    public async Task<Budget?> GetByIdAsync(int id)
    {
        var entity = budgetRepo.Query()
            .Where(x => x.Id == id && x.UserId == userContext.UserId)
            .FirstOrDefaultAsync();

        return await entity;
    }

    public async Task<BudgetInfoDto> GetInfoByIdAsync(int id, int budgetYear)
    {
        var budget = await budgetRepo.GetByIdAsync(id);
        if (budget is null)
            throw new Exception("Bütçe Bulunamadı!");

        var expenseAmount = await expenseRepo.Query()
            .Where(b => b.BudgetId == id && b.Budget.Year == budgetYear && b.ExpenseType == ExpenseType.Expense)
            .SumAsync(b => b.Amount);

        var savingAmount = await expenseRepo.Query()
            .Where(b => b.BudgetId == id && b.Budget.Year == budgetYear && b.ExpenseType == ExpenseType.Saving)
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
            throw new ArgumentException("Geçmiş yıl bütçe ekleyemezsiniz!");

        if (budget.Month < 1 || budget.Month > 12)
            throw new ArgumentException("Ay değeri 1-12 arasinda olmalıdır!");

        if (budget.TotalAmount < 0)
            throw new ArgumentException("Bütce pozitif bir sayı olmalıdır!");

        if (await ExistsAsync(budget.Year, budget.Month))
            throw new ArgumentException("Aynı ay ve yıl için yalnızca bir bütçe kaydedilebilir!");
        
        budget.UserId = userContext.UserId;
        
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
            .Where(b => b.BudgetId == budgetId && b.ExpenseType == ExpenseType.Expense)
            .SumAsync(b => b.Amount);

        var savingAmount = await expenseRepo.Query()
            .Where(b => b.BudgetId == budgetId && b.ExpenseType == ExpenseType.Saving)
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

    public async Task<List<BudgetListResponseDto>> GetListAsync(int budgetId, BudgetListRequestDto budgetListDto,
        int page = 1)
    {
        var pageSize = 5;
        var entity = await budgetRepo.GetByIdAsync(budgetId);

        if (entity is null)
            throw new Exception("Bütçe Bulunamadı!");
        
        List<ExpenseListDto> expenseList = new();
        
        //if (budgetListDto.ExpenseType is null || budgetListDto.ExpenseType != ExpenseType.Saving)
        //{
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
                ExpenseType = e.ExpenseType
            })
            .ToListAsync();
        //}

        return new List<BudgetListResponseDto>
        {
            new BudgetListResponseDto
            {
                TotalCount = totalExpenses,
                Page = page,
                PageSize = pageSize,
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

    public async Task<List<int>> GetYearsAsync()
    {
        var result = await budgetRepo.Query()
            .Select(b => b.Year)
            .Distinct()
            .ToListAsync();

        return result;
    }

    public async Task CreateYearAsync(int year)
    {
        var isExist = await budgetRepo.Query().AnyAsync(x => x.Year == year);
        if (!isExist)
        {
           
            for (int i = 1; i <= 12; i++)
            {
                var budget = new Budget
                {
                    Year = year,
                    Month =  i,
                    TotalAmount = 0,
                    Amount = 0
                };

                await AddAsync(budget);
            }
            return;
        }   
        else
        {
            throw new Exception("Budget year already exists!");
        }
    }
}