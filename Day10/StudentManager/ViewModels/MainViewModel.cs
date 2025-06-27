using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using StudentManager.Data;
using StudentManager.Models;
using StudentManager.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace StudentManager.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly StudentDbContext _context;
        private readonly ReportService _reportService;
        
        // Collections
        public ObservableCollection<Student> Students { get; set; }
        public ObservableCollection<Student> FilteredStudents { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<string> Classes { get; set; }

        // Selected items
        private Student? _selectedStudent;
        private Order? _selectedOrder;

        // Form properties for Student
        private string _studentName = string.Empty;
        private string _studentCode = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _studentClass = string.Empty;
        private double? _grade;

        // Form properties for Order
        private string _orderCode = string.Empty;
        private string _productName = string.Empty;
        private int _quantity = 1;
        private decimal _price;
        private string _description = string.Empty;

        // Search and filter
        private string _searchText = string.Empty;
        private string _selectedClassFilter = "Tất cả";

        public MainViewModel()
        {
            _context = new StudentDbContext();
            _reportService = new ReportService();
            
            Students = new ObservableCollection<Student>();
            FilteredStudents = new ObservableCollection<Student>();
            Orders = new ObservableCollection<Order>();
            Classes = new ObservableCollection<string> { "Tất cả" };

            Title = "Quản lý Sinh viên & Đơn hàng";
            
            LoadData();
        }

        #region Properties

        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                SetProperty(ref _selectedStudent, value);
                if (value != null)
                {
                    LoadStudentToForm(value);
                    LoadOrdersForStudent(value.Id);
                }
                OnPropertyChanged(nameof(IsStudentSelected));
            }
        }

        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                SetProperty(ref _selectedOrder, value);
                if (value != null)
                {
                    LoadOrderToForm(value);
                }
                OnPropertyChanged(nameof(IsOrderSelected));
            }
        }

        public bool IsStudentSelected => SelectedStudent != null;
        public bool IsOrderSelected => SelectedOrder != null;

        // Student Form Properties
        public string StudentName
        {
            get => _studentName;
            set => SetProperty(ref _studentName, value);
        }

        public string StudentCode
        {
            get => _studentCode;
            set => SetProperty(ref _studentCode, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string StudentClass
        {
            get => _studentClass;
            set => SetProperty(ref _studentClass, value);
        }

        public double? Grade
        {
            get => _grade;
            set => SetProperty(ref _grade, value);
        }

        // Order Form Properties
        public string OrderCode
        {
            get => _orderCode;
            set => SetProperty(ref _orderCode, value);
        }

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        // Search and Filter Properties
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilterStudents();
            }
        }

        public string SelectedClassFilter
        {
            get => _selectedClassFilter;
            set
            {
                SetProperty(ref _selectedClassFilter, value);
                FilterStudents();
            }
        }

        #endregion

        #region Commands

        [RelayCommand]
        private async Task AddStudent()
        {
            if (!ValidateStudentForm()) return;

            try
            {
                var student = new Student
                {
                    Name = StudentName,
                    StudentCode = StudentCode,
                    Email = Email,
                    Phone = Phone,
                    Class = StudentClass,
                    Grade = Grade,
                    CreatedDate = DateTime.Now
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                Students.Add(student);
                FilterStudents();
                UpdateClasses();
                ClearStudentForm();

                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm sinh viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task UpdateStudent()
        {
            if (SelectedStudent == null || !ValidateStudentForm()) return;

            try
            {
                SelectedStudent.Name = StudentName;
                SelectedStudent.StudentCode = StudentCode;
                SelectedStudent.Email = Email;
                SelectedStudent.Phone = Phone;
                SelectedStudent.Class = StudentClass;
                SelectedStudent.Grade = Grade;
                SelectedStudent.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                FilterStudents();
                UpdateClasses();

                MessageBox.Show("Cập nhật sinh viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật sinh viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeleteStudent()
        {
            if (SelectedStudent == null) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa sinh viên '{SelectedStudent.Name}'?", 
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Students.Remove(SelectedStudent);
                    await _context.SaveChangesAsync();

                    Students.Remove(SelectedStudent);
                    FilterStudents();
                    UpdateClasses();
                    ClearStudentForm();
                    Orders.Clear();

                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa sinh viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task AddOrder()
        {
            if (SelectedStudent == null)
            {
                MessageBox.Show("Vui lòng chọn sinh viên trước khi thêm đơn hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateOrderForm()) return;

            try
            {
                var order = new Order
                {
                    OrderCode = OrderCode,
                    ProductName = ProductName,
                    Quantity = Quantity,
                    Price = Price,
                    Description = Description,
                    StudentId = SelectedStudent.Id,
                    OrderDate = DateTime.Now
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                Orders.Add(order);
                ClearOrderForm();

                MessageBox.Show("Thêm đơn hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task UpdateOrder()
        {
            if (SelectedOrder == null || !ValidateOrderForm()) return;

            try
            {
                SelectedOrder.OrderCode = OrderCode;
                SelectedOrder.ProductName = ProductName;
                SelectedOrder.Quantity = Quantity;
                SelectedOrder.Price = Price;
                SelectedOrder.Description = Description;

                await _context.SaveChangesAsync();

                MessageBox.Show("Cập nhật đơn hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeleteOrder()
        {
            if (SelectedOrder == null) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa đơn hàng '{SelectedOrder.OrderCode}'?", 
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Orders.Remove(SelectedOrder);
                    await _context.SaveChangesAsync();

                    Orders.Remove(SelectedOrder);
                    ClearOrderForm();

                    MessageBox.Show("Xóa đơn hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void ClearStudentForm()
        {
            StudentName = string.Empty;
            StudentCode = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            StudentClass = string.Empty;
            Grade = null;
            SelectedStudent = null;
        }

        [RelayCommand]
        private void ClearOrderForm()
        {
            OrderCode = string.Empty;
            ProductName = string.Empty;
            Quantity = 1;
            Price = 0;
            Description = string.Empty;
            SelectedOrder = null;
        }

        [RelayCommand]
        private async Task ExportStudentsPdf()
        {
            try
            {
                await _reportService.ExportStudentsToPdf(FilteredStudents.ToList());
                MessageBox.Show("Xuất file PDF thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task ExportStudentsCsv()
        {
            try
            {
                await _reportService.ExportStudentsToCsv(FilteredStudents.ToList());
                MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất CSV: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task ExportOrderReport()
        {
            if (SelectedStudent == null)
            {
                MessageBox.Show("Vui lòng chọn sinh viên để xuất báo cáo đơn hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _reportService.ExportOrderReportToPdf(SelectedStudent, Orders.ToList());
                MessageBox.Show("Xuất báo cáo đơn hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Private Methods

        private async void LoadData()
        {
            try
            {
                var students = await _context.Students.Include(s => s.Orders).ToListAsync();
                Students.Clear();
                foreach (var student in students)
                {
                    Students.Add(student);
                }
                
                FilterStudents();
                UpdateClasses();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterStudents()
        {
            FilteredStudents.Clear();
            
            var filtered = Students.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(s => 
                    s.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    s.StudentCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    s.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
            
            if (SelectedClassFilter != "Tất cả")
            {
                filtered = filtered.Where(s => s.Class == SelectedClassFilter);
            }
            
            foreach (var student in filtered)
            {
                FilteredStudents.Add(student);
            }
        }

        private void UpdateClasses()
        {
            var classes = Students.Select(s => s.Class).Distinct().OrderBy(c => c).ToList();
            Classes.Clear();
            Classes.Add("Tất cả");
            foreach (var cls in classes)
            {
                Classes.Add(cls);
            }
        }

        private async void LoadOrdersForStudent(int studentId)
        {
            try
            {
                var orders = await _context.Orders.Where(o => o.StudentId == studentId).ToListAsync();
                Orders.Clear();
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStudentToForm(Student student)
        {
            StudentName = student.Name;
            StudentCode = student.StudentCode;
            Email = student.Email;
            Phone = student.Phone ?? string.Empty;
            StudentClass = student.Class;
            Grade = student.Grade;
        }

        private void LoadOrderToForm(Order order)
        {
            OrderCode = order.OrderCode;
            ProductName = order.ProductName;
            Quantity = order.Quantity;
            Price = order.Price;
            Description = order.Description ?? string.Empty;
        }

        private bool ValidateStudentForm()
        {
            var student = new Student
            {
                Name = StudentName,
                StudentCode = StudentCode,
                Email = Email,
                Phone = Phone,
                Class = StudentClass,
                Grade = Grade
            };

            var context = new ValidationContext(student);
            var results = new List<ValidationResult>();
            
            if (!Validator.TryValidateObject(student, context, results, true))
            {
                var errors = string.Join("\n", results.Select(r => r.ErrorMessage));
                MessageBox.Show($"Dữ liệu không hợp lệ:\n{errors}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool ValidateOrderForm()
        {
            var order = new Order
            {
                OrderCode = OrderCode,
                ProductName = ProductName,
                Quantity = Quantity,
                Price = Price,
                Description = Description,
                StudentId = SelectedStudent?.Id ?? 0
            };

            var context = new ValidationContext(order);
            var results = new List<ValidationResult>();
            
            if (!Validator.TryValidateObject(order, context, results, true))
            {
                var errors = string.Join("\n", results.Select(r => r.ErrorMessage));
                MessageBox.Show($"Dữ liệu không hợp lệ:\n{errors}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}