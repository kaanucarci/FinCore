namespace FinCore.Entities.Models;

public class Budget : BaseEntity
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Amount { get; set; }

    public ICollection<Expense>? Expenses { get; set; }
    public ICollection<Saving>? Savings { get; set; }
}