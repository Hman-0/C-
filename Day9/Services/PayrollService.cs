using Day9.Models;
using Day9.Repositories;

namespace Day9.Services
{
    public class PayrollService
    {
        private readonly IEmployeeRepository _repo;
        
        public PayrollService(IEmployeeRepository repo)
        {
            _repo = repo;
        }

        public decimal GetNetSalary(int employeeId)
        {
            var emp = _repo.GetById(employeeId);
            if (emp == null)
                throw new ArgumentException($"Employee with ID {employeeId} not found");
                
            var calc = new SalaryCalculator();
            return calc.CalculateNetSalary(emp);
        }
    }
}