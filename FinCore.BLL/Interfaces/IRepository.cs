using FinCore.DAL.Context;
using FinCore.Entities.Models;

namespace FinCore.BLL.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T item, bool? saveImmediately = true);
    Task AddRangeAsync(IEnumerable<T> items);
    void Update(T item);
    void Delete(T item);
    void DeleteRange(IEnumerable<T> items);
    Task SaveChangesAsync();
    
    IQueryable<T> Query();
    AppDbContext Context { get; }
}   