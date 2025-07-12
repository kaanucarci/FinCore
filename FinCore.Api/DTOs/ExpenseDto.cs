namespace FinCore.Api.DTOs;

public class ExpenseDto
{
    public record ExpenseCreateDto(int BudgetId, decimal Amount, string? Description);
    public record ExpenseReadDto(int Id, int BudgetId, decimal Amount, string? Description);
}