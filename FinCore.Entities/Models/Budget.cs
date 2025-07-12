namespace FinCore.Entities.Models;

public class Budget : BaseEntity
{
    public int Year { get; set; }
    public int Month { get; set; }
    
    //Total amount will always stay the same
    public decimal TotalAmount { get; set; }
    
    //When the user adds a new expense or saving Amount will be updated
    public decimal Amount { get; set; }

    public virtual ICollection<Expense>? Expenses { get; set; }
    public virtual ICollection<Saving>? Savings { get; set; }
}