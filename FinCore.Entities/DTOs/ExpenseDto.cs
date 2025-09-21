namespace FinCore.Entities.DTOs;

public class ExpenseDto
{
    public record ExpenseCreateDto(int BudgetId, decimal Amount, string? Description);
    public record ExpenseUpdateDto(decimal Amount, string? Description, int BudgetId);
    public record ExpenseReadDto(int Id, int BudgetId, decimal Amount, string? Description, DateTime createdDate, ExpenseType ExpenseType = ExpenseType.Expense);
}