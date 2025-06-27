using Day9.Models;
using Day9.Services;
using Xunit;
using Xunit.Abstractions;

namespace Day9.Tests
{
    public class DebuggingExerciseTests
    {
        private readonly ITestOutputHelper _output;

        public DebuggingExerciseTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DebuggingExercise_InjectBugAndDebug()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 1,
                Name = "John Doe",
                BaseSalary = 5000m,
                Bonus = 1000m,
                Deduction = 500m
            };

            // Tạo một calculator với bug cố ý
            var buggyCalculator = new BuggySalaryCalculator();
            
            // Act
            var result = buggyCalculator.CalculateNetSalary(employee);
            
            // Debug output
            _output.WriteLine($"Expected: {5000m + 1000m - 500m} = {5500m}");
            _output.WriteLine($"Actual: {result}");
            
            // Uncomment dòng dưới để test fail và debug
            // Assert.Equal(5500m, result);
            
            // Sau khi debug và fix bug, uncomment dòng dưới để test pass
            Assert.Equal(5500m, buggyCalculator.CalculateNetSalaryFixed(employee));
        }
    }

    // Class này cố tình có bug để demo debugging
    public class BuggySalaryCalculator
    {
        public decimal CalculateNetSalary(Employee emp)
        {
            if (emp == null) throw new ArgumentNullException(nameof(emp));
            if (emp.BaseSalary < 0 || emp.Deduction < 0) throw new ArgumentException("Invalid amount");
            
            // BUG: Cộng thay vì trừ deduction
            return emp.BaseSalary + emp.Bonus + emp.Deduction;
        }   
        
        public decimal CalculateNetSalaryFixed(Employee emp)
        {
            if (emp == null) throw new ArgumentNullException(nameof(emp));
            if (emp.BaseSalary < 0 || emp.Deduction < 0) throw new ArgumentException("Invalid amount");
            
            // Fixed: Trừ deduction đúng cách
            return emp.BaseSalary + emp.Bonus - emp.Deduction;
        }
    }
}