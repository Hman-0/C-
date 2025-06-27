# 🎓 Hệ Thống Quản Lý Sinh Viên

## Mô tả
Ứng dụng console quản lý sinh viên được xây dựng bằng C# và Entity Framework Core, thực hiện yêu cầu 3 trong dự án mini tổng hợp ôn tập cuối khóa.

## Tính năng chính

### 📋 Quản lý sinh viên
- ✅ **CRUD Operations**: Thêm, xem, sửa, xóa sinh viên
- 🔍 **Tìm kiếm**: Tìm kiếm theo tên, mã sinh viên, lớp
- 🏫 **Lọc theo lớp**: Xem danh sách sinh viên theo từng lớp
- ✏️ **Validation**: Kiểm tra dữ liệu đầu vào với Data Annotations

### 📊 Báo cáo và thống kê
- 📈 **Thống kê tổng quan**: Tổng số sinh viên, GPA trung bình
- 📋 **Thống kê theo lớp**: Số lượng sinh viên và GPA trung bình mỗi lớp
- 🎯 **Phân bố GPA**: Thống kê theo xếp loại học lực

### 📄 Xuất dữ liệu
- 📄 **Xuất PDF**: Tạo báo cáo PDF với định dạng đẹp
- 📊 **Xuất CSV**: Xuất dữ liệu ra file CSV để xử lý trong Excel
- 📋 **Xuất cả hai**: Tùy chọn xuất đồng thời PDF và CSV

## Công nghệ sử dụng

- **Framework**: .NET 9.0
- **Database**: SQLite với Entity Framework Core 8.0
- **UI**: Console Application với ConsoleTables
- **PDF Export**: iTextSharp
- **CSV Export**: CsvHelper
- **Validation**: Data Annotations

## Cấu trúc dự án

```
StudentManager/
├── Models/
│   └── Student.cs              # Entity model
├── Data/
│   └── StudentContext.cs       # DbContext
├── Services/
│   ├── StudentService.cs       # Business logic
│   └── ExportService.cs        # Export functionality
├── Program.cs                  # Main application
├── StudentManager.csproj       # Project file
└── README.md                   # Documentation
```

## Cài đặt và chạy

### Yêu cầu hệ thống
- .NET 9.0 SDK
- Windows/Linux/macOS

### Các bước chạy ứng dụng

1. **Clone hoặc tải về dự án**
2. **Restore packages**:
   ```bash
   dotnet restore
   ```

3. **Chạy ứng dụng**:
   ```bash
   dotnet run
   ```

4. **Database**: SQLite database sẽ được tự động tạo khi chạy lần đầu

## Hướng dẫn sử dụng

### Menu chính
Khi khởi động, ứng dụng sẽ hiển thị menu với các tùy chọn:

```
📋 MENU CHÍNH
═══════════════════════════════════════
1. 📖 Xem danh sách sinh viên
2. ➕ Thêm sinh viên mới
3. ✏️ Cập nhật thông tin sinh viên
4. 🗑️ Xóa sinh viên
5. 🔍 Tìm kiếm sinh viên
6. 🏫 Xem sinh viên theo lớp
7. 📊 Thống kê
8. 📄 Xuất dữ liệu (PDF/CSV)
0. 🚪 Thoát
```

### Thêm sinh viên mới
- Nhập đầy đủ thông tin: họ tên, mã SV, email, lớp, GPA, ngày sinh
- Hệ thống sẽ validate dữ liệu và kiểm tra trùng lặp
- Mã sinh viên và email phải là duy nhất

### Tìm kiếm
- Có thể tìm theo tên, mã sinh viên, hoặc lớp
- Kết quả hiển thị dưới dạng bảng

### Xuất dữ liệu
- File được lưu trong thư mục `Exports/`
- Tên file có timestamp để tránh trùng lặp
- PDF: Định dạng báo cáo chuyên nghiệp
- CSV: Tương thích với Excel, có BOM UTF-8

## Validation Rules

- **Họ tên**: Bắt buộc, tối đa 100 ký tự
- **Mã sinh viên**: Bắt buộc, tối đa 20 ký tự, duy nhất
- **Email**: Bắt buộc, định dạng email hợp lệ, duy nhất
- **Lớp**: Bắt buộc, tối đa 50 ký tự
- **GPA**: Từ 0 đến 10
- **Số điện thoại**: Tùy chọn, định dạng số điện thoại

## Kiến thức được ôn tập

### Entity Framework Core
- ✅ Code First approach
- ✅ DbContext configuration
- ✅ Entity relationships
- ✅ Database migrations
- ✅ LINQ queries

### CRUD Operations
- ✅ Create: Thêm sinh viên mới
- ✅ Read: Xem danh sách, tìm kiếm
- ✅ Update: Cập nhật thông tin
- ✅ Delete: Xóa sinh viên

### Data Validation
- ✅ Data Annotations
- ✅ Custom validation logic
- ✅ Error handling

### LINQ & DateTime
- ✅ Filtering và searching
- ✅ Grouping và aggregation
- ✅ DateTime operations
- ✅ Statistical calculations

### Console UI
- ✅ Menu-driven interface
- ✅ Table formatting với ConsoleTables
- ✅ User input validation
- ✅ Error messaging

### File Operations
- ✅ PDF generation
- ✅ CSV export
- ✅ Directory management

## Mở rộng có thể thực hiện

1. **Thêm entity mới**: Khoa, Môn học, Điểm số
2. **Authentication**: Đăng nhập cho admin
3. **Import data**: Nhập từ file Excel/CSV
4. **Advanced reporting**: Biểu đồ, charts
5. **Web interface**: Chuyển sang ASP.NET Core
6. **API endpoints**: RESTful API

## Troubleshooting

### Lỗi thường gặp

1. **Package restore failed**:
   ```bash
   dotnet clean
   dotnet restore
   ```

2. **Database connection error**:
   - Kiểm tra quyền ghi file trong thư mục dự án
   - Database SQLite sẽ được tạo tự động

3. **Export file error**:
   - Đảm bảo thư mục `Exports/` có quyền ghi
   - Đóng file PDF/CSV nếu đang mở

## Tác giả
Dự án được phát triển như một phần của khóa học C# - Yêu cầu 3: Ứng dụng desktop quản lý sinh viên.

---

**Lưu ý**: Đây là ứng dụng demo cho mục đích học tập. Trong môi trường production, cần bổ sung thêm các tính năng bảo mật và tối ưu hóa hiệu suất.