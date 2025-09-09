namespace FinCore.Entities.DTOs;

public class SavingDto
{
    public record SavingCreateDto(int BudgetId, decimal Amount, string? Description);
    public record SavingReadDto(int Id, int BudgetId, decimal Amount, string? Description);
}