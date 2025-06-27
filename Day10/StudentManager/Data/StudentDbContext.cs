using Microsoft.EntityFrameworkCore;
using StudentManager.Models;

namespace StudentManager.Data
{
    public class StudentDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Using SQLite for development (no installation required)
            optionsBuilder.UseSqlite("Data Source=StudentManager.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Student entity
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.StudentCode).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.StudentCode).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Class).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Phone).HasMaxLength(20);
            });

            // Configure Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.OrderCode).IsUnique();
                
                entity.Property(e => e.OrderCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Description).HasMaxLength(500);

                // Configure relationship
                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Orders)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Students
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 1,
                    Name = "Nguyễn Văn An",
                    StudentCode = "SV001",
                    Email = "an.nguyen@email.com",
                    Phone = "0123456789",
                    Class = "CNTT01",
                    Grade = 8.5,
                    CreatedDate = DateTime.Now.AddDays(-30)
                },
                new Student
                {
                    Id = 2,
                    Name = "Trần Thị Bình",
                    StudentCode = "SV002",
                    Email = "binh.tran@email.com",
                    Phone = "0987654321",
                    Class = "CNTT02",
                    Grade = 7.8,
                    CreatedDate = DateTime.Now.AddDays(-25)
                },
                new Student
                {
                    Id = 3,
                    Name = "Lê Văn Cường",
                    StudentCode = "SV003",
                    Email = "cuong.le@email.com",
                    Phone = "0369852147",
                    Class = "CNTT01",
                    Grade = 9.2,
                    CreatedDate = DateTime.Now.AddDays(-20)
                }
            );

            // Seed Orders
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    OrderCode = "ORD001",
                    ProductName = "Laptop Dell",
                    Quantity = 1,
                    Price = 15000000,
                    StudentId = 1,
                    OrderDate = DateTime.Now.AddDays(-10),
                    Description = "Laptop cho học tập"
                },
                new Order
                {
                    Id = 2,
                    OrderCode = "ORD002",
                    ProductName = "Chuột không dây",
                    Quantity = 2,
                    Price = 250000,
                    StudentId = 1,
                    OrderDate = DateTime.Now.AddDays(-8),
                    Description = "Phụ kiện máy tính"
                },
                new Order
                {
                    Id = 3,
                    OrderCode = "ORD003",
                    ProductName = "Sách lập trình C#",
                    Quantity = 3,
                    Price = 120000,
                    StudentId = 2,
                    OrderDate = DateTime.Now.AddDays(-5),
                    Description = "Tài liệu học tập"
                }
            );
        }
    }
}