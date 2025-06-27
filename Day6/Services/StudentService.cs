using Microsoft.EntityFrameworkCore;
using Day6.Data;
using Day6.Models;

namespace Day6.Services
{
    public class StudentService
    {
        private readonly AppDbContext _context;

        public StudentService(AppDbContext context)
        {
            _context = context;
        }

        // Thêm học viên mới
        public async Task<Student> AddStudentAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        // Lấy tất cả học viên
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .ToListAsync();
        }

        // Lấy học viên theo ID
        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // Cập nhật học viên
        public async Task<Student?> UpdateStudentAsync(int id, Student updatedStudent)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return null;

            student.FullName = updatedStudent.FullName;
            student.Email = updatedStudent.Email;
            student.BirthDate = updatedStudent.BirthDate;

            await _context.SaveChangesAsync();
            return student;
        }

        // Xóa học viên
        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        // Lấy danh sách học viên đang học một khóa học cụ thể
        public async Task<List<Student>> GetStudentsByCourseAsync(int courseId)
        {
            return await _context.Students
                .Where(s => s.Enrollments.Any(e => e.CourseId == courseId))
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .AsNoTracking()
                .ToListAsync();
        }

        // Lọc học viên theo tên
        public async Task<List<Student>> SearchStudentsByNameAsync(string name)
        {
            return await _context.Students
                .Where(s => s.FullName.Contains(name))
                .OrderBy(s => s.FullName)
                .AsNoTracking()
                .ToListAsync();
        }

        // Lọc học viên theo ngày sinh
        public async Task<List<Student>> GetStudentsByBirthDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Students
                .Where(s => s.BirthDate >= fromDate && s.BirthDate <= toDate)
                .OrderBy(s => s.FullName)
                .AsNoTracking()
                .ToListAsync();
        }

        // Sắp xếp học viên theo tên tăng dần
        public async Task<List<Student>> GetStudentsOrderedByNameAsync()
        {
            return await _context.Students
                .OrderBy(s => s.FullName)
                .AsNoTracking()
                .ToListAsync();
        }

        // Projection - chỉ lấy thông tin cần thiết
        public async Task<List<object>> GetStudentSummaryAsync()
        {
            return await _context.Students
                .Select(s => new
                {
                    s.Id,
                    s.FullName,
                    s.Email,
                    CourseCount = s.Enrollments.Count()
                })
                .AsNoTracking()
                .ToListAsync<object>();
        }
    }
}