using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using StudentManager.Data;
using StudentManager.Models;
using StudentManager.Services;
using System.Globalization;

namespace StudentManager
{
    class Program
    {
        private static StudentService _studentService = null!;
        private static ExportService _exportService = null!;
        
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("🎓 HỆ THỐNG QUẢN LÝ SINH VIÊN");
            Console.WriteLine("================================\n");
            
            // Initialize database and services
            await InitializeAsync();
            
            // Main menu loop
            while (true)
            {
                ShowMainMenu();
                var choice = Console.ReadLine();
                
                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ShowAllStudentsAsync();
                            break;
                        case "2":
                            await AddStudentAsync();
                            break;
                        case "3":
                            await UpdateStudentAsync();
                            break;
                        case "4":
                            await DeleteStudentAsync();
                            break;
                        case "5":
                            await SearchStudentsAsync();
                            break;
                        case "6":
                            await ShowStudentsByClassAsync();
                            break;
                        case "7":
                            await ShowStatisticsAsync();
                            break;
                        case "8":
                            await ExportDataAsync();
                            break;
                        case "0":
                            Console.WriteLine("\n👋 Cảm ơn bạn đã sử dụng hệ thống!");
                            return;
                        default:
                            Console.WriteLine("\n❌ Lựa chọn không hợp lệ! Vui lòng thử lại.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n❌ Lỗi: {ex.Message}");
                }
                
                Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
                Console.Clear();
            }
        }
        
        static async Task InitializeAsync()
        {
            try
            {
                using var context = new StudentContext();
                await context.Database.EnsureCreatedAsync();
                
                _studentService = new StudentService(context);
                _exportService = new ExportService();
                
                Console.WriteLine("✅ Khởi tạo cơ sở dữ liệu thành công!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khởi tạo: {ex.Message}");
                Environment.Exit(1);
            }
        }
        
        static void ShowMainMenu()
        {
            Console.WriteLine("\n📋 MENU CHÍNH");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("1. 📖 Xem danh sách sinh viên");
            Console.WriteLine("2. ➕ Thêm sinh viên mới");
            Console.WriteLine("3. ✏️  Cập nhật thông tin sinh viên");
            Console.WriteLine("4. 🗑️  Xóa sinh viên");
            Console.WriteLine("5. 🔍 Tìm kiếm sinh viên");
            Console.WriteLine("6. 🏫 Xem sinh viên theo lớp");
            Console.WriteLine("7. 📊 Thống kê");
            Console.WriteLine("8. 📄 Xuất dữ liệu (PDF/CSV)");
            Console.WriteLine("0. 🚪 Thoát");
            Console.WriteLine("═══════════════════════════════════════");
            Console.Write("👉 Nhập lựa chọn của bạn: ");
        }
        
        static async Task ShowAllStudentsAsync()
        {
            Console.WriteLine("\n📖 DANH SÁCH TẤT CẢ SINH VIÊN");
            Console.WriteLine("═══════════════════════════════════════\n");
            
            using var context = new StudentContext();
            var service = new StudentService(context);
            var students = await service.GetAllStudentsAsync();
            
            if (!students.Any())
            {
                Console.WriteLine("📭 Chưa có sinh viên nào trong hệ thống.");
                return;
            }
            
            DisplayStudentsTable(students);
        }
        
        static async Task AddStudentAsync()
        {
            Console.WriteLine("\n➕ THÊM SINH VIÊN MỚI");
            Console.WriteLine("═══════════════════════════════════════\n");
            
            var student = new Student();
            
            Console.Write("Họ và tên: ");
            student.FullName = Console.ReadLine()?.Trim() ?? "";
            
            Console.Write("Mã sinh viên: ");
            student.StudentCode = Console.ReadLine()?.Trim() ?? "";
            
            Console.Write("Email: ");
            student.Email = Console.ReadLine()?.Trim() ?? "";
            
            Console.Write("Số điện thoại (tùy chọn): ");
            var phone = Console.ReadLine()?.Trim();
            student.PhoneNumber = string.IsNullOrEmpty(phone) ? null : phone;
            
            Console.Write("Lớp: ");
            student.Class = Console.ReadLine()?.Trim() ?? "";
            
            Console.Write("GPA (0-10): ");
            if (double.TryParse(Console.ReadLine(), out double gpa))
            {
                student.GPA = gpa;
            }
            
            Console.Write("Ngày sinh (dd/MM/yyyy): ");
            if (DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dob))
            {
                student.DateOfBirth = dob;
            }
            
            using var context = new StudentContext();
            var service = new StudentService(context);
            
            if (await service.AddStudentAsync(student))
            {
                Console.WriteLine("\n✅ Thêm sinh viên thành công!");
            }
            else
            {
                Console.WriteLine("\n❌ Thêm sinh viên thất bại!");
            }
        }
        
        static async Task UpdateStudentAsync()
        {
            Console.WriteLine("\n✏️ CẬP NHẬT THÔNG TIN SINH VIÊN");
            Console.WriteLine("═══════════════════════════════════════\n");
            
            Console.Write("Nhập ID sinh viên cần cập nhật: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ ID không hợp lệ!");
                return;
            }
            
            using var context = new StudentContext();
            var service = new StudentService(context);
            var student = await service.GetStudentByIdAsync(id);
            
            if (student == null)
            {
                Console.WriteLine("❌ Không tìm thấy sinh viên!");
                return;
            }
            
            Console.WriteLine($"\nThông tin hiện tại của sinh viên {student.FullName}:");
            DisplayStudentsTable(new List<Student> { student });
            
            Console.WriteLine("\nNhập thông tin mới (để trống nếu không muốn thay đổi):\n");
            
            Console.Write($"Họ và tên [{student.FullName}]: ");
            var fullName = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(fullName)) student.FullName = fullName;
            
            Console.Write($"Mã sinh viên [{student.StudentCode}]: ");
            var studentCode = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(studentCode)) student.StudentCode = studentCode;
            
            Console.Write($"Email [{student.Email}]: ");
            var email = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(email)) student.Email = email;
            
            Console.Write($"Số điện thoại [{student.PhoneNumber}]: ");
            var phone = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(phone)) student.PhoneNumber = phone;
            
            Console.Write($"Lớp [{student.Class}]: ");
            var className = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(className)) student.Class = className;
            
            Console.Write($"GPA [{student.GPA:F2}]: ");
            var gpaInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(gpaInput) && double.TryParse(gpaInput, out double gpa))
            {
                student.GPA = gpa;
            }
            
            Console.Write($"Ngày sinh [{student.DateOfBirth:dd/MM/yyyy}] (dd/MM/yyyy): ");
            var dobInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(dobInput) && DateTime.TryParseExact(dobInput, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dob))
            {
                student.DateOfBirth = dob;
            }
            
            if (await service.UpdateStudentAsync(student))
            {
                Console.WriteLine("\n✅ Cập nhật thông tin thành công!");
            }
            else
            {
                Console.WriteLine("\n❌ Cập nhật thông tin thất bại!");
            }
        }
        
        static async Task DeleteStudentAsync()
        {
            Console.WriteLine("\n🗑️ XÓA SINH VIÊN");
            Console.WriteLine("═══════════════════════════════════════\n");
            
            Console.Write("Nhập ID sinh viên cần xóa: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("❌ ID không hợp lệ!");
                return;
            }
            
            using var context = new StudentContext();
            var service = new StudentService(context);
            var student = await service.GetStudentByIdAsync(id);
            
            if (student == null)
            {
                Console.WriteLine("❌ Không tìm thấy sinh viên!");
                return;
            }
            
            Console.WriteLine($"\nThông tin sinh viên sẽ bị xóa:");
            DisplayStudentsTable(new List<Student> { student });
            
            Console.Write("\n⚠️ Bạn có chắc chắn muốn xóa sinh viên này? (y/N): ");
            var confirm = Console.ReadLine()?.Trim().ToLower();
            
            if (confirm == "y" || confirm == "yes")
            {
                if (await service.DeleteStudentAsync(id))
                {
                    Console.WriteLine("\n✅ Xóa sinh viên thành công!");
                }
                else
                {
                    Console.WriteLine("\n❌ Xóa sinh viên thất bại!");
                }
            }
            else
            {
                Console.WriteLine("\n🚫 Đã hủy thao tác xóa.");
            }
        }
        
        static async Task SearchStudentsAsync()
        {
            Console.WriteLine("\n🔍 TÌM KIẾM SINH VIÊN");
            Console.WriteLine("═══════════════════════════════════════\n");
            
            Console.Write("Nhập từ khóa tìm kiếm (tên, mã SV, lớp): ");
            var searchTerm = Console.ReadLine()?.Trim();
            
            if (string.IsNullOrEmpty(searchTerm))
            {
                Console.WriteLine("❌ Vui lòng nhập từ khóa tìm kiếm!");
                return;
            }
            
            using var context = new StudentContext();
            var service = new StudentService(context);
            var students = await service.SearchStudentsAsync(searchTerm);
            
            if (!students.Any())
            {
                Console.WriteLine($"📭 Không tìm thấy sinh viên nào với từ khóa '{searchTerm}'.");
                return;
            }
            
            Console.WriteLine($"\n🎯 Tìm thấy {students.Count} sinh viên:");
            DisplayStudentsTable(students);
        }
        
        static async Task ShowStudentsByClassAsync()
        {
            Console.WriteLine("\n🏫 XEM SINH VIÊN THEO LỚP");
            Console.WriteLine("═══════════════════════════════════════\n");
            
            using var context = new StudentContext();
            var service = new StudentService(context);
            var classes = await service.GetAllClassesAsync();
            
            if (!classes.Any())
            {
                Console.WriteLine("📭 Chưa có lớp nào trong hệ thống.");
                return;
            }
            
            Console.WriteLine("Danh sách các lớp:");
            for (int i = 0; i < classes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {classes[i]}");
            }
            
            Console.Write("\nChọn lớp (nhập số thứ tự): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > classes.Count)
            {
                Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                return;
            }
            
            var selectedClass = classes[choice - 1];
            var students = await service.GetStudentsByClassAsync(selectedClass);
            
            Console.WriteLine($"\n👥 Sinh viên lớp {selectedClass} ({students.Count} sinh viên):");
            DisplayStudentsTable(students);
        }
        
        static async Task ShowStatisticsAsync()
        {
            Console.WriteLine("\n📊 THỐNG KÊ");
            Console.WriteLine("═══════════════════════════════════════\n");
            
            using var context = new StudentContext();
            var service = new StudentService(context);
            
            var allStudents = await service.GetAllStudentsAsync();
            var clasStats = await service.GetStudentStatsByClassAsync();
            var averageGPA = await service.GetAverageGPAAsync();
            
            Console.WriteLine($"📈 Tổng số sinh viên: {allStudents.Count}");
            Console.WriteLine($"📊 GPA trung bình: {averageGPA:F2}");
            Console.WriteLine($"🏫 Số lớp: {clasStats.Count}\n");
            
            if (clasStats.Any())
            {
                Console.WriteLine("📋 Thống kê theo lớp:");
                var table = new ConsoleTable("Lớp", "Số sinh viên", "GPA trung bình");
                
                foreach (var stat in clasStats.OrderBy(x => x.Key))
                {
                    var classStudents = allStudents.Where(s => s.Class == stat.Key).ToList();
                    var classAvgGPA = classStudents.Any() ? classStudents.Average(s => s.GPA) : 0;
                    table.AddRow(stat.Key, stat.Value, classAvgGPA.ToString("F2"));
                }
                
                table.Write();
            }
            
            // GPA distribution
            Console.WriteLine("\n🎯 Phân bố GPA:");
            var gpaRanges = new Dictionary<string, int>
            {
                ["Xuất sắc (8.5-10)"] = allStudents.Count(s => s.GPA >= 8.5),
                ["Giỏi (7.0-8.4)"] = allStudents.Count(s => s.GPA >= 7.0 && s.GPA < 8.5),
                ["Khá (5.5-6.9)"] = allStudents.Count(s => s.GPA >= 5.5 && s.GPA < 7.0),
                ["Trung bình (4.0-5.4)"] = allStudents.Count(s => s.GPA >= 4.0 && s.GPA < 5.5),
                ["Yếu (<4.0)"] = allStudents.Count(s => s.GPA < 4.0)
            };
            
            var gpaTable = new ConsoleTable("Xếp loại", "Số sinh viên", "Tỷ lệ (%)");
            foreach (var range in gpaRanges)
            {
                var percentage = allStudents.Count > 0 ? (range.Value * 100.0 / allStudents.Count) : 0;
                gpaTable.AddRow(range.Key, range.Value, percentage.ToString("F1"));
            }
            gpaTable.Write();
        }
        
        static async Task ExportDataAsync()
        {
            Console.WriteLine("\n📄 XUẤT DỮ LIỆU");
            Console.WriteLine("═══════════════════════════════════════\n");
            
            using var context = new StudentContext();
            var service = new StudentService(context);
            var students = await service.GetAllStudentsAsync();
            
            if (!students.Any())
            {
                Console.WriteLine("📭 Không có dữ liệu để xuất.");
                return;
            }
            
            Console.WriteLine("Chọn định dạng xuất:");
            Console.WriteLine("1. 📄 PDF");
            Console.WriteLine("2. 📊 CSV");
            Console.WriteLine("3. 📋 Cả hai");
            Console.Write("\nLựa chọn: ");
            
            var choice = Console.ReadLine();
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var exportDir = "Exports";
            
            if (!Directory.Exists(exportDir))
            {
                Directory.CreateDirectory(exportDir);
            }
            
            switch (choice)
            {
                case "1":
                    var pdfPath = Path.Combine(exportDir, $"DanhSachSinhVien_{timestamp}.pdf");
                    await _exportService.ExportToPdfAsync(students, pdfPath);
                    break;
                    
                case "2":
                    var csvPath = Path.Combine(exportDir, $"DanhSachSinhVien_{timestamp}.csv");
                    await _exportService.ExportToCsvAsync(students, csvPath);
                    break;
                    
                case "3":
                    var pdfPath2 = Path.Combine(exportDir, $"DanhSachSinhVien_{timestamp}.pdf");
                    var csvPath2 = Path.Combine(exportDir, $"DanhSachSinhVien_{timestamp}.csv");
                    await _exportService.ExportToPdfAsync(students, pdfPath2);
                    await _exportService.ExportToCsvAsync(students, csvPath2);
                    break;
                    
                default:
                    Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                    break;
            }
        }
        
        static void DisplayStudentsTable(List<Student> students)
        {
            if (!students.Any())
            {
                Console.WriteLine("📭 Không có dữ liệu để hiển thị.");
                return;
            }
            
            var table = new ConsoleTable("ID", "Họ và Tên", "Mã SV", "Email", "Lớp", "GPA", "Ngày sinh");
            
            foreach (var student in students)
            {
                table.AddRow(
                    student.Id,
                    student.FullName,
                    student.StudentCode,
                    student.Email,
                    student.Class,
                    student.GPA.ToString("F2"),
                    student.DateOfBirth.ToString("dd/MM/yyyy")
                );
            }
            
            table.Write();
            Console.WriteLine($"\n📊 Tổng cộng: {students.Count} sinh viên");
        }
    }
}
