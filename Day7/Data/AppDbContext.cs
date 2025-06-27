using Microsoft.EntityFrameworkCore;
using Day7.Models;

namespace Day7.Data;

public class AppDbContext : DbContext
{
    public DbSet<TodoItem> TodoItems { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("Server=localhost;Database=TodoAppDB;User=root;Password=;", 
            new MySqlServerVersion(new Version(8, 0, 21)));
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Deadline).IsRequired();
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt);
        });
    }
}