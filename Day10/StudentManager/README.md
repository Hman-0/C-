# Student Manager - Ứng dụng Quản lý Sinh viên & Đơn hàng

## Mô tả
Ứng dụng desktop quản lý sinh viên và đơn hàng được xây dựng bằng WPF với kiến trúc MVVM và Entity Framework Core.

## Công nghệ sử dụng
- **Framework**: .NET 9.0
- **UI**: WPF (Windows Presentation Foundation)
- **Pattern**: MVVM (Model-View-ViewModel)
- **ORM**: Entity Framework Core
- **Database**: SQL Server LocalDB
- **MVVM Toolkit**: CommunityToolkit.Mvvm
- **Export**: iTextSharp (PDF), CSV

## Tính năng chính

### 1. Quản lý Sinh viên
- ✅ **CRUD Operations**: Thêm, sửa, xóa, xem danh sách sinh viên
- ✅ **Validation**: Kiểm tra dữ liệu đầu vào với Data Annotations
- ✅ **Tìm kiếm**: Tìm kiếm theo tên, mã sinh viên, email
- ✅ **Lọc**: Lọc sinh viên theo lớp
- ✅ **Export**: Xuất danh sách ra file PDF và CSV

### 2. Quản lý Đơn hàng
- ✅ **CRUD Operations**: Thêm, sửa, xóa, xem danh sách đơn hàng
- ✅ **Relationship**: Liên kết đơn hàng với sinh viên
- ✅ **Tính toán**: Tự động tính thành tiền (số lượng × đơn giá)
- ✅ **Báo cáo**: Xuất báo cáo đơn hàng theo sinh viên ra PDF

### 3. Giao diện người dùng
- ✅ **Modern UI**: Giao diện hiện đại, thân thiện
- ✅ **Responsive**: Tự động điều chỉnh kích thước
- ✅ **Data Binding**: Liên kết dữ liệu hai chiều
- ✅ **Tab Navigation**: Điều hướng bằng tab

### 4. Báo cáo & Export
- ✅ **PDF Export**: Xuất danh sách sinh viên và báo cáo đơn hàng
- ✅ **CSV Export**: Xuất danh sách sinh viên
- ✅ **Formatting**: Định dạng chuyên nghiệp với header, footer

## Cấu trúc dự án

```
StudentManager/
├── Models/
│   ├── Student.cs          # Model sinh viên
│   └── Order.cs            # Model đơn hàng
├── Data/
│   └── StudentDbContext.cs # Entity Framework DbContext
├── ViewModels/
│   ├── BaseViewModel.cs    # Base class cho ViewModel
│   └── MainViewModel.cs    # ViewModel chính
├── Views/
│   ├── MainWindow.xaml     # Giao diện chính
│   └── MainWindow.xaml.cs  # Code-behind
├── Services/
│   └── ReportService.cs    # Service xuất báo cáo
├── App.xaml                # Application resources
├── App.xaml.cs             # Application startup
└── Program.cs              # Entry point
```

## Cài đặt và chạy ứng dụng

### Yêu cầu hệ thống
- Windows 10/11
- .NET 9.0 Runtime
- SQL Server LocalDB (thường có sẵn với Visual Studio)

### Hướng dẫn cài đặt

1. **Clone hoặc tải về source code**

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Build ứng dụng**
   ```bash
   dotnet build
   ```

4. **Chạy ứng dụng**
   ```bash
   dotnet run
   ```

### Cấu hình Database

Ứng dụng sử dụng SQL Server LocalDB với connection string:
```
Server=(localdb)\mssqllocaldb;Database=StudentManagerDb;Trusted_Connection=true;MultipleActiveResultSets=true
```

Database sẽ được tự động tạo khi chạy ứng dụng lần đầu với dữ liệu mẫu.

## Hướng dẫn sử dụng

### 1. Quản lý Sinh viên
- **Thêm sinh viên**: Điền thông tin vào form bên phải và nhấn "Thêm"
- **Sửa sinh viên**: Chọn sinh viên trong danh sách, sửa thông tin và nhấn "Cập nhật"
- **Xóa sinh viên**: Chọn sinh viên và nhấn "Xóa"
- **Tìm kiếm**: Nhập từ khóa vào ô tìm kiếm
- **Lọc theo lớp**: Chọn lớp trong dropdown

### 2. Quản lý Đơn hàng
- **Xem đơn hàng**: Chọn sinh viên ở tab "Quản lý Sinh viên" trước
- **Thêm đơn hàng**: Chuyển sang tab "Quản lý Đơn hàng", điền form và nhấn "Thêm"
- **Sửa/Xóa**: Tương tự như quản lý sinh viên

### 3. Xuất báo cáo
- **PDF/CSV sinh viên**: Nhấn nút "Xuất PDF" hoặc "Xuất CSV" ở tab sinh viên
- **Báo cáo đơn hàng**: Chọn sinh viên và nhấn "Báo cáo PDF" ở tab đơn hàng
- File sẽ được lưu trên Desktop

## Validation Rules

### Sinh viên
- **Tên**: Bắt buộc, tối đa 100 ký tự
- **Mã sinh viên**: Bắt buộc, tối đa 20 ký tự, duy nhất
- **Email**: Bắt buộc, định dạng email hợp lệ, duy nhất
- **Số điện thoại**: Tùy chọn, định dạng số điện thoại
- **Lớp**: Bắt buộc, tối đa 50 ký tự
- **Điểm**: Tùy chọn, từ 0 đến 10

### Đơn hàng
- **Mã đơn hàng**: Bắt buộc, tối đa 50 ký tự, duy nhất
- **Tên sản phẩm**: Bắt buộc, tối đa 200 ký tự
- **Số lượng**: Bắt buộc, lớn hơn 0
- **Đơn giá**: Bắt buộc, lớn hơn 0
- **Mô tả**: Tùy chọn, tối đa 500 ký tự

## Kiến trúc MVVM

### Model
- `Student`: Thực thể sinh viên với validation attributes
- `Order`: Thực thể đơn hàng với relationship

### View
- `MainWindow.xaml`: Giao diện chính với data binding
- Sử dụng TabControl cho navigation
- DataGrid cho hiển thị danh sách
- Form controls cho input

### ViewModel
- `BaseViewModel`: Base class với INotifyPropertyChanged
- `MainViewModel`: Logic nghiệp vụ chính
- Commands cho các action
- ObservableCollection cho data binding

## Mở rộng

Ứng dụng có thể được mở rộng với:
- **Authentication**: Đăng nhập, phân quyền
- **More Reports**: Thêm các loại báo cáo khác
- **Import**: Nhập dữ liệu từ Excel/CSV
- **Backup/Restore**: Sao lưu và khôi phục dữ liệu
- **Multi-language**: Hỗ trợ đa ngôn ngữ
- **Themes**: Thay đổi giao diện

## Troubleshooting

### Lỗi kết nối Database
- Kiểm tra SQL Server LocalDB đã được cài đặt
- Thử thay đổi connection string trong `StudentDbContext.cs`

### Lỗi Export PDF
- Kiểm tra quyền ghi file trên Desktop
- Đảm bảo không có file cùng tên đang mở

### Lỗi Validation
- Kiểm tra dữ liệu đầu vào theo rules đã định
- Xem thông báo lỗi chi tiết trong MessageBox

## Tác giả
Ứng dụng được phát triển để thực hành:
- WPF UI Development
- MVVM Pattern
- Entity Framework Core
- Data Validation
- Report Generation