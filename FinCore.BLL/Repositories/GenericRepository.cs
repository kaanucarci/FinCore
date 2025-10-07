using FinCore.BLL.Interfaces;
using FinCore.DAL.Context;
using FinCore.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace FinCore.BLL.Repositories;

public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _ctx;
    public AppDbContext Context => _ctx;

    public GenericRepository(AppDbContext ctx) => _ctx = ctx;


    public async Task<List<T>> GetAllAsync() => await _ctx.Set<T>().ToListAsync();

    public async Task<T?> GetByIdAsync(int id) => await _ctx.Set<T>().FindAsync(id);
    

    public async Task AddAsync(T item, bool? saveImmediately = true) => await _ctx.Set<T>().AddAsync(item);

    public async Task AddRangeAsync(IEnumerable<T> items) => await _ctx.Set<T>().AddRangeAsync(items);


    public void Update(T item)
    {
        item.UpdatedDate = DateTime.Now;
        _ctx.Set<T>().Update(item);  
    } 

    public void Delete(T item) => _ctx.Set<T>().Remove(item);

    public void DeleteRange(IEnumerable<T> items) => _ctx.Set<T>().RemoveRange(items);
    
    public Task SaveChangesAsync() => _ctx.SaveChangesAsync();
    
    public IQueryable<T> Query() => _ctx.Set<T>().AsQueryable();

}