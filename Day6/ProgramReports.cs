using Day6.Data;
using Day6.Models;
using Day6.Services;
using Microsoft.EntityFrameworkCore;

namespace Day6
{
    public partial class Program
    {
        // === REPORT METHODS ===
        static async Task ShowCoursesWithManyStudentsAsync()
        {
            Console.WriteLine("\n=== KHÓA HỌC CÓ NHIỀU HƠN 5 HỌC VIÊN ===");
            var courses = await _courseService.GetCoursesWithMoreThanFiveStudentsAsync();
            
            if (!courses.Any())
            {
                Console.WriteLine("Không có khóa học nào có nhiều hơn 5 học viên.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Tiêu đề",-30} {"Cấp độ",-15} {"Số học viên",-12}");
            Console.WriteLine(new string('-', 65));
            
            foreach (var course in courses)
            {
                Console.WriteLine($"{course.Id,-5} {course.Title,-30} {course.Level,-15} {course.Enrollments.Count,-12}");
            }
        }

        static async Task ShowMostPopularCourseAsync()
        {
            Console.WriteLine("\n=== KHÓA HỌC ĐƯỢC ĐĂNG KÝ NHIỀU NHẤT ===");
            var course = await _courseService.GetMostPopularCourseAsync();
            
            if (course == null)
            {
                Console.WriteLine("Không có dữ liệu khóa học.");
                return;
            }

            Console.WriteLine($"Khóa học: {course.Title}");
            Console.WriteLine($"Cấp độ: {course.Level}");
            Console.WriteLine($"Thời lượng: {course.Duration} giờ");
            Console.WriteLine($"Số học viên đã đăng ký: {course.Enrollments.Count}");
            
            if (course.Enrollments.Any())
            {
                Console.WriteLine("\nDanh sách học viên:");
                foreach (var enrollment in course.Enrollments.Take(10)) // Hiển thị tối đa 10 học viên
                {
                    Console.WriteLine($"- {enrollment.Student.FullName} (Đăng ký: {enrollment.EnrollDate:dd/MM/yyyy})");
                }
                
                if (course.Enrollments.Count > 10)
                {
                    Console.WriteLine($"... và {course.Enrollments.Count - 10} học viên khác");
                }
            }
        }

        static async Task ShowStudentStatisticsAsync()
        {
            Console.WriteLine("\n=== THỐNG KÊ HỌC VIÊN ===");
            
            var studentSummary = await _studentService.GetStudentSummaryAsync();
            
            if (!studentSummary.Any())
            {
                Console.WriteLine("Không có dữ liệu học viên.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Họ tên",-25} {"Email",-30} {"Số khóa học",-12}");
            Console.WriteLine(new string('-', 75));
            
            foreach (dynamic student in studentSummary)
            {
                Console.WriteLine($"{student.Id,-5} {student.FullName,-25} {student.Email,-30} {student.CourseCount,-12}");
            }
            
            // Thống kê tổng quan
            var totalStudents = studentSummary.Count;
            var totalEnrollments = studentSummary.Sum(s => (int)((dynamic)s).CourseCount);
            var avgCoursesPerStudent = totalStudents > 0 ? (double)totalEnrollments / totalStudents : 0;
            
            Console.WriteLine("\n=== TỔNG QUAN ===");
            Console.WriteLine($"Tổng số học viên: {totalStudents}");
            Console.WriteLine($"Tổng số đăng ký: {totalEnrollments}");
            Console.WriteLine($"Trung bình khóa học/học viên: {avgCoursesPerStudent:F2}");
        }

        static async Task ShowCourseStatisticsAsync()
        {
            Console.WriteLine("\n=== THỐNG KÊ KHÓA HỌC ===");
            
            var courseStats = await _courseService.GetCourseStatisticsAsync();
            
            if (!courseStats.Any())
            {
                Console.WriteLine("Không có dữ liệu khóa học.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Tiêu đề",-25} {"Cấp độ",-12} {"Thời lượng",-10} {"Học viên",-10} {"Đăng ký gần nhất",-18}");
            Console.WriteLine(new string('-', 85));
            
            foreach (dynamic course in courseStats)
            {
                var latestEnrollment = course.LatestEnrollment != null ? 
                    ((DateTime)course.LatestEnrollment).ToString("dd/MM/yyyy") : "Chưa có";
                    
                Console.WriteLine($"{course.Id,-5} {course.Title,-25} {course.Level,-12} {course.Duration + "h",-10} {course.StudentCount,-10} {latestEnrollment,-18}");
            }
            
            // Thống kê theo cấp độ
            var levelStats = courseStats.GroupBy(c => ((dynamic)c).Level)
                .Select(g => new { Level = g.Key, Count = g.Count(), TotalStudents = g.Sum(c => (int)((dynamic)c).StudentCount) })
                .ToList();
            
            Console.WriteLine("\n=== THỐNG KÊ THEO CẤP ĐỘ ===");
            Console.WriteLine($"{"Cấp độ",-15} {"Số khóa học",-12} {"Tổng học viên",-15}");
            Console.WriteLine(new string('-', 45));
            
            foreach (var stat in levelStats)
            {
                Console.WriteLine($"{stat.Level,-15} {stat.Count,-12} {stat.TotalStudents,-15}");
            }
        }

        static async Task ShowEnrollmentStatisticsAsync()
        {
            Console.WriteLine("\n=== THỐNG KÊ ĐĂNG KÝ THEO THÁNG ===");
            
            Console.Write("Nhập năm cần thống kê (ví dụ: 2024): ");
            if (!int.TryParse(Console.ReadLine(), out var year))
            {
                year = DateTime.Now.Year;
                Console.WriteLine($"Sử dụng năm mặc định: {year}");
            }
            
            var monthlyStats = await _enrollmentService.GetEnrollmentStatisticsByMonthAsync(year);
            
            if (!monthlyStats.Any())
            {
                Console.WriteLine($"Không có dữ liệu đăng ký cho năm {year}.");
                return;
            }

            Console.WriteLine($"\n=== THỐNG KÊ NĂM {year} ===");
            Console.WriteLine($"{"Tháng",-8} {"Số đăng ký",-12} {"Học viên mới",-15}");
            Console.WriteLine(new string('-', 40));
            
            var monthNames = new string[] { "", "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6",
                                          "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" };
            
            foreach (dynamic stat in monthlyStats)
            {
                var monthName = monthNames[stat.Month];
                Console.WriteLine($"{monthName,-8} {stat.Count,-12} {stat.Students,-15}");
            }
            
            // Tổng kết
            var totalEnrollments = monthlyStats.Sum(s => (int)((dynamic)s).Count);
            var totalUniqueStudents = monthlyStats.Sum(s => (int)((dynamic)s).Students);
            
            Console.WriteLine("\n=== TỔNG KẾT ===");
            Console.WriteLine($"Tổng đăng ký trong năm: {totalEnrollments}");
            Console.WriteLine($"Tổng học viên tham gia: {totalUniqueStudents}");
            
            // Tháng có nhiều đăng ký nhất
            var peakMonth = monthlyStats.OrderByDescending(s => ((dynamic)s).Count).FirstOrDefault();
            if (peakMonth != null)
            {
                var peakMonthName = monthNames[((dynamic)peakMonth).Month];
                Console.WriteLine($"Tháng có nhiều đăng ký nhất: {peakMonthName} ({((dynamic)peakMonth).Count} đăng ký)");
            }
        }

        // === ADDITIONAL UTILITY METHODS ===
        static async Task ShowAdvancedReportsAsync()
        {
            Console.WriteLine("\n=== BÁO CÁO NÂNG CAO ===");
            
            // Học viên đăng ký nhiều khóa học nhất
            var students = await _studentService.GetAllStudentsAsync();
            var topStudent = students.OrderByDescending(s => s.Enrollments.Count).FirstOrDefault();
            
            if (topStudent != null)
            {
                Console.WriteLine($"Học viên tích cực nhất: {topStudent.FullName} ({topStudent.Enrollments.Count} khóa học)");
            }
            
            // Khóa học theo thời lượng
            var courses = await _courseService.GetAllCoursesAsync();
            var avgDuration = courses.Any() ? courses.Average(c => c.Duration) : 0;
            var shortCourses = courses.Count(c => c.Duration < avgDuration);
            var longCourses = courses.Count(c => c.Duration >= avgDuration);
            
            Console.WriteLine($"\nPhân bố khóa học theo thời lượng:");
            Console.WriteLine($"- Thời lượng trung bình: {avgDuration:F1} giờ");
            Console.WriteLine($"- Khóa học ngắn (< {avgDuration:F1}h): {shortCourses}");
            Console.WriteLine($"- Khóa học dài (>= {avgDuration:F1}h): {longCourses}");
            
            // Tỷ lệ đăng ký theo cấp độ
            var enrollmentsByLevel = await _context.Enrollments
                .Include(e => e.Course)
                .GroupBy(e => e.Course.Level)
                .Select(g => new { Level = g.Key, Count = g.Count() })
                .ToListAsync();
            
            if (enrollmentsByLevel.Any())
            {
                Console.WriteLine($"\nTỷ lệ đăng ký theo cấp độ:");
                var totalEnrollments = enrollmentsByLevel.Sum(e => e.Count);
                foreach (var item in enrollmentsByLevel)
                {
                    var percentage = (double)item.Count / totalEnrollments * 100;
                    Console.WriteLine($"- {item.Level}: {item.Count} đăng ký ({percentage:F1}%)");
                }
            }
        }

        static async Task ShowReportsMenuAsync()
        {
            while (true)
            {
                Console.WriteLine("\n=== BÁO CÁO VÀ THỐNG KÊ ===");
                Console.WriteLine("1. Khóa học có nhiều hơn 5 học viên");
                Console.WriteLine("2. Khóa học được đăng ký nhiều nhất");
                Console.WriteLine("3. Thống kê học viên");
                Console.WriteLine("4. Thống kê khóa học");
                Console.WriteLine("5. Thống kê đăng ký theo tháng");
                Console.WriteLine("6. Quay lại menu chính");
                Console.Write("Chọn chức năng (1-6): ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await ShowCoursesWithManyStudentsAsync();
                        break;
                    case "2":
                        await ShowMostPopularCourseAsync();
                        break;
                    case "3":
                        await ShowStudentStatisticsAsync();
                        break;
                    case "4":
                        await ShowCourseStatisticsAsync();
                        break;
                    case "5":
                        await ShowEnrollmentStatisticsAsync();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                        break;
                }
            }
        }
    }
}