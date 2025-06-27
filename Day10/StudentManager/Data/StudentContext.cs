using Microsoft.EntityFrameworkCore;
using StudentManager.Models;

namespace StudentManager.Data
{
    public class StudentContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=students.db");
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.StudentCode).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);
                    
                entity.Property(e => e.StudentCode)
                    .IsRequired()
                    .HasMaxLength(20);
                    
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                    
                entity.Property(e => e.Class)
                    .IsRequired()
                    .HasMaxLength(50);
                    
                entity.Property(e => e.GPA)
                    .HasColumnType("REAL");
            });
            
            // Seed data
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 1,
                    FullName = "Nguyễn Văn An",
                    StudentCode = "SV001",
                    Email = "nguyenvanan@email.com",
                    PhoneNumber = "0901234567",
                    Class = "CNTT01",
                    GPA = 8.5,
                    DateOfBirth = new DateTime(2002, 3, 15),
                    CreatedAt = DateTime.Now
                },
                new Student
                {
                    Id = 2,
                    FullName = "Trần Thị Bình",
                    StudentCode = "SV002",
                    Email = "tranthibinh@email.com",
                    PhoneNumber = "0912345678",
                    Class = "CNTT01",
                    GPA = 7.8,
                    DateOfBirth = new DateTime(2002, 7, 22),
                    CreatedAt = DateTime.Now
                },
                new Student
                {
                    Id = 3,
                    FullName = "Lê Minh Cường",
                    StudentCode = "SV003",
                    Email = "leminhcuong@email.com",
                    PhoneNumber = "0923456789",
                    Class = "CNTT02",
                    GPA = 9.2,
                    DateOfBirth = new DateTime(2001, 11, 8),
                    CreatedAt = DateTime.Now
                },
                new Student
                {
                    Id = 4,
                    FullName = "Phạm Thị Dung",
                    StudentCode = "SV004",
                    Email = "phamthidung@email.com",
                    PhoneNumber = "0934567890",
                    Class = "CNTT02",
                    GPA = 8.9,
                    DateOfBirth = new DateTime(2002, 1, 30),
                    CreatedAt = DateTime.Now
                },
                new Student
                {
                    Id = 5,
                    FullName = "Hoàng Văn Em",
                    StudentCode = "SV005",
                    Email = "hoangvanem@email.com",
                    PhoneNumber = "0945678901",
                    Class = "CNTT03",
                    GPA = 7.5,
                    DateOfBirth = new DateTime(2002, 5, 12),
                    CreatedAt = DateTime.Now
                },
                new Student
                {
                    Id = 6,
                    FullName = "Vũ Thị Phương",
                    StudentCode = "SV006",
                    Email = "vuthiphuong@email.com",
                    PhoneNumber = "0956789012",
                    Class = "CNTT03",
                    GPA = 8.7,
                    DateOfBirth = new DateTime(2001, 9, 25),
                    CreatedAt = DateTime.Now
                },
                new Student
                {
                    Id = 7,
                    FullName = "Đỗ Minh Giang",
                    StudentCode = "SV007",
                    Email = "dominhhgiang@email.com",
                    PhoneNumber = "0967890123",
                    Class = "KTPM01",
                    GPA = 9.0,
                    DateOfBirth = new DateTime(2002, 4, 18),
                    CreatedAt = DateTime.Now
                },
                new Student
                {
                    Id = 8,
                    FullName = "Bùi Thị Hoa",
                    StudentCode = "SV008",
                    Email = "buithihoa@email.com",
                    PhoneNumber = "0978901234",
                    Class = "KTPM01",
                    GPA = 8.3,
                    DateOfBirth = new DateTime(2001, 12, 3),
                    CreatedAt = DateTime.Now
                },
                new Student
                {
                    Id = 9,
                    FullName = "Ngô Văn Inh",
                    StudentCode = "SV009",
                    Email = "ngovaninh@email.com",
                    PhoneNumber = "0989012345",
                    Class = "KTPM02",
                    GPA = 7.9,
                    DateOfBirth = new DateTime(2002, 6, 14),
                    CreatedAt = DateTime.Now
                },
                new Student
                {
                    Id = 10,
                    FullName = "Đinh Thị Kim",
                    StudentCode = "SV010",
                    Email = "dinhthikim@email.com",
                    PhoneNumber = "0990123456",
                    Class = "KTPM02",
                    GPA = 8.8,
                    DateOfBirth = new DateTime(2001, 8, 27),
                    CreatedAt = DateTime.Now
                }
            );
            
            base.OnModelCreating(modelBuilder);
        }
    }
}