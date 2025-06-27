using Microsoft.EntityFrameworkCore;
using Day6.Data;
using Day6.Models;

namespace Day6.Services
{
    public class EnrollmentService
    {
        private readonly AppDbContext _context;

        public EnrollmentService(AppDbContext context)
        {
            _context = context;
        }

        // Đăng ký khóa học cho học viên
        public async Task<Enrollment?> EnrollStudentAsync(int studentId, int courseId)
        {
            // Kiểm tra học viên và khóa học có tồn tại không
            var student = await _context.Students.FindAsync(studentId);
            var course = await _context.Courses.FindAsync(courseId);

            if (student == null || course == null)
                return null;

            // Kiểm tra xem học viên đã đăng ký khóa học này chưa
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (existingEnrollment != null)
                return null; // Đã đăng ký rồi

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollDate = DateTime.Now
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        // Hủy đăng ký khóa học
        public async Task<bool> UnenrollStudentAsync(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
                return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        // Lấy tất cả đăng ký
        public async Task<List<Enrollment>> GetAllEnrollmentsAsync()
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .OrderByDescending(e => e.EnrollDate)
                .ToListAsync();
        }

        // Lấy đăng ký theo ID
        public async Task<Enrollment?> GetEnrollmentByIdAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // Lấy danh sách đăng ký của một học viên
        public async Task<List<Enrollment>> GetEnrollmentsByStudentAsync(int studentId)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                .OrderByDescending(e => e.EnrollDate)
                .AsNoTracking()
                .ToListAsync();
        }

        // Lấy danh sách đăng ký của một khóa học
        public async Task<List<Enrollment>> GetEnrollmentsByCourseAsync(int courseId)
        {
            return await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .OrderByDescending(e => e.EnrollDate)
                .AsNoTracking()
                .ToListAsync();
        }

        // Lấy đăng ký theo khoảng thời gian
        public async Task<List<Enrollment>> GetEnrollmentsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Enrollments
                .Where(e => e.EnrollDate >= fromDate && e.EnrollDate <= toDate)
                .Include(e => e.Student)
                .Include(e => e.Course)
                .OrderByDescending(e => e.EnrollDate)
                .AsNoTracking()
                .ToListAsync();
        }

        // Thống kê đăng ký theo tháng
        public async Task<List<object>> GetEnrollmentStatisticsByMonthAsync(int year)
        {
            return await _context.Enrollments
                .Where(e => e.EnrollDate.Year == year)
                .GroupBy(e => e.EnrollDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count(),
                    Students = g.Select(e => e.Student.FullName).Distinct().Count()
                })
                .OrderBy(x => x.Month)
                .AsNoTracking()
                .ToListAsync<object>();
        }

        // Kiểm tra học viên đã đăng ký khóa học chưa
        public async Task<bool> IsStudentEnrolledAsync(int studentId, int courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }

        // Lấy số lượng học viên đã đăng ký cho mỗi khóa học
        public async Task<Dictionary<string, int>> GetEnrollmentCountByCourseAsync()
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .GroupBy(e => e.Course.Title)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        // Xóa đăng ký theo ID
        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}