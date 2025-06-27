using Day9.Models;

namespace Day9.Services
{
    public class SalaryCalculator
    {
        public decimal CalculateNetSalary(Employee emp)
        {
            if (emp == null) throw new ArgumentNullException(nameof(emp));
            if (emp.BaseSalary < 0 || emp.Deduction < 0) throw new ArgumentException("Invalid amount");
            
            return emp.BaseSalary + emp.Bonus - emp.Deduction;
        }
    }
}