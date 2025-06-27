using Day9.Models;
using Day9.Repositories;
using Day9.Services;
using Moq;
using Xunit;

namespace Day9.Tests
{
    public class PayrollServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepository;
        private readonly PayrollService _payrollService;

        public PayrollServiceTests()
        {
            _mockRepository = new Mock<IEmployeeRepository>();
            _payrollService = new PayrollService(_mockRepository.Object);
        }

        [Fact]
        public void GetNetSalary_ValidEmployeeId_ReturnsCorrectNetSalary()
        {
            // Arrange
            var employeeId = 1;
            var employee = new Employee
            {
                Id = employeeId,
                Name = "John Doe",
                BaseSalary = 5000m,
                Bonus = 1000m,
                Deduction = 500m
            };
            
            _mockRepository.Setup(repo => repo.GetById(employeeId))
                          .Returns(employee);

            // Act
            var result = _payrollService.GetNetSalary(employeeId);

            // Assert
            Assert.Equal(5500m, result);
            _mockRepository.Verify(repo => repo.GetById(employeeId), Times.Once);
        }

        [Fact]
        public void GetNetSalary_EmployeeWithZeroBonusAndDeduction_ReturnsBaseSalary()
        {
            // Arrange
            var employeeId = 2;
            var employee = new Employee
            {
                Id = employeeId,
                Name = "Jane Smith",
                BaseSalary = 4000m,
                Bonus = 0m,
                Deduction = 0m
            };
            
            _mockRepository.Setup(repo => repo.GetById(employeeId))
                          .Returns(employee);

            // Act
            var result = _payrollService.GetNetSalary(employeeId);

            // Assert
            Assert.Equal(4000m, result);
            _mockRepository.Verify(repo => repo.GetById(employeeId), Times.Once);
        }

        [Fact]
        public void GetNetSalary_RepositoryReturnsNull_ThrowsArgumentException()
        {
            // Arrange
            var employeeId = 999;
            _mockRepository.Setup(repo => repo.GetById(employeeId))
                          .Returns((Employee?)null);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _payrollService.GetNetSalary(employeeId));
            Assert.Contains("Employee with ID 999 not found", exception.Message);
            _mockRepository.Verify(repo => repo.GetById(employeeId), Times.Once);
        }

        [Fact]
        public void GetNetSalary_EmployeeWithNegativeBaseSalary_ThrowsArgumentException()
        {
            // Arrange
            var employeeId = 3;
            var employee = new Employee
            {
                Id = employeeId,
                Name = "Invalid Employee",
                BaseSalary = -1000m,
                Bonus = 500m,
                Deduction = 200m
            };
            
            _mockRepository.Setup(repo => repo.GetById(employeeId))
                          .Returns(employee);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _payrollService.GetNetSalary(employeeId));
            _mockRepository.Verify(repo => repo.GetById(employeeId), Times.Once);
        }

        [Theory]
        [InlineData(1, 3000, 500, 200, 3300)]
        [InlineData(2, 5000, 0, 1000, 4000)]
        [InlineData(3, 2000, 1500, 0, 3500)]
        public void GetNetSalary_VariousEmployees_ReturnsExpectedResults(int employeeId, decimal baseSalary, decimal bonus, decimal deduction, decimal expected)
        {
            // Arrange
            var employee = new Employee
            {
                Id = employeeId,
                Name = $"Employee {employeeId}",
                BaseSalary = baseSalary,
                Bonus = bonus,
                Deduction = deduction
            };
            
            _mockRepository.Setup(repo => repo.GetById(employeeId))
                          .Returns(employee);

            // Act
            var result = _payrollService.GetNetSalary(employeeId);

            // Assert
            Assert.Equal(expected, result);
            _mockRepository.Verify(repo => repo.GetById(employeeId), Times.Once);
        }

        [Fact]
        public void GetNetSalary_MultipleCallsWithSameId_CallsRepositoryMultipleTimes()
        {
            // Arrange
            var employeeId = 1;
            var employee = new Employee
            {
                Id = employeeId,
                Name = "John Doe",
                BaseSalary = 5000m,
                Bonus = 1000m,
                Deduction = 500m
            };
            
            _mockRepository.Setup(repo => repo.GetById(employeeId))
                          .Returns(employee);

            // Act
            _payrollService.GetNetSalary(employeeId);
            _payrollService.GetNetSalary(employeeId);

            // Assert
            _mockRepository.Verify(repo => repo.GetById(employeeId), Times.Exactly(2));
        }
    }
}