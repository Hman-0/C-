using EnterpriseExpenseManager.DTOs;

namespace EnterpriseExpenseManager.Services
{
    public interface IExpenseService
    {
        Task<List<ExpenseDto>> GetAllAsync();
        Task<ExpenseDto?> GetByIdAsync(int id);
        Task AddAsync(CreateExpenseDto dto);
        Task UpdateAsync(int id, UpdateExpenseDto dto);
        Task DeleteAsync(int id);
        Task<List<ExpenseDto>> GetByFilterAsync(string? type, string? category, DateTime? from, DateTime? to);
        Task<decimal> GetTotalByTypeAsync(string type);
        Task<decimal> GetTotalByCategoryAsync(string category);
        Task<Dictionary<string, decimal>> GetTotalByMonthAsync(string type);
        Task<decimal> GetProfitAsync();
        Task<decimal> GetCashFlowAsync(DateTime from, DateTime to);
        Task<decimal> GetBudgetAsync();
    }
} 