# Hướng dẫn Debug chi tiết cho Hệ thống tính lương

## 🎯 Mục tiêu Debugging

1. Học cách đặt breakpoints hiệu quả
2. Sử dụng Watch Window và Immediate Window
3. Debug unit tests
4. Tìm và sửa lỗi logic

## 🔧 Chuẩn bị Debug Environment

### Visual Studio
1. Mở project trong Visual Studio
2. Đảm bảo build configuration là **Debug** (không phải Release)
3. Enable "Just My Code" trong Tools → Options → Debugging

### VS Code
1. Cài đặt C# extension
2. Tạo `.vscode/launch.json` configuration

## 🐛 Exercise 1: Debug Logic Bug

### Bước 1: Inject Bug

Mở file `Services/SalaryCalculator.cs` và thay đổi dòng:
```csharp
// Từ:
return emp.BaseSalary + emp.Bonus - emp.Deduction;

// Thành (có bug):
return emp.BaseSalary + emp.Bonus + emp.Deduction;
```

### Bước 2: Chạy Test và Quan sát Failure
```bash
dotnet test
```

Bạn sẽ thấy nhiều tests fail với message tương tự:
```
Expected: 5500
Actual: 6000
```

### Bước 3: Debug Test Case

1. **Đặt Breakpoint:**
   - Mở `Tests/SalaryCalculatorTests.cs`
   - Đặt breakpoint tại dòng `var result = _calculator.CalculateNetSalary(employee);`
   - Đặt breakpoint trong `SalaryCalculator.CalculateNetSalary()` tại dòng return

2. **Debug Test:**
   - Right-click trên test method `CalculateNetSalary_ValidEmployee_ReturnsCorrectAmount`
   - Chọn "Debug Test"

3. **Inspect Variables:**
   - Khi dừng tại breakpoint, hover over `employee` để xem properties
   - Mở **Watch Window** (Debug → Windows → Watch)
   - Add các expressions:
     ```
     emp.BaseSalary
     emp.Bonus
     emp.Deduction
     emp.BaseSalary + emp.Bonus - emp.Deduction
     emp.BaseSalary + emp.Bonus + emp.Deduction
     ```

4. **Immediate Window:**
   - Mở **Immediate Window** (Debug → Windows → Immediate)
   - Test expressions:
     ```csharp
     emp.BaseSalary
     emp.Bonus
     emp.Deduction
     5000 + 1000 - 500
     5000 + 1000 + 500
     ```

### Bước 4: Identify Bug

Trong Watch Window, bạn sẽ thấy:
- `emp.BaseSalary + emp.Bonus - emp.Deduction` = 5500 (đúng)
- `emp.BaseSalary + emp.Bonus + emp.Deduction` = 6000 (sai)

### Bước 5: Fix Bug

Sửa lại logic đúng:
```csharp
return emp.BaseSalary + emp.Bonus - emp.Deduction;
```

### Bước 6: Verify Fix
```bash
dotnet test
```

## 🔍 Exercise 2: Debug PayrollService

### Scenario: Employee Not Found

1. **Đặt Breakpoint:**
   - Trong `PayrollService.GetNetSalary()`
   - Tại dòng `var emp = _repo.GetById(employeeId);`
   - Tại dòng `if (emp == null)`

2. **Debug Test:**
   - Debug test `GetNetSalary_RepositoryReturnsNull_ThrowsArgumentException`

3. **Watch Variables:**
   ```
   employeeId
   emp
   emp == null
   ```

4. **Step Through Code:**
   - F10 (Step Over) để đi qua từng dòng
   - F11 (Step Into) để vào method calls
   - F5 (Continue) để chạy đến breakpoint tiếp theo

## 🧪 Exercise 3: Debug với Mock Objects

### Setup Mock Debugging

1. **Đặt Breakpoint trong Test:**
   ```csharp
   // Trong PayrollServiceTests
   _mockRepository.Setup(repo => repo.GetById(employeeId))
                 .Returns(employee); // <- Breakpoint here
   
   var result = _payrollService.GetNetSalary(employeeId); // <- Breakpoint here
   ```

2. **Verify Mock Behavior:**
   - Watch `_mockRepository.Object`
   - Immediate Window: `_mockRepository.Verify(repo => repo.GetById(It.IsAny<int>()), Times.Once)`

## 📊 Advanced Debugging Techniques

### 1. Conditional Breakpoints

Right-click breakpoint → Conditions:
```csharp
// Chỉ dừng khi BaseSalary > 5000
emp.BaseSalary > 5000

// Chỉ dừng khi employeeId = 999
employeeId == 999
```

### 2. Tracepoints (Logpoints)

Right-click breakpoint → Actions:
```
Employee ID: {employeeId}, Salary: {emp?.BaseSalary}
```

### 3. Call Stack Analysis

- Mở **Call Stack Window** (Debug → Windows → Call Stack)
- Double-click để navigate giữa các method calls
- Xem parameter values tại mỗi level

### 4. Exception Settings

- Debug → Windows → Exception Settings
- Enable "Common Language Runtime Exceptions"
- Break khi exception được thrown (không chỉ unhandled)

## 🎯 Debugging Checklist

### Trước khi Debug:
- [ ] Build project thành công
- [ ] Identify failing test case
- [ ] Understand expected vs actual behavior

### Trong quá trình Debug:
- [ ] Đặt breakpoints tại key locations
- [ ] Use Watch Window cho important variables
- [ ] Step through code systematically
- [ ] Verify assumptions với Immediate Window

### Sau khi Fix:
- [ ] Run all tests
- [ ] Verify fix không break other functionality
- [ ] Update tests nếu cần
- [ ] Document lesson learned

## 🚀 Pro Tips

1. **Debug Tests, Not Just Code:**
   - Tests cũng có thể có bugs
   - Debug test setup và assertions

2. **Use Data Tips:**
   - Hover over variables để xem values
   - Pin data tips cho variables quan trọng

3. **Parallel Debugging:**
   - Debug multiple test cases
   - Compare behavior giữa passing và failing tests

4. **Mock Verification:**
   - Always verify mock interactions
   - Check Times.Once, Times.Never, etc.

5. **Exception Debugging:**
   - Enable first-chance exceptions
   - Debug exception constructors
   - Verify exception messages

## 📝 Common Debugging Scenarios

### Scenario 1: Null Reference Exception
```csharp
// Debug steps:
1. Identify which object is null
2. Trace back to where it should be initialized
3. Check mock setups
4. Verify dependency injection
```

### Scenario 2: Wrong Calculation Result
```csharp
// Debug steps:
1. Break at calculation point
2. Verify input values
3. Step through calculation logic
4. Compare with expected formula
```

### Scenario 3: Mock Not Working
```csharp
// Debug steps:
1. Verify mock setup syntax
2. Check parameter matching (It.IsAny vs specific values)
3. Verify mock object is being used
4. Check Times verification
```

## 🎓 Learning Outcomes

Sau khi hoàn thành exercises này, bạn sẽ:

✅ Biết cách đặt và sử dụng breakpoints hiệu quả
✅ Thành thạo Watch Window và Immediate Window
✅ Debug unit tests và mock objects
✅ Identify và fix logic bugs
✅ Understand call stack và exception flow
✅ Apply debugging best practices

Hãy practice với các scenarios khác nhau để master debugging skills!