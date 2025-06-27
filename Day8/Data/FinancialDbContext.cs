using Microsoft.EntityFrameworkCore;
using Day8.Models;

namespace Day8.Data;

public class FinancialDbContext : DbContext
{
    public FinancialDbContext(DbContextOptions<FinancialDbContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Department> Departments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Transaction
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Amount).HasPrecision(18, 2);
            entity.Property(t => t.Date).IsRequired();
            entity.Property(t => t.Category).HasMaxLength(100).IsRequired();
            entity.Property(t => t.Description).HasMaxLength(500);
            
            entity.HasOne(t => t.Department)
                  .WithMany(d => d.Transactions)
                  .HasForeignKey(t => t.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Department
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).HasMaxLength(100).IsRequired();
            entity.Property(d => d.BudgetLimit).HasPrecision(18, 2);
        });

        // Seed data
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "IT", BudgetLimit = 50000 },
            new Department { Id = 2, Name = "Marketing", BudgetLimit = 30000 },
            new Department { Id = 3, Name = "HR", BudgetLimit = 25000 },
            new Department { Id = 4, Name = "Finance", BudgetLimit = 40000 }
        );
    }
}