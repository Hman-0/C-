# Ứng dụng Quản lý Học viên và Khóa học

## Mô tả
Ứng dụng quản lý học viên cho trung tâm đào tạo lập trình, được xây dựng bằng C# và Entity Framework Core với MySQL.

## Tính năng chính

### 1. Quản lý Học viên
- ✅ Thêm học viên mới
- ✅ Xem danh sách tất cả học viên
- ✅ Tìm kiếm học viên theo ID và tên
- ✅ Cập nhật thông tin học viên
- ✅ Xóa học viên
- ✅ Lọc học viên theo ngày sinh
- ✅ Sắp xếp học viên theo tên

### 2. Quản lý Khóa học
- ✅ Thêm khóa học mới
- ✅ Xem danh sách tất cả khóa học
- ✅ Tìm kiếm khóa học theo ID và tiêu đề
- ✅ Cập nhật thông tin khóa học
- ✅ Xóa khóa học
- ✅ Lọc khóa học theo cấp độ và thời lượng

### 3. Quản lý Đăng ký
- ✅ Đăng ký khóa học cho học viên
- ✅ Hủy đăng ký khóa học
- ✅ Xem tất cả đăng ký
- ✅ Xem đăng ký theo học viên
- ✅ Xem đăng ký theo khóa học
- ✅ Thống kê đăng ký theo thời gian

### 4. Báo cáo và Thống kê
- ✅ Khóa học có nhiều hơn 5 học viên
- ✅ Khóa học được đăng ký nhiều nhất
- ✅ Thống kê học viên
- ✅ Thống kê khóa học theo cấp độ
- ✅ Thống kê đăng ký theo tháng
- ✅ Báo cáo nâng cao

## Cấu trúc dự án

```
Day6/
├── Models/
│   ├── Student.cs          # Model học viên
│   ├── Course.cs           # Model khóa học
│   └── Enrollment.cs       # Model đăng ký
├── Data/
│   └── AppDbContext.cs     # DbContext và cấu hình EF
├── Services/
│   ├── StudentService.cs   # Service xử lý logic học viên
│   ├── CourseService.cs    # Service xử lý logic khóa học
│   └── EnrollmentService.cs # Service xử lý logic đăng ký
├── Program.cs              # Entry point và menu chính
├── ProgramMethods.cs       # Các phương thức xử lý học viên
├── ProgramMethods2.cs      # Các phương thức xử lý khóa học và đăng ký
├── ProgramReports.cs       # Các phương thức báo cáo
└── Day6.csproj            # File cấu hình project
```

## Công nghệ sử dụng

- **Framework**: .NET 9.0
- **ORM**: Entity Framework Core 9.0
- **Database**: MySQL (sử dụng Pomelo.EntityFrameworkCore.MySql)
- **Architecture**: Code First, Repository Pattern, Service Layer

## Cài đặt và Chạy

### Yêu cầu hệ thống
- .NET 9.0 SDK
- MySQL Server
- Visual Studio 2022 hoặc VS Code

### Bước 1: Clone và cài đặt dependencies
```bash
cd Day6
dotnet restore
```

### Bước 2: Cấu hình database
Mở file `Data/AppDbContext.cs` và cập nhật connection string:
```csharp
var connectionString = "Server=localhost;Database=StudentManagementDB;User=root;Password=your_password;";
```

### Bước 3: Tạo và áp dụng migrations
```bash
# Cài đặt EF Tools (nếu chưa có)
dotnet tool install --global dotnet-ef

# Tạo migration
dotnet ef migrations add InitialCreate

# Cập nhật database
dotnet ef database update
```

### Bước 4: Chạy ứng dụng
```bash
dotnet run
```

## Tính năng Entity Framework Core

### Data Annotations được sử dụng
- `[Key]`: Khóa chính
- `[Required]`: Trường bắt buộc
- `[MaxLength]`: Độ dài tối đa
- `[EmailAddress]`: Validation email
- `[Range]`: Giới hạn giá trị
- `[DataType]`: Kiểu dữ liệu
- `[ForeignKey]`: Khóa ngoại
- `[DatabaseGenerated]`: Tự động tạo giá trị

### Fluent API
- Cấu hình quan hệ 1-n giữa Student/Course và Enrollment
- Thiết lập unique constraints
- Cấu hình cascade delete
- Default values cho timestamps

### LINQ to Entities
- `Include()` và `ThenInclude()`: Eager loading
- `AsNoTracking()`: Tối ưu cho read-only queries
- `Select()`: Projection để chỉ lấy dữ liệu cần thiết
- `Where()`, `OrderBy()`, `GroupBy()`: Filtering và sorting
- `Any()`, `Count()`, `Sum()`, `Average()`: Aggregate functions

### Tối ưu hóa
- **Eager Loading**: Sử dụng `Include()` để load related data
- **Projection**: Sử dụng `Select()` để chỉ lấy fields cần thiết
- **No Tracking**: Sử dụng `AsNoTracking()` cho read-only operations
- **Async Operations**: Tất cả database operations đều async
- **Connection Pooling**: Tự động quản lý bởi EF Core

## Mô hình dữ liệu

### Student (Học viên)
- Id (int, PK)
- FullName (string, required, max 100)
- Email (string, required, unique, max 150)
- BirthDate (DateTime, required)
- CreatedAt (DateTime, auto-generated)
- UpdatedAt (DateTime, nullable)

### Course (Khóa học)
- Id (int, PK)
- Title (string, required, max 200)
- Level (string, required, max 50)
- Duration (int, required, 1-1000 hours)
- CreatedAt (DateTime, auto-generated)
- UpdatedAt (DateTime, nullable)

### Enrollment (Đăng ký)
- Id (int, PK)
- StudentId (int, FK)
- CourseId (int, FK)
- EnrollDate (DateTime, required)
- CreatedAt (DateTime, auto-generated)
- UpdatedAt (DateTime, nullable)

### Quan hệ
- Student 1:n Enrollment
- Course 1:n Enrollment
- Student n:n Course (thông qua Enrollment)

## Dữ liệu mẫu
Ứng dụng tự động tạo dữ liệu mẫu khi chạy lần đầu:
- 6 học viên mẫu
- 6 khóa học với các cấp độ khác nhau
- 9 đăng ký mẫu

## Tính năng nâng cao

### EF Hooks
- Tự động cập nhật `UpdatedAt` khi entity được modify
- Tự động set `CreatedAt` khi tạo mới

### Service Layer
- Tách biệt logic nghiệp vụ khỏi UI
- Dependency injection pattern
- Async/await cho tất cả operations

### Error Handling
- Try-catch cho tất cả database operations
- Validation messages tiếng Việt
- User-friendly error messages

### Performance
- Sử dụng async/await
- Projection cho queries chỉ đọc
- No-tracking cho read-only operations
- Efficient LINQ queries

## Hướng dẫn sử dụng

1. **Khởi động**: Chạy `dotnet run` và chọn menu tương ứng
2. **Thêm dữ liệu**: Bắt đầu bằng việc thêm học viên và khóa học
3. **Đăng ký**: Đăng ký học viên vào các khóa học
4. **Báo cáo**: Xem các báo cáo và thống kê
5. **Thoát**: Chọn option 5 từ menu chính

## Troubleshooting

### Lỗi kết nối MySQL
- Kiểm tra MySQL server đang chạy
- Xác nhận connection string đúng
- Kiểm tra username/password

### Lỗi Migration
```bash
# Xóa migrations cũ
rm -rf Migrations/

# Tạo migration mới
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Lỗi Dependencies
```bash
# Restore packages
dotnet restore

# Clear cache nếu cần
dotnet nuget locals all --clear
```

## Mở rộng

Có thể mở rộng ứng dụng với:
- Web API với ASP.NET Core
- Authentication và Authorization
- File upload cho ảnh đại diện
- Email notifications
- Payment integration
- Mobile app với Xamarin/MAUI
- Real-time updates với SignalR

## Tác giả
Ứng dụng được phát triển như một bài tập thực hành Entity Framework Core và MySQL.