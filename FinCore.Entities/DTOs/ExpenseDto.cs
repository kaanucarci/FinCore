namespace FinCore.Entities.DTOs;

public class ExpenseDto
{
    public record ExpenseCreateDto(int BudgetId, decimal Amount, string? Description, ExpenseType ExpenseType);
    public record ExpenseUpdateDto(decimal Amount, string? Description, int BudgetId, ExpenseType ExpenseType);
    public record ExpenseReadDto(int Id, int BudgetId, decimal Amount, string? Description, DateTime createdDate, ExpenseType ExpenseType);
}