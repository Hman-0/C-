using Microsoft.EntityFrameworkCore;
using Day6.Models;

namespace Day6.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Cấu hình kết nối MySQL
            var connectionString = "Server=localhost;Database=StudentManagementDB;User=root;Password=;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Student
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.FullName).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(s => s.Email).IsUnique();
                entity.Property(s => s.BirthDate).IsRequired();
                entity.Property(s => s.CreatedAt).HasColumnType("datetime");
            });

            // Cấu hình Course
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Level).IsRequired().HasMaxLength(50);
                entity.Property(c => c.Duration).IsRequired();
                entity.Property(c => c.CreatedAt).HasColumnType("datetime");
            });

            // Cấu hình Enrollment và quan hệ
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EnrollDate).IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                // Cấu hình quan hệ với Student
                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Enrollments)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Cấu hình quan hệ với Course
                entity.HasOne(e => e.Course)
                      .WithMany(c => c.Enrollments)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Đảm bảo một học viên không thể đăng ký cùng một khóa học nhiều lần
                entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
            });
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var addedEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added)
                .Select(e => e.Entity);

            var modifiedEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .Select(e => e.Entity);

            foreach (var entity in addedEntries)
            {
                if (entity is Student student)
                {
                    student.CreatedAt = DateTime.Now;
                    student.UpdatedAt = DateTime.Now;
                }
                else if (entity is Course course)
                {
                    course.CreatedAt = DateTime.Now;
                    course.UpdatedAt = DateTime.Now;
                }
                else if (entity is Enrollment enrollment)
                {
                    enrollment.CreatedAt = DateTime.Now;
                    enrollment.UpdatedAt = DateTime.Now;
                }
            }

            foreach (var entity in modifiedEntries)
            {
                if (entity is Student student)
                    student.UpdatedAt = DateTime.Now;
                else if (entity is Course course)
                    course.UpdatedAt = DateTime.Now;
                else if (entity is Enrollment enrollment)
                    enrollment.UpdatedAt = DateTime.Now;
            }
        }
    }
}