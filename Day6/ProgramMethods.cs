using Day6.Data;
using Day6.Models;
using Day6.Services;
using Microsoft.EntityFrameworkCore;

namespace Day6
{
    public partial class Program
    {
        // Thêm dữ liệu mẫu
        static async Task SeedDataAsync()
        {
            var studentCount = await _context.Students.CountAsync();
            if (studentCount == 0)
            {
                Console.WriteLine("Đang thêm dữ liệu mẫu...");

                // Thêm học viên mẫu
                var students = new List<Student>
                {
                    new Student { FullName = "Nguyễn Văn An", Email = "an.nguyen@email.com", BirthDate = new DateTime(1995, 5, 15) },
                    new Student { FullName = "Trần Thị Bình", Email = "binh.tran@email.com", BirthDate = new DateTime(1996, 8, 22) },
                    new Student { FullName = "Lê Văn Cường", Email = "cuong.le@email.com", BirthDate = new DateTime(1994, 12, 10) },
                    new Student { FullName = "Phạm Thị Dung", Email = "dung.pham@email.com", BirthDate = new DateTime(1997, 3, 8) },
                    new Student { FullName = "Hoàng Văn Em", Email = "em.hoang@email.com", BirthDate = new DateTime(1995, 11, 25) },
                    new Student { FullName = "Vũ Thị Phương", Email = "phuong.vu@email.com", BirthDate = new DateTime(1996, 7, 14) }
                };

                await _context.Students.AddRangeAsync(students);

                // Thêm khóa học mẫu
                var courses = new List<Course>
                {
                    new Course { Title = "Lập trình C# cơ bản", Level = "Beginner", Duration = 40 },
                    new Course { Title = "ASP.NET Core MVC", Level = "Intermediate", Duration = 60 },
                    new Course { Title = "Entity Framework Core", Level = "Intermediate", Duration = 30 },
                    new Course { Title = "JavaScript và React", Level = "Intermediate", Duration = 50 },
                    new Course { Title = "SQL Server và Database Design", Level = "Beginner", Duration = 35 },
                    new Course { Title = "Clean Architecture", Level = "Advanced", Duration = 25 }
                };

                await _context.Courses.AddRangeAsync(courses);
                await _context.SaveChangesAsync();

                // Thêm đăng ký mẫu
                var enrollments = new List<Enrollment>
                {
                    new Enrollment { StudentId = 1, CourseId = 1, EnrollDate = DateTime.Now.AddDays(-30) },
                    new Enrollment { StudentId = 1, CourseId = 2, EnrollDate = DateTime.Now.AddDays(-25) },
                    new Enrollment { StudentId = 2, CourseId = 1, EnrollDate = DateTime.Now.AddDays(-28) },
                    new Enrollment { StudentId = 2, CourseId = 3, EnrollDate = DateTime.Now.AddDays(-20) },
                    new Enrollment { StudentId = 3, CourseId = 1, EnrollDate = DateTime.Now.AddDays(-26) },
                    new Enrollment { StudentId = 3, CourseId = 4, EnrollDate = DateTime.Now.AddDays(-15) },
                    new Enrollment { StudentId = 4, CourseId = 2, EnrollDate = DateTime.Now.AddDays(-22) },
                    new Enrollment { StudentId = 5, CourseId = 1, EnrollDate = DateTime.Now.AddDays(-18) },
                    new Enrollment { StudentId = 6, CourseId = 5, EnrollDate = DateTime.Now.AddDays(-12) }
                };

                await _context.Enrollments.AddRangeAsync(enrollments);
                await _context.SaveChangesAsync();

                Console.WriteLine("✅ Đã thêm dữ liệu mẫu thành công!");
            }
        }

        // === STUDENT METHODS ===
        static async Task AddStudentAsync()
        {
            Console.WriteLine("\n=== THÊM HỌC VIÊN MỚI ===");
            
            Console.Write("Nhập họ tên: ");
            var fullName = Console.ReadLine();
            
            Console.Write("Nhập email: ");
            var email = Console.ReadLine();
            
            Console.Write("Nhập ngày sinh (dd/MM/yyyy): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var birthDate))
            {
                Console.WriteLine("❌ Định dạng ngày không hợp lệ!");
                return;
            }

            var student = new Student
            {
                FullName = fullName ?? "",
                Email = email ?? "",
                BirthDate = birthDate
            };

            try
            {
                await _studentService.AddStudentAsync(student);
                Console.WriteLine($"✅ Đã thêm học viên '{fullName}' thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }

        static async Task ShowAllStudentsAsync()
        {
            Console.WriteLine("\n=== DANH SÁCH HỌC VIÊN ===");
            var students = await _studentService.GetAllStudentsAsync();
            
            if (!students.Any())
            {
                Console.WriteLine("Không có học viên nào.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Họ tên",-25} {"Email",-30} {"Ngày sinh",-12} {"Số khóa học",-12}");
            Console.WriteLine(new string('-', 85));
            
            foreach (var student in students)
            {
                Console.WriteLine($"{student.Id,-5} {student.FullName,-25} {student.Email,-30} {student.BirthDate:dd/MM/yyyy,-12} {student.Enrollments.Count,-12}");
            }
        }

        static async Task FindStudentByIdAsync()
        {
            Console.Write("Nhập ID học viên: ");
            if (!int.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("❌ ID không hợp lệ!");
                return;
            }

            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                Console.WriteLine("❌ Không tìm thấy học viên!");
                return;
            }

            Console.WriteLine($"\n=== THÔNG TIN HỌC VIÊN ===");
            Console.WriteLine($"ID: {student.Id}");
            Console.WriteLine($"Họ tên: {student.FullName}");
            Console.WriteLine($"Email: {student.Email}");
            Console.WriteLine($"Ngày sinh: {student.BirthDate:dd/MM/yyyy}");
            Console.WriteLine($"Số khóa học đã đăng ký: {student.Enrollments.Count}");
            
            if (student.Enrollments.Any())
            {
                Console.WriteLine("\nCác khóa học đã đăng ký:");
                foreach (var enrollment in student.Enrollments)
                {
                    Console.WriteLine($"- {enrollment.Course.Title} (Đăng ký: {enrollment.EnrollDate:dd/MM/yyyy})");
                }
            }
        }

        static async Task SearchStudentsByNameAsync()
        {
            Console.Write("Nhập tên cần tìm: ");
            var name = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("❌ Vui lòng nhập tên!");
                return;
            }

            var students = await _studentService.SearchStudentsByNameAsync(name);
            
            if (!students.Any())
            {
                Console.WriteLine("❌ Không tìm thấy học viên nào!");
                return;
            }

            Console.WriteLine($"\n=== KẾT QUẢ TÌM KIẾM ({students.Count} học viên) ===");
            Console.WriteLine($"{"ID",-5} {"Họ tên",-25} {"Email",-30} {"Ngày sinh",-12}");
            Console.WriteLine(new string('-', 75));
            
            foreach (var student in students)
            {
                Console.WriteLine($"{student.Id,-5} {student.FullName,-25} {student.Email,-30} {student.BirthDate:dd/MM/yyyy,-12}");
            }
        }

        static async Task UpdateStudentAsync()
        {
            Console.Write("Nhập ID học viên cần cập nhật: ");
            if (!int.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("❌ ID không hợp lệ!");
                return;
            }

            var existingStudent = await _studentService.GetStudentByIdAsync(id);
            if (existingStudent == null)
            {
                Console.WriteLine("❌ Không tìm thấy học viên!");
                return;
            }

            Console.WriteLine($"Thông tin hiện tại: {existingStudent.FullName} - {existingStudent.Email}");
            
            Console.Write($"Nhập họ tên mới (hiện tại: {existingStudent.FullName}): ");
            var fullName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fullName)) fullName = existingStudent.FullName;
            
            Console.Write($"Nhập email mới (hiện tại: {existingStudent.Email}): ");
            var email = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(email)) email = existingStudent.Email;
            
            Console.Write($"Nhập ngày sinh mới (hiện tại: {existingStudent.BirthDate:dd/MM/yyyy}, định dạng dd/MM/yyyy): ");
            var birthDateStr = Console.ReadLine();
            var birthDate = existingStudent.BirthDate;
            if (!string.IsNullOrWhiteSpace(birthDateStr))
            {
                if (!DateTime.TryParseExact(birthDateStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out birthDate))
                {
                    Console.WriteLine("❌ Định dạng ngày không hợp lệ!");
                    return;
                }
            }

            var updatedStudent = new Student
            {
                FullName = fullName,
                Email = email,
                BirthDate = birthDate
            };

            try
            {
                await _studentService.UpdateStudentAsync(id, updatedStudent);
                Console.WriteLine("✅ Đã cập nhật học viên thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }

        static async Task DeleteStudentAsync()
        {
            Console.Write("Nhập ID học viên cần xóa: ");
            if (!int.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("❌ ID không hợp lệ!");
                return;
            }

            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                Console.WriteLine("❌ Không tìm thấy học viên!");
                return;
            }

            Console.WriteLine($"Bạn có chắc chắn muốn xóa học viên '{student.FullName}'? (y/N): ");
            var confirm = Console.ReadLine();
            
            if (confirm?.ToLower() != "y")
            {
                Console.WriteLine("Đã hủy thao tác xóa.");
                return;
            }

            try
            {
                await _studentService.DeleteStudentAsync(id);
                Console.WriteLine("✅ Đã xóa học viên thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }
    }
}