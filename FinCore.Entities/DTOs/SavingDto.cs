namespace FinCore.Entities.DTOs;

public class SavingDto
{
    public record SavingCreateDto(int BudgetId, decimal Amount, string? Description);
    public record SavingUpdateDto(decimal Amount, string? Description, int BudgetId);
    public record SavingReadDto(int Id, int BudgetId, decimal Amount, string? Description);
}