namespace FinCore.Entities.Models;

public class UserResetCode: BaseEntity
{
    public string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int UserId { get; set; }
    public bool IsUsed { get; set; } = false;

    public virtual User User { get; set; }
}