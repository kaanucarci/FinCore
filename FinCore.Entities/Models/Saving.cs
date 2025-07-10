namespace FinCore.Entities.Models;

public class Saving : BaseEntity
{
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    
    public int BudgetId { get; set; }
    public virtual Budget Budget { get; set; }
}