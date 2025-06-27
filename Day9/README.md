# Hệ thống tính lương nhân viên - Unit Test & Debug

## Mô tả dự án

Dự án này triển khai một hệ thống tính lương nhân viên hoàn chỉnh với unit tests, mocking và coverage reporting.

## Cấu trúc dự án

```
Day9/
├── Models/
│   └── Employee.cs              # Model nhân viên
├── Services/
│   ├── SalaryCalculator.cs      # Logic tính lương
│   └── PayrollService.cs        # Service tính lương theo ID
├── Repositories/
│   ├── IEmployeeRepository.cs   # Interface repository
│   └── EmployeeRepository.cs    # Implementation repository
├── Tests/
│   ├── SalaryCalculatorTests.cs # Unit tests cho SalaryCalculator
│   └── PayrollServiceTests.cs   # Unit tests với Moq cho PayrollService
├── Program.cs                   # Demo application
└── Day9.csproj                  # Project file với test packages
```

## Công nghệ sử dụng

- **.NET 9.0**: Framework chính
- **xUnit**: Framework unit testing
- **Moq**: Library mocking cho dependency injection
- **Coverlet**: Tool đo test coverage
- **Visual Studio**: IDE với debugging tools

## Cài đặt và chạy

### 1. Restore packages
```bash
dotnet restore
```

### 2. Build project
```bash
dotnet build
```

### 3. Chạy application demo
```bash
dotnet run
```

### 4. Chạy unit tests
```bash
dotnet test
```

### 5. Chạy tests với coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### 6. Tạo coverage report chi tiết
```bash
# Cài đặt ReportGenerator (chỉ cần 1 lần)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Chạy test với coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Tạo HTML report
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./CoverageReport" -reporttypes:Html
```

## Unit Tests

### SalaryCalculatorTests

✅ **Test cases được cover:**
- Tính lương đúng cho nhân viên bình thường
- Trường hợp Bonus = 0, Deduction = 0
- Exception khi BaseSalary < 0
- Exception khi Deduction < 0
- Exception khi Employee = null
- Theory tests với nhiều bộ dữ liệu khác nhau

### PayrollServiceTests (với Moq)

✅ **Test cases được cover:**
- Mock IEmployeeRepository.GetById()
- Verify method calls với Times.Once
- Test với repository trả về null
- Test với dữ liệu không hợp lệ
- Theory tests với nhiều scenarios
- Test multiple calls với Times.Exactly()

## Debugging Techniques

### 1. Breakpoints
- Đặt breakpoint trong `SalaryCalculator.CalculateNetSalary()`
- Đặt breakpoint trong test methods
- Sử dụng conditional breakpoints cho specific test cases

### 2. Watch Window
```csharp
// Các biến cần watch trong debug:
emp.BaseSalary
emp.Bonus
emp.Deduction
result = emp.BaseSalary + emp.Bonus - emp.Deduction
```

### 3. Immediate Window
```csharp
// Test expressions trong runtime:
emp.BaseSalary + emp.Bonus
emp.Deduction
emp == null
```

### 4. Debug Test Cases

Để debug một test cụ thể:
```bash
# Debug specific test
dotnet test --filter "CalculateNetSalary_ValidEmployee_ReturnsCorrectAmount"
```

## Test Coverage Goals

🎯 **Target: >95% coverage cho logic chính**

- ✅ SalaryCalculator: 100% line coverage
- ✅ PayrollService: 100% line coverage
- ✅ Exception paths: Fully covered
- ✅ Edge cases: Comprehensive coverage

## Mocking Strategy

### IEmployeeRepository Mock
```csharp
// Setup mock return values
_mockRepository.Setup(repo => repo.GetById(It.IsAny<int>()))
              .Returns(employee);

// Verify method calls
_mockRepository.Verify(repo => repo.GetById(employeeId), Times.Once);
```

## Debugging Exercises

### Exercise 1: Inject Logic Bug
```csharp
// Thay đổi logic trong SalaryCalculator (có chủ ý)
return emp.BaseSalary + emp.Bonus + emp.Deduction; // Bug: + thay vì -
```

### Exercise 2: Debug Steps
1. Chạy tests → Thấy failures
2. Đặt breakpoint trong `CalculateNetSalary`
3. Step through code
4. Watch variables trong Watch Window
5. Identify bug trong Immediate Window
6. Fix và verify

## Best Practices

### Unit Testing
- ✅ Arrange-Act-Assert pattern
- ✅ Descriptive test names
- ✅ One assertion per test (when possible)
- ✅ Test both happy path và edge cases
- ✅ Use Theory tests cho multiple scenarios

### Mocking
- ✅ Mock external dependencies
- ✅ Verify interactions với dependencies
- ✅ Setup specific return values
- ✅ Test error scenarios với mock exceptions

### Coverage
- ✅ Aim for >95% line coverage
- ✅ Cover all exception paths
- ✅ Test boundary conditions
- ✅ Regular coverage monitoring

## Kết quả mong đợi

Sau khi chạy `dotnet test`, bạn sẽ thấy:
```
Test run for Day9.dll (.NETCoreApp,Version=v9.0)
Microsoft (R) Test Execution Command Line Tool Version 17.8.0

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    12, Skipped:     0, Total:    12
```

Và coverage report sẽ hiển thị >95% coverage cho tất cả classes chính.