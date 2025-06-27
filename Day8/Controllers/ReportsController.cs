using Microsoft.AspNetCore.Mvc;
using Day8.Models;
using Day8.Services;

namespace Day8.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Lấy báo cáo dòng tiền theo tháng và phòng ban
    /// </summary>
    /// <param name="month">Tháng (1-12)</param>
    /// <param name="year">Năm</param>
    /// <param name="departmentId">ID phòng ban (tùy chọn)</param>
    /// <returns>Báo cáo dòng tiền</returns>
    [HttpGet("cashflow")]
    public async Task<ActionResult<CashFlowReport>> GetCashFlowReport(
        [FromQuery] int? month = null,
        [FromQuery] int? year = null,
        [FromQuery] int? departmentId = null)
    {
        var report = await _reportService.GetCashFlowReportAsync(month, year, departmentId);
        return Ok(report);
    }

    /// <summary>
    /// Lấy báo cáo phân tích ngân sách của tất cả phòng ban
    /// </summary>
    /// <returns>Danh sách báo cáo phân tích ngân sách</returns>
    [HttpGet("budget-variance")]
    public async Task<ActionResult<IEnumerable<BudgetVarianceReport>>> GetBudgetVarianceReport()
    {
        var reports = await _reportService.GetBudgetVarianceReportAsync();
        return Ok(reports);
    }

    /// <summary>
    /// Lấy báo cáo phân tích ngân sách của một phòng ban cụ thể
    /// </summary>
    /// <param name="departmentId">ID phòng ban</param>
    /// <returns>Báo cáo phân tích ngân sách</returns>
    [HttpGet("budget-variance/{departmentId}")]
    public async Task<ActionResult<IEnumerable<BudgetVarianceReport>>> GetBudgetVarianceReport(int departmentId)
    {
        var reports = await _reportService.GetBudgetVarianceReportAsync(departmentId);
        return Ok(reports);
    }

    /// <summary>
    /// Export báo cáo dòng tiền ra file Excel
    /// </summary>
    /// <param name="month">Tháng (1-12)</param>
    /// <param name="year">Năm</param>
    /// <param name="departmentId">ID phòng ban (tùy chọn)</param>
    /// <returns>File Excel</returns>
    [HttpGet("cashflow/export")]
    public async Task<IActionResult> ExportCashFlowReport(
        [FromQuery] int? month = null,
        [FromQuery] int? year = null,
        [FromQuery] int? departmentId = null)
    {
        var excelData = await _reportService.ExportCashFlowReportToExcelAsync(month, year, departmentId);
        
        var fileName = $"CashFlowReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    /// <summary>
    /// Kiểm tra cảnh báo dòng tiền âm liên tiếp
    /// </summary>
    /// <param name="departmentId">ID phòng ban</param>
    /// <returns>Kết quả kiểm tra cảnh báo</returns>
    [HttpGet("cashflow-warning/{departmentId}")]
    public async Task<ActionResult> CheckCashFlowWarning(int departmentId)
    {
        var hasWarning = await _reportService.CheckCashFlowWarningAsync(departmentId);
        
        return Ok(new 
        {
            DepartmentId = departmentId,
            HasWarning = hasWarning,
            Message = hasWarning ? "Cảnh báo: Dòng tiền âm trong 2 tháng liên tiếp" : "Dòng tiền ổn định"
        });
    }

    /// <summary>
    /// Lấy dữ liệu biểu đồ thu chi theo thời gian
    /// </summary>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <param name="departmentId">ID phòng ban (tùy chọn)</param>
    /// <returns>Dữ liệu biểu đồ</returns>
    [HttpGet("chart-data")]
    public async Task<ActionResult> GetChartData(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? departmentId = null)
    {
        var start = startDate ?? DateTime.Now.AddMonths(-6);
        var end = endDate ?? DateTime.Now;
        
        var chartData = new List<object>();
        var current = new DateTime(start.Year, start.Month, 1);
        
        while (current <= end)
        {
            var report = await _reportService.GetCashFlowReportAsync(current.Month, current.Year, departmentId);
            chartData.Add(new
            {
                Month = current.ToString("yyyy-MM"),
                Income = report.TotalIncome,
                Expense = report.TotalExpense,
                NetProfit = report.NetProfit
            });
            current = current.AddMonths(1);
        }
        
        return Ok(chartData);
    }
}