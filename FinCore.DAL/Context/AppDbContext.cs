using FinCore.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace FinCore.DAL.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User>    Users    => Set<User>();
    public DbSet<Budget>  Budgets  => Set<Budget>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Saving>  Savings  => Set<Saving>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Budget)
            .WithMany(b => b.Expenses)
            .HasForeignKey(e => e.BudgetId);
        
        modelBuilder.Entity<Saving>()
            .HasOne(s => s.Budget)
            .WithMany(b => b.Savings)
            .HasForeignKey(s => s.BudgetId);
        
    }
}