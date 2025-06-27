using Day8.Models;

namespace Day8.Services;

public interface IReportService
{
    Task<CashFlowReport> GetCashFlowReportAsync(int? month = null, int? year = null, int? departmentId = null);
    Task<IEnumerable<BudgetVarianceReport>> GetBudgetVarianceReportAsync();
    Task<IEnumerable<BudgetVarianceReport>> GetBudgetVarianceReportAsync(int departmentId);
    Task<byte[]> ExportCashFlowReportToExcelAsync(int? month = null, int? year = null, int? departmentId = null);
    Task<bool> CheckCashFlowWarningAsync(int departmentId);
}