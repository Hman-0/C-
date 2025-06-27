# Financial Management API

## Mô tả
API quản lý dòng tiền và phân tích tài chính doanh nghiệp được xây dựng bằng ASP.NET Core 9.0. Hệ thống hỗ trợ quản lý thu chi từ nhiều nguồn, tính toán báo cáo tài chính theo tháng/quý, theo dõi lợi nhuận và phân tích hiệu suất theo danh mục.

## Tính năng chính

### 1. Quản lý Giao dịch (Transactions)
- ✅ Thêm giao dịch thu/chi
- ✅ Lọc giao dịch theo ngày, phòng ban, danh mục
- ✅ Validation nghiệp vụ (Amount > 0, Date <= hiện tại)
- ✅ Kiểm tra ngân sách phòng ban tự động
- ✅ Tổng hợp giao dịch theo danh mục

### 2. Quản lý Phòng ban (Departments)
- ✅ CRUD phòng ban
- ✅ Thiết lập ngân sách cho từng phòng ban
- ✅ Kiểm tra ràng buộc khi xóa phòng ban

### 3. Báo cáo Tài chính (Reports)
- ✅ Báo cáo dòng tiền theo tháng/năm
- ✅ Phân tích ngân sách từng phòng ban
- ✅ Export báo cáo ra Excel
- ✅ Cảnh báo dòng tiền âm liên tiếp
- ✅ Dữ liệu biểu đồ thu chi theo thời gian

### 4. Tính năng nâng cao
- ✅ AutoMapper cho DTO mapping
- ✅ Swagger UI documentation
- ✅ In-Memory Database với seed data
- ✅ CORS configuration
- ✅ Export Excel với EPPlus

## Cấu trúc Project

```
Day8/
├── Controllers/
│   ├── TransactionsController.cs    # API quản lý giao dịch
│   ├── ReportsController.cs         # API báo cáo tài chính
│   └── DepartmentsController.cs     # API quản lý phòng ban
├── Models/
│   ├── Transaction.cs               # Entity giao dịch
│   ├── Department.cs                # Entity phòng ban
│   └── CashFlowReport.cs           # Model báo cáo
├── DTOs/
│   └── TransactionDTOs.cs          # Data Transfer Objects
├── Services/
│   ├── ITransactionService.cs       # Interface service giao dịch
│   ├── TransactionService.cs        # Implementation service giao dịch
│   ├── IReportService.cs           # Interface service báo cáo
│   └── ReportService.cs            # Implementation service báo cáo
├── Data/
│   └── FinancialDbContext.cs       # Entity Framework DbContext
├── Mappings/
│   └── MappingProfile.cs           # AutoMapper configuration
└── Program.cs                      # Application entry point
```

## API Endpoints

### Transactions Controller
- `POST /api/transactions` - Tạo giao dịch mới
- `GET /api/transactions` - Lấy danh sách giao dịch (có filter)
- `GET /api/transactions/summary` - Tổng hợp giao dịch theo danh mục
- `GET /api/transactions/validate-budget/{departmentId}` - Kiểm tra ngân sách

### Reports Controller
- `GET /api/reports/cashflow` - Báo cáo dòng tiền
- `GET /api/reports/budget-variance` - Phân tích ngân sách tất cả phòng ban
- `GET /api/reports/budget-variance/{departmentId}` - Phân tích ngân sách một phòng ban
- `GET /api/reports/cashflow/export` - Export báo cáo Excel
- `GET /api/reports/cashflow-warning/{departmentId}` - Kiểm tra cảnh báo dòng tiền
- `GET /api/reports/chart-data` - Dữ liệu biểu đồ

### Departments Controller
- `GET /api/departments` - Lấy danh sách phòng ban
- `GET /api/departments/{id}` - Lấy thông tin phòng ban
- `POST /api/departments` - Tạo phòng ban mới
- `PUT /api/departments/{id}` - Cập nhật phòng ban
- `DELETE /api/departments/{id}` - Xóa phòng ban

## Cách chạy ứng dụng

### Yêu cầu hệ thống
- .NET 9.0 SDK
- Visual Studio 2022 hoặc VS Code

### Các bước chạy

1. **Clone hoặc tải project**
```bash
cd Day8
```

2. **Restore packages**
```bash
dotnet restore
```

3. **Chạy ứng dụng**
```bash
dotnet run
```

4. **Truy cập Swagger UI**
- Mở browser và truy cập: `https://localhost:5001` hoặc `http://localhost:5000`
- Swagger UI sẽ hiển thị tất cả API endpoints với documentation

## Dữ liệu mẫu

Hệ thống tự động tạo dữ liệu mẫu khi khởi động:

### Phòng ban mẫu:
- IT (Budget: 50,000)
- Marketing (Budget: 30,000) 
- HR (Budget: 25,000)
- Finance (Budget: 40,000)

### Giao dịch mẫu:
- Doanh thu bán hàng: +50,000 (IT)
- Lương nhân viên: -15,000 (IT)
- Quảng cáo Facebook: -8,000 (Marketing)
- Văn phòng phẩm: -12,000 (HR)

## Validation Rules

1. **Giao dịch (Transaction)**
   - Amount phải > 0
   - Date không được vượt quá ngày hiện tại
   - Category và Description có giới hạn độ dài
   - DepartmentId phải tồn tại

2. **Ngân sách (Budget)**
   - Tự động reject nếu tổng chi phòng ban vượt BudgetLimit
   - Kiểm tra ngân sách theo tháng hiện tại

3. **Phòng ban (Department)**
   - Name bắt buộc, tối đa 100 ký tự
   - BudgetLimit phải >= 0
   - Không thể xóa phòng ban có giao dịch

## Business Logic

### Tính toán tài chính
- **Net Profit** = TotalIncome - TotalExpense
- **Budget Variance** = BudgetLimit - DepartmentTotalExpense
- **Variance Percentage** = (Variance / BudgetLimit) * 100

### Phân loại chi tiêu
- **Chi cố định**: Lương, Văn phòng phẩm, Thuê mặt bằng
- **Chi biến đổi**: Marketing, Dịch vụ, Du lịch

### Cảnh báo hệ thống
- Cảnh báo khi dòng tiền âm trong 2 tháng liên tiếp
- Cảnh báo khi chi tiêu vượt ngân sách phòng ban

## Testing với Swagger

1. **Tạo giao dịch mới**
   - Endpoint: `POST /api/transactions`
   - Test với amount âm → Expect validation error
   - Test với date tương lai → Expect validation error
   - Test với departmentId không tồn tại → Expect error

2. **Filter giao dịch**
   - Endpoint: `GET /api/transactions`
   - Test filter theo startDate, endDate
   - Test filter theo departmentId
   - Test filter theo category

3. **Báo cáo dòng tiền**
   - Endpoint: `GET /api/reports/cashflow`
   - Test với month, year parameters
   - Test với departmentId parameter

4. **Export Excel**
   - Endpoint: `GET /api/reports/cashflow/export`
   - Download file Excel và kiểm tra nội dung

## Mở rộng tương lai

- [ ] Authentication & Authorization
- [ ] Role-based access (mỗi phòng ban chỉ xem giao dịch của mình)
- [ ] Export PDF reports
- [ ] Email notifications cho cảnh báo
- [ ] Dashboard với real-time charts
- [ ] Audit trail cho các thay đổi
- [ ] Backup/Restore database
- [ ] Multi-currency support

## Liên hệ

Nếu có thắc mắc hoặc cần hỗ trợ, vui lòng liên hệ:
- Email: finance@company.com
- Team: Financial Development Team