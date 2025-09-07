namespace FinCore.Api.DTOs;

public class BudgetDto
{
    public record BudgetCreateDto(int Month, int Year, decimal TotalAmount);
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