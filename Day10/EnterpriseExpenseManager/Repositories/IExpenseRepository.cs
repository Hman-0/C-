using EnterpriseExpenseManager.Data;

namespace EnterpriseExpenseManager.Repositories
{
    public interface IExpenseRepository
    {
        Task<List<Expense>> GetAllAsync();
        Task<Expense?> GetByIdAsync(int id);
        Task AddAsync(Expense expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(Expense expense);
        Task<List<Expense>> GetByFilterAsync(string? type, string? category, DateTime? from, DateTime? to);
        Task<decimal> GetTotalByTypeAsync(string type);
        Task<decimal> GetTotalByCategoryAsync(string category);
        Task<Dictionary<string, decimal>> GetTotalByMonthAsync(string type);
    }
} 