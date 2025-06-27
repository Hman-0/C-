using Day6.Data;
using Day6.Models;
using Day6.Services;
using Microsoft.EntityFrameworkCore;

namespace Day6
{
    public partial class Program
    {
        // === COURSE METHODS ===
        static async Task AddCourseAsync()
        {
            Console.WriteLine("\n=== THÊM KHÓA HỌC MỚI ===");
            
            Console.Write("Nhập tiêu đề khóa học: ");
            var title = Console.ReadLine();
            
            Console.Write("Nhập cấp độ (Beginner/Intermediate/Advanced): ");
            var level = Console.ReadLine();
            
            Console.Write("Nhập thời lượng (giờ): ");
            if (!int.TryParse(Console.ReadLine(), out var duration))
            {
                Console.WriteLine("❌ Thời lượng không hợp lệ!");
                return;
            }

            var course = new Course
            {
                Title = title ?? "",
                Level = level ?? "",
                Duration = duration
            };

            try
            {
                await _courseService.AddCourseAsync(course);
                Console.WriteLine($"✅ Đã thêm khóa học '{title}' thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }

        static async Task ShowAllCoursesAsync()
        {
            Console.WriteLine("\n=== DANH SÁCH KHÓA HỌC ===");
            var courses = await _courseService.GetAllCoursesAsync();
            
            if (!courses.Any())
            {
                Console.WriteLine("Không có khóa học nào.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Tiêu đề",-30} {"Cấp độ",-15} {"Thời lượng",-12} {"Số học viên",-12}");
            Console.WriteLine(new string('-', 75));
            
            foreach (var course in courses)
            {
                Console.WriteLine($"{course.Id,-5} {course.Title,-30} {course.Level,-15} {course.Duration + " giờ",-12} {course.Enrollments.Count,-12}");
            }
        }

        static async Task FindCourseByIdAsync()
        {
            Console.Write("Nhập ID khóa học: ");
            if (!int.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("❌ ID không hợp lệ!");
                return;
            }

            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                Console.WriteLine("❌ Không tìm thấy khóa học!");
                return;
            }

            Console.WriteLine($"\n=== THÔNG TIN KHÓA HỌC ===");
            Console.WriteLine($"ID: {course.Id}");
            Console.WriteLine($"Tiêu đề: {course.Title}");
            Console.WriteLine($"Cấp độ: {course.Level}");
            Console.WriteLine($"Thời lượng: {course.Duration} giờ");
            Console.WriteLine($"Số học viên đã đăng ký: {course.Enrollments.Count}");
            
            if (course.Enrollments.Any())
            {
                Console.WriteLine("\nDanh sách học viên:");
                foreach (var enrollment in course.Enrollments)
                {
                    Console.WriteLine($"- {enrollment.Student.FullName} (Đăng ký: {enrollment.EnrollDate:dd/MM/yyyy})");
                }
            }
        }

        static async Task SearchCoursesByTitleAsync()
        {
            Console.Write("Nhập tiêu đề cần tìm: ");
            var title = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("❌ Vui lòng nhập tiêu đề!");
                return;
            }

            var courses = await _courseService.SearchCoursesByTitleAsync(title);
            
            if (!courses.Any())
            {
                Console.WriteLine("❌ Không tìm thấy khóa học nào!");
                return;
            }

            Console.WriteLine($"\n=== KẾT QUẢ TÌM KIẾM ({courses.Count} khóa học) ===");
            Console.WriteLine($"{"ID",-5} {"Tiêu đề",-30} {"Cấp độ",-15} {"Thời lượng",-12}");
            Console.WriteLine(new string('-', 65));
            
            foreach (var course in courses)
            {
                Console.WriteLine($"{course.Id,-5} {course.Title,-30} {course.Level,-15} {course.Duration + " giờ",-12}");
            }
        }

        static async Task UpdateCourseAsync()
        {
            Console.Write("Nhập ID khóa học cần cập nhật: ");
            if (!int.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("❌ ID không hợp lệ!");
                return;
            }

            var existingCourse = await _courseService.GetCourseByIdAsync(id);
            if (existingCourse == null)
            {
                Console.WriteLine("❌ Không tìm thấy khóa học!");
                return;
            }

            Console.WriteLine($"Thông tin hiện tại: {existingCourse.Title} - {existingCourse.Level} - {existingCourse.Duration} giờ");
            
            Console.Write($"Nhập tiêu đề mới (hiện tại: {existingCourse.Title}): ");
            var title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title)) title = existingCourse.Title;
            
            Console.Write($"Nhập cấp độ mới (hiện tại: {existingCourse.Level}): ");
            var level = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(level)) level = existingCourse.Level;
            
            Console.Write($"Nhập thời lượng mới (hiện tại: {existingCourse.Duration} giờ): ");
            var durationStr = Console.ReadLine();
            var duration = existingCourse.Duration;
            if (!string.IsNullOrWhiteSpace(durationStr))
            {
                if (!int.TryParse(durationStr, out duration))
                {
                    Console.WriteLine("❌ Thời lượng không hợp lệ!");
                    return;
                }
            }

            var updatedCourse = new Course
            {
                Title = title,
                Level = level,
                Duration = duration
            };

            try
            {
                await _courseService.UpdateCourseAsync(id, updatedCourse);
                Console.WriteLine("✅ Đã cập nhật khóa học thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }

        static async Task DeleteCourseAsync()
        {
            Console.Write("Nhập ID khóa học cần xóa: ");
            if (!int.TryParse(Console.ReadLine(), out var id))
            {
                Console.WriteLine("❌ ID không hợp lệ!");
                return;
            }

            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                Console.WriteLine("❌ Không tìm thấy khóa học!");
                return;
            }

            Console.WriteLine($"Bạn có chắc chắn muốn xóa khóa học '{course.Title}'? (y/N): ");
            var confirm = Console.ReadLine();
            
            if (confirm?.ToLower() != "y")
            {
                Console.WriteLine("Đã hủy thao tác xóa.");
                return;
            }

            try
            {
                await _courseService.DeleteCourseAsync(id);
                Console.WriteLine("✅ Đã xóa khóa học thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }

        // === ENROLLMENT METHODS ===
        static async Task EnrollStudentAsync()
        {
            Console.WriteLine("\n=== ĐĂNG KÝ KHÓA HỌC ===");
            
            // Hiển thị danh sách học viên
            var students = await _studentService.GetAllStudentsAsync();
            if (!students.Any())
            {
                Console.WriteLine("❌ Không có học viên nào trong hệ thống!");
                return;
            }
            
            Console.WriteLine("Danh sách học viên:");
            foreach (var s in students)
            {
                Console.WriteLine($"{s.Id}. {s.FullName}");
            }
            
            Console.Write("Chọn ID học viên: ");
            if (!int.TryParse(Console.ReadLine(), out var studentId))
            {
                Console.WriteLine("❌ ID học viên không hợp lệ!");
                return;
            }
            
            // Hiển thị danh sách khóa học
            var courses = await _courseService.GetAllCoursesAsync();
            if (!courses.Any())
            {
                Console.WriteLine("❌ Không có khóa học nào trong hệ thống!");
                return;
            }
            
            Console.WriteLine("\nDanh sách khóa học:");
            foreach (var c in courses)
            {
                Console.WriteLine($"{c.Id}. {c.Title} ({c.Level} - {c.Duration} giờ)");
            }
            
            Console.Write("Chọn ID khóa học: ");
            if (!int.TryParse(Console.ReadLine(), out var courseId))
            {
                Console.WriteLine("❌ ID khóa học không hợp lệ!");
                return;
            }

            try
            {
                var enrollment = await _enrollmentService.EnrollStudentAsync(studentId, courseId);
                if (enrollment != null)
                {
                    Console.WriteLine("✅ Đăng ký khóa học thành công!");
                }
                else
                {
                    Console.WriteLine("❌ Không thể đăng ký! Có thể học viên đã đăng ký khóa học này rồi.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }

        static async Task UnenrollStudentAsync()
        {
            Console.WriteLine("\n=== HỦY ĐĂNG KÝ KHÓA HỌC ===");
            
            Console.Write("Nhập ID học viên: ");
            if (!int.TryParse(Console.ReadLine(), out var studentId))
            {
                Console.WriteLine("❌ ID học viên không hợp lệ!");
                return;
            }
            
            Console.Write("Nhập ID khóa học: ");
            if (!int.TryParse(Console.ReadLine(), out var courseId))
            {
                Console.WriteLine("❌ ID khóa học không hợp lệ!");
                return;
            }

            try
            {
                var success = await _enrollmentService.UnenrollStudentAsync(studentId, courseId);
                if (success)
                {
                    Console.WriteLine("✅ Hủy đăng ký thành công!");
                }
                else
                {
                    Console.WriteLine("❌ Không tìm thấy đăng ký để hủy!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi: {ex.Message}");
            }
        }

        static async Task ShowAllEnrollmentsAsync()
        {
            Console.WriteLine("\n=== DANH SÁCH TẤT CẢ ĐĂNG KÝ ===");
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            
            if (!enrollments.Any())
            {
                Console.WriteLine("Không có đăng ký nào.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Học viên",-25} {"Khóa học",-30} {"Ngày đăng ký",-15}");
            Console.WriteLine(new string('-', 80));
            
            foreach (var enrollment in enrollments)
            {
                Console.WriteLine($"{enrollment.Id,-5} {enrollment.Student.FullName,-25} {enrollment.Course.Title,-30} {enrollment.EnrollDate:dd/MM/yyyy,-15}");
            }
        }

        static async Task ShowEnrollmentsByStudentAsync()
        {
            Console.Write("Nhập ID học viên: ");
            if (!int.TryParse(Console.ReadLine(), out var studentId))
            {
                Console.WriteLine("❌ ID không hợp lệ!");
                return;
            }

            var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(studentId);
            
            if (!enrollments.Any())
            {
                Console.WriteLine("❌ Học viên này chưa đăng ký khóa học nào!");
                return;
            }

            Console.WriteLine($"\n=== KHÓA HỌC CỦA HỌC VIÊN ({enrollments.Count} khóa học) ===");
            Console.WriteLine($"{"ID",-5} {"Khóa học",-30} {"Cấp độ",-15} {"Ngày đăng ký",-15}");
            Console.WriteLine(new string('-', 70));
            
            foreach (var enrollment in enrollments)
            {
                Console.WriteLine($"{enrollment.Id,-5} {enrollment.Course.Title,-30} {enrollment.Course.Level,-15} {enrollment.EnrollDate:dd/MM/yyyy,-15}");
            }
        }

        static async Task ShowEnrollmentsByCourseAsync()
        {
            Console.Write("Nhập ID khóa học: ");
            if (!int.TryParse(Console.ReadLine(), out var courseId))
            {
                Console.WriteLine("❌ ID không hợp lệ!");
                return;
            }

            var enrollments = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId);
            
            if (!enrollments.Any())
            {
                Console.WriteLine("❌ Khóa học này chưa có học viên nào đăng ký!");
                return;
            }

            Console.WriteLine($"\n=== HỌC VIÊN CỦA KHÓA HỌC ({enrollments.Count} học viên) ===");
            Console.WriteLine($"{"ID",-5} {"Học viên",-25} {"Email",-30} {"Ngày đăng ký",-15}");
            Console.WriteLine(new string('-', 80));
            
            foreach (var enrollment in enrollments)
            {
                Console.WriteLine($"{enrollment.Id,-5} {enrollment.Student.FullName,-25} {enrollment.Student.Email,-30} {enrollment.EnrollDate:dd/MM/yyyy,-15}");
            }
        }

        // === MENU METHODS ===
        static async Task ShowStudentMenuAsync()
        {
            while (true)
            {
                Console.WriteLine("\n=== QUẢN LÝ HỌC VIÊN ===");
                Console.WriteLine("1. Thêm học viên");
                Console.WriteLine("2. Xem danh sách học viên");
                Console.WriteLine("3. Tìm học viên theo ID");
                Console.WriteLine("4. Tìm học viên theo tên");
                Console.WriteLine("5. Cập nhật học viên");
                Console.WriteLine("6. Xóa học viên");
                Console.WriteLine("7. Quay lại menu chính");
                Console.Write("Chọn chức năng (1-7): ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await AddStudentAsync();
                        break;
                    case "2":
                        await ShowAllStudentsAsync();
                        break;
                    case "3":
                        await FindStudentByIdAsync();
                        break;
                    case "4":
                        await SearchStudentsByNameAsync();
                        break;
                    case "5":
                        await UpdateStudentAsync();
                        break;
                    case "6":
                        await DeleteStudentAsync();
                        break;
                    case "7":
                        return;
                    default:
                        Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                        break;
                }
            }
        }

        static async Task ShowCourseMenuAsync()
        {
            while (true)
            {
                Console.WriteLine("\n=== QUẢN LÝ KHÓA HỌC ===");
                Console.WriteLine("1. Thêm khóa học");
                Console.WriteLine("2. Xem danh sách khóa học");
                Console.WriteLine("3. Tìm khóa học theo ID");
                Console.WriteLine("4. Tìm khóa học theo tiêu đề");
                Console.WriteLine("5. Cập nhật khóa học");
                Console.WriteLine("6. Xóa khóa học");
                Console.WriteLine("7. Quay lại menu chính");
                Console.Write("Chọn chức năng (1-7): ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await AddCourseAsync();
                        break;
                    case "2":
                        await ShowAllCoursesAsync();
                        break;
                    case "3":
                        await FindCourseByIdAsync();
                        break;
                    case "4":
                        await SearchCoursesByTitleAsync();
                        break;
                    case "5":
                        await UpdateCourseAsync();
                        break;
                    case "6":
                        await DeleteCourseAsync();
                        break;
                    case "7":
                        return;
                    default:
                        Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                        break;
                }
            }
        }

        static async Task ShowEnrollmentMenuAsync()
        {
            while (true)
            {
                Console.WriteLine("\n=== QUẢN LÝ ĐĂNG KÝ ===");
                Console.WriteLine("1. Đăng ký khóa học cho học viên");
                Console.WriteLine("2. Hủy đăng ký");
                Console.WriteLine("3. Xem tất cả đăng ký");
                Console.WriteLine("4. Xem đăng ký theo học viên");
                Console.WriteLine("5. Xem đăng ký theo khóa học");
                Console.WriteLine("6. Quay lại menu chính");
                Console.Write("Chọn chức năng (1-6): ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        await EnrollStudentAsync();
                        break;
                    case "2":
                        await UnenrollStudentAsync();
                        break;
                    case "3":
                        await ShowAllEnrollmentsAsync();
                        break;
                    case "4":
                        await ShowEnrollmentsByStudentAsync();
                        break;
                    case "5":
                        await ShowEnrollmentsByCourseAsync();
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