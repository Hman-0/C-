using Microsoft.EntityFrameworkCore;

namespace EnterpriseExpenseManager.Data
{
    public class EnterpriseExpenseContext : DbContext
    {
        public EnterpriseExpenseContext(DbContextOptions<EnterpriseExpenseContext> options) : base(options) { }
        public DbSet<Expense> Expenses { get; set; }
    }

    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
    }
} 