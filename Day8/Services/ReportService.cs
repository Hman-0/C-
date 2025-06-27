using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Day8.Data;
using Day8.Models;

namespace Day8.Services;

public class ReportService : IReportService
{
    private readonly FinancialDbContext _context;

    public ReportService(FinancialDbContext context)
    {
        _context = context;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<CashFlowReport> GetCashFlowReportAsync(int? month = null, int? year = null, int? departmentId = null)
    {
        var currentDate = DateTime.Now;
        var targetMonth = month ?? currentDate.Month;
        var targetYear = year ?? currentDate.Year;
        
        var startDate = new DateTime(targetYear, targetMonth, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var query = _context.Transactions
            .Where(t => t.Date >= startDate && t.Date <= endDate);

        if (departmentId.HasValue)
        {
            query = query.Where(t => t.DepartmentId == departmentId.Value);
        }

        var transactions = await query.ToListAsync();
        
        var totalIncome = transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);
            
        var totalExpense = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        decimal budgetVariance = 0;
        string? departmentName = null;

        if (departmentId.HasValue)
        {
            var department = await _context.Departments.FindAsync(departmentId.Value);
            if (department != null)
            {
                budgetVariance = department.BudgetLimit - totalExpense;
                departmentName = department.Name;
            }
        }
        else
        {
            var totalBudgetLimit = await _context.Departments.SumAsync(d => d.BudgetLimit);
            budgetVariance = totalBudgetLimit - totalExpense;
        }

        return new CashFlowReport
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            BudgetVariance = budgetVariance,
            DepartmentId = departmentId,
            DepartmentName = departmentName,
            StartDate = startDate,
            EndDate = endDate
        };
    }

    public async Task<IEnumerable<BudgetVarianceReport>> GetBudgetVarianceReportAsync()
    {
        var currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

        var departments = await _context.Departments.ToListAsync();
        var reports = new List<BudgetVarianceReport>();

        foreach (var department in departments)
        {
            var totalExpense = await _context.Transactions
                .Where(t => t.DepartmentId == department.Id && 
                           t.Type == TransactionType.Expense &&
                           t.Date >= currentMonthStart && 
                           t.Date <= currentMonthEnd)
                .SumAsync(t => t.Amount);

            reports.Add(new BudgetVarianceReport
            {
                DepartmentId = department.Id,
                DepartmentName = department.Name,
                BudgetLimit = department.BudgetLimit,
                TotalExpense = totalExpense
            });
        }

        return reports;
    }

    public async Task<IEnumerable<BudgetVarianceReport>> GetBudgetVarianceReportAsync(int departmentId)
    {
        var reports = await GetBudgetVarianceReportAsync();
        return reports.Where(r => r.DepartmentId == departmentId);
    }

    public async Task<byte[]> ExportCashFlowReportToExcelAsync(int? month = null, int? year = null, int? departmentId = null)
    {
        var report = await GetCashFlowReportAsync(month, year, departmentId);
        
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Cash Flow Report");

        // Headers
        worksheet.Cells[1, 1].Value = "Cash Flow Report";
        worksheet.Cells[1, 1].Style.Font.Bold = true;
        worksheet.Cells[1, 1].Style.Font.Size = 16;

        worksheet.Cells[3, 1].Value = "Period:";
        worksheet.Cells[3, 2].Value = $"{report.StartDate:yyyy-MM-dd} to {report.EndDate:yyyy-MM-dd}";
        
        if (!string.IsNullOrEmpty(report.DepartmentName))
        {
            worksheet.Cells[4, 1].Value = "Department:";
            worksheet.Cells[4, 2].Value = report.DepartmentName;
        }

        // Data
        worksheet.Cells[6, 1].Value = "Total Income:";
        worksheet.Cells[6, 2].Value = report.TotalIncome;
        worksheet.Cells[6, 2].Style.Numberformat.Format = "#,##0.00";

        worksheet.Cells[7, 1].Value = "Total Expense:";
        worksheet.Cells[7, 2].Value = report.TotalExpense;
        worksheet.Cells[7, 2].Style.Numberformat.Format = "#,##0.00";

        worksheet.Cells[8, 1].Value = "Net Profit:";
        worksheet.Cells[8, 2].Value = report.NetProfit;
        worksheet.Cells[8, 2].Style.Numberformat.Format = "#,##0.00";
        worksheet.Cells[8, 1].Style.Font.Bold = true;
        worksheet.Cells[8, 2].Style.Font.Bold = true;

        worksheet.Cells[9, 1].Value = "Budget Variance:";
        worksheet.Cells[9, 2].Value = report.BudgetVariance;
        worksheet.Cells[9, 2].Style.Numberformat.Format = "#,##0.00";

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();

        return package.GetAsByteArray();
    }

    public async Task<bool> CheckCashFlowWarningAsync(int departmentId)
    {
        var currentDate = DateTime.Now;
        var twoMonthsAgo = currentDate.AddMonths(-2);
        var lastMonth = currentDate.AddMonths(-1);

        // Check last month
        var lastMonthStart = new DateTime(lastMonth.Year, lastMonth.Month, 1);
        var lastMonthEnd = lastMonthStart.AddMonths(1).AddDays(-1);
        var lastMonthReport = await GetCashFlowReportAsync(lastMonth.Month, lastMonth.Year, departmentId);

        // Check two months ago
        var twoMonthsAgoStart = new DateTime(twoMonthsAgo.Year, twoMonthsAgo.Month, 1);
        var twoMonthsAgoEnd = twoMonthsAgoStart.AddMonths(1).AddDays(-1);
        var twoMonthsAgoReport = await GetCashFlowReportAsync(twoMonthsAgo.Month, twoMonthsAgo.Year, departmentId);

        return lastMonthReport.NetProfit < 0 && twoMonthsAgoReport.NetProfit < 0;
    }
}