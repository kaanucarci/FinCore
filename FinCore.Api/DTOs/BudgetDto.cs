namespace FinCore.Api.DTOs;

public class BudgetDto
{
    public record BudgetCreateDto(int Month, int Year, decimal TotalAmount);
    public record BudgetReadDto(int Id, int Year, int Month, decimal TotalAmount, decimal Amount);
}