namespace FinCore.Entities.Interfaces;

public interface IUserContext
{
    int UserId { get; }
    string UserName { get; }
    bool IsAuthenticated { get; }
}