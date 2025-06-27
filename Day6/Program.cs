using Day6.Data;
using Day6.Models;
using Day6.Services;
using Microsoft.EntityFrameworkCore;

namespace Day6
{
    public partial class Program
    {
        private static AppDbContext _context = null!;
        private static StudentService _studentService = null!;
        private static CourseService _courseService = null!;
        private static EnrollmentService _enrollmentService = null!;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== HỆ THỐNG QUẢN LÝ HỌC VIÊN VÀ KHÓA HỌC ===");
            Console.WriteLine();

            // Khởi tạo DbContext và Services
            _context = new AppDbContext();
            _studentService = new StudentService(_context);
            _courseService = new CourseService(_context);
            _enrollmentService = new EnrollmentService(_context);

            try
            {
                // Đảm bảo database được tạo
                await _context.Database.EnsureCreatedAsync();
                Console.WriteLine("✅ Kết nối database thành công!");
                Console.WriteLine();

                // Thêm dữ liệu mẫu nếu chưa có
                await SeedDataAsync();

                // Hiển thị menu chính
                await ShowMainMenuAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
                Console.WriteLine("Vui lòng kiểm tra kết nối MySQL và thử lại.");
            }
            finally
            {
                await _context.DisposeAsync();
            }
        }

        static async Task ShowMainMenuAsync()
        {
            while (true)
            {
                Console.WriteLine("\n=== MENU CHÍNH ===");
                Console.WriteLine("1. Quản lý Học viên");
                Console.WriteLine("2. Quản lý Khóa học");
                Console.WriteLine("3. Quản lý Đăng ký");
                Console.WriteLine("4. Báo cáo và Thống kê");
                Console.WriteLine("5. Thoát");
                Console.Write("Chọn chức năng (1-5): ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await ShowStudentMenuAsync();
                        break;
                    case "2":
                        await ShowCourseMenuAsync();
                        break;
                    case "3":
                        await ShowEnrollmentMenuAsync();
                        break;
                    case "4":
                        await ShowReportsMenuAsync();
                        break;
                    case "5":
                        Console.WriteLine("Cảm ơn bạn đã sử dụng hệ thống!");
                        return;
                    default:
                        Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                        break;
                }
            }
        }
    }
}
