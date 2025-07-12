namespace FinCore.Api.DTOs;

public class BudgetDto
{
    public record BudgetCreateDto(int Month, decimal TotalAmount);
    public record BudgetReadDto(int Id, int Year, int Month, decimal TotalAmount, decimal Amount);
}