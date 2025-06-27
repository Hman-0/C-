using EnterpriseExpenseManager.Data;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseExpenseManager.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly EnterpriseExpenseContext _context;
        public ExpenseRepository(EnterpriseExpenseContext context)
        {
            _context = context;
        }
        public async Task<List<Expense>> GetAllAsync() => await _context.Expenses.ToListAsync();
        public async Task<Expense?> GetByIdAsync(int id) => await _context.Expenses.FindAsync(id);
        public async Task AddAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Expense expense)
        {
            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Expense expense)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Expense>> GetByFilterAsync(string? type, string? category, DateTime? from, DateTime? to)
        {
            var query = _context.Expenses.AsQueryable();
            if (!string.IsNullOrWhiteSpace(type)) query = query.Where(x => x.Type == type);
            if (!string.IsNullOrWhiteSpace(category)) query = query.Where(x => x.Category == category);
            if (from.HasValue) query = query.Where(x => x.Date >= from);
            if (to.HasValue) query = query.Where(x => x.Date <= to);
            return await query.ToListAsync();
        }
        public async Task<decimal> GetTotalByTypeAsync(string type)
        {
            return await _context.Expenses.Where(x => x.Type == type).SumAsync(x => x.Amount);
        }
        public async Task<decimal> GetTotalByCategoryAsync(string category)
        {
            return await _context.Expenses.Where(x => x.Category == category).SumAsync(x => x.Amount);
        }
        public async Task<Dictionary<string, decimal>> GetTotalByMonthAsync(string type)
        {
            return await _context.Expenses.Where(x => x.Type == type)
                .GroupBy(x => x.Date.ToString("yyyy-MM"))
                .ToDictionaryAsync(g => g.Key, g => g.Sum(x => x.Amount));
        }
    }
} 