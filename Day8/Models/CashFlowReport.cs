namespace Day8.Models;

public class CashFlowReport
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetProfit => TotalIncome - TotalExpense;
    public decimal BudgetVariance { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class BudgetVarianceReport
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public decimal BudgetLimit { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Variance => BudgetLimit - TotalExpense;
    public decimal VariancePercentage => BudgetLimit > 0 ? (Variance / BudgetLimit) * 100 : 0;
    public bool IsOverBudget => Variance < 0;
}