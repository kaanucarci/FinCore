using FinCore.Entities.Models;

namespace FinCore.BLL.Interfaces;

public interface IUserService
{
    Task<User> LoginAsync(string userName, string password);
    Task<User?> GetByIdAsync(int Id);
}