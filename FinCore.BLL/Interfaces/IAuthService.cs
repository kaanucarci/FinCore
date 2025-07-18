namespace FinCore.BLL.Interfaces;

public interface IAuthService
{
    Task<string> Authenticate(string username, string password);
}