using Microsoft.EntityFrameworkCore;
using Day6.Data;
using Day6.Models;

namespace Day6.Services
{
    public class CourseService
    {
        private readonly AppDbContext _context;

        public CourseService(AppDbContext context)
        {
            _context = context;
        }

        // Thêm khóa học mới
        public async Task<Course> AddCourseAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        // Lấy tất cả khóa học
        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .ToListAsync();
        }

        // Lấy khóa học theo ID
        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Cập nhật khóa học
        public async Task<Course?> UpdateCourseAsync(int id, Course updatedCourse)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return null;

            course.Title = updatedCourse.Title;
            course.Level = updatedCourse.Level;
            course.Duration = updatedCourse.Duration;

            await _context.SaveChangesAsync();
            return course;
        }

        // Xóa khóa học
        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        // Lấy danh sách khóa học có nhiều hơn 5 học viên
        public async Task<List<Course>> GetCoursesWithMoreThanFiveStudentsAsync()
        {
            return await _context.Courses
                .Where(c => c.Enrollments.Count() > 5)
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .AsNoTracking()
                .ToListAsync();
        }

        // Lấy khóa học theo cấp độ
        public async Task<List<Course>> GetCoursesByLevelAsync(string level)
        {
            return await _context.Courses
                .Where(c => c.Level.ToLower() == level.ToLower())
                .OrderBy(c => c.Title)
                .AsNoTracking()
                .ToListAsync();
        }

        // Tìm kiếm khóa học theo tiêu đề
        public async Task<List<Course>> SearchCoursesByTitleAsync(string title)
        {
            return await _context.Courses
                .Where(c => c.Title.Contains(title))
                .OrderBy(c => c.Title)
                .AsNoTracking()
                .ToListAsync();
        }

        // Lấy khóa học được đăng ký nhiều nhất
        public async Task<Course?> GetMostPopularCourseAsync()
        {
            return await _context.Courses
                .OrderByDescending(c => c.Enrollments.Count())
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        // Projection - thống kê khóa học
        public async Task<List<object>> GetCourseStatisticsAsync()
        {
            return await _context.Courses
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    c.Level,
                    c.Duration,
                    StudentCount = c.Enrollments.Count(),
                    LatestEnrollment = c.Enrollments.Max(e => (DateTime?)e.EnrollDate)
                })
                .AsNoTracking()
                .ToListAsync<object>();
        }

        // Lấy khóa học theo khoảng thời lượng
        public async Task<List<Course>> GetCoursesByDurationRangeAsync(int minDuration, int maxDuration)
        {
            return await _context.Courses
                .Where(c => c.Duration >= minDuration && c.Duration <= maxDuration)
                .OrderBy(c => c.Duration)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}