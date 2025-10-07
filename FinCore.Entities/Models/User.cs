namespace FinCore.Entities.Models;

public class User: BaseEntity
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    
    public ICollection<Budget> Budgets { get; set; }
}