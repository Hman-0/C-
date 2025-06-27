using Day9.Models;
using Day9.Repositories;
using Day9.Services;

namespace Day9
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Hệ thống tính lương nhân viên ===");
            
            // Khởi tạo repository và service
            var repository = new EmployeeRepository();
            var payrollService = new PayrollService(repository);
            var calculator = new SalaryCalculator();
            
            // Demo tính lương cho các nhân viên
            for (int i = 1; i <= 3; i++)
            {
                try
                {
                    var netSalary = payrollService.GetNetSalary(i);
                    var employee = repository.GetById(i);
                    
                    Console.WriteLine($"\nNhân viên ID {i}: {employee?.Name}");
                    Console.WriteLine($"Lương cơ bản: {employee?.BaseSalary:C}");
                    Console.WriteLine($"Thưởng: {employee?.Bonus:C}");
                    Console.WriteLine($"Khấu trừ: {employee?.Deduction:C}");
                    Console.WriteLine($"Lương thực nhận: {netSalary:C}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi tính lương cho nhân viên ID {i}: {ex.Message}");
                }
            }
            
            // Demo test case với dữ liệu không hợp lệ
            Console.WriteLine("\n=== Test case với dữ liệu không hợp lệ ===");
            try
            {
                var invalidEmployee = new Employee
                {
                    Id = 999,
                    Name = "Invalid Employee",
                    BaseSalary = -1000m,
                    Bonus = 500m,
                    Deduction = 200m
                };
                
                calculator.CalculateNetSalary(invalidEmployee);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Đã bắt được lỗi như mong đợi: {ex.Message}");
            }
            
            Console.WriteLine("\nChạy 'dotnet test' để thực hiện unit tests!");
        }
    }
}
