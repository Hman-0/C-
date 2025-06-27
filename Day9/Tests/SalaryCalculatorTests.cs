using Day9.Models;
using Day9.Services;
using Xunit;

namespace Day9.Tests
{
    public class SalaryCalculatorTests
    {
        private readonly SalaryCalculator _calculator;

        public SalaryCalculatorTests()
        {
            _calculator = new SalaryCalculator();
        }

        [Fact]
        public void CalculateNetSalary_ValidEmployee_ReturnsCorrectAmount()
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

            // Act
            var result = _calculator.CalculateNetSalary(employee);

            // Assert
            Assert.Equal(5500m, result);
        }

        [Fact]
        public void CalculateNetSalary_ZeroBonusAndDeduction_ReturnsBaseSalary()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 2,
                Name = "Jane Smith",
                BaseSalary = 4000m,
                Bonus = 0m,
                Deduction = 0m
            };

            // Act
            var result = _calculator.CalculateNetSalary(employee);

            // Assert
            Assert.Equal(4000m, result);
        }

        [Fact]
        public void CalculateNetSalary_NullEmployee_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _calculator.CalculateNetSalary(null!));
        }

        [Fact]
        public void CalculateNetSalary_NegativeBaseSalary_ThrowsArgumentException()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 3,
                Name = "Invalid Employee",
                BaseSalary = -1000m,
                Bonus = 500m,
                Deduction = 200m
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _calculator.CalculateNetSalary(employee));
        }

        [Fact]
        public void CalculateNetSalary_NegativeDeduction_ThrowsArgumentException()
        {
            // Arrange
            var employee = new Employee
            {
                Id = 4,
                Name = "Invalid Employee 2",
                BaseSalary = 5000m,
                Bonus = 500m,
                Deduction = -200m
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _calculator.CalculateNetSalary(employee));
        }

        [Theory]
        [InlineData(3000, 500, 200, 3300)]
        [InlineData(5000, 0, 1000, 4000)]
        [InlineData(2000, 1500, 0, 3500)]
        [InlineData(10000, 2000, 1500, 10500)]
        public void CalculateNetSalary_VariousInputs_ReturnsExpectedResults(decimal baseSalary, decimal bonus, decimal deduction, decimal expected)
        {
            // Arrange
            var employee = new Employee
            {
                Id = 5,
                Name = "Test Employee",
                BaseSalary = baseSalary,
                Bonus = bonus,
                Deduction = deduction
            };

            // Act
            var result = _calculator.CalculateNetSalary(employee);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}