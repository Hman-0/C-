using Day9.Models;

namespace Day9.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly List<Employee> _employees;

        public EmployeeRepository()
        {
            // Sample data for demonstration
            _employees = new List<Employee>
            {
                new Employee { Id = 1, Name = "John Doe", BaseSalary = 5000m, Bonus = 1000m, Deduction = 500m },
                new Employee { Id = 2, Name = "Jane Smith", BaseSalary = 4000m, Bonus = 800m, Deduction = 300m },
                new Employee { Id = 3, Name = "Bob Johnson", BaseSalary = 6000m, Bonus = 1200m, Deduction = 700m }
            };
        }

        public Employee? GetById(int id)
        {
            return _employees.FirstOrDefault(e => e.Id == id);
        }
    }
}