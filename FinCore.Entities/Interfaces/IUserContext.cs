namespace FinCore.Entities.Interfaces;

public interface IUserContext
{
    int UserId { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
}