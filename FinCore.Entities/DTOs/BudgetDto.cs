using System.ComponentModel.DataAnnotations;
using FinCore.Entities.Models;

namespace FinCore.Entities.DTOs;

public class BudgetDto
{
    public record BudgetCreateDto(int Month, int Year, decimal TotalAmount);
    public record BudgetUpdateDto(int Month, int Year, decimal TotalAmount);
    public record BudgetReadDto(int Id, int Year, int Month, decimal TotalAmount, decimal Amount);
}

public class BudgetInfoDto
{
    public int BudgetId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Amount { get; set; }
    public decimal Saving { get; set; }
    public decimal Expense { get; set; }
}

public class BudgetListRequestDto
{
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public ExpenseType? ExpenseType { get; set; } = null;
    [Required]
    public int BudgetYear { get; set; }
}

public class BudgetListResponseDto
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<ExpenseListDto> Expenses { get; set; } = new();
    public List<ExpenseListDto> Savings { get; set; } = new();
}


public enum ExpenseType
{
    None,   
    Expense,
    Saving  
}

public class ExpenseListDto
{
    public int Id { get; set; }
    public int BudgetId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public ExpenseType? ExpenseType { get; set; }
}