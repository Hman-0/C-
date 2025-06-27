using Microsoft.EntityFrameworkCore;
using ToDoApp;
using Microsoft.Extensions.Configuration;
using ConsoleTables;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Build();

var optionsBuilder = new DbContextOptionsBuilder<ToDoContext>();
optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection")));

using var context = new ToDoContext(optionsBuilder.Options);

void ShowMenu()
{
    Console.WriteLine("\n==== TO-DO APP ====");
    Console.WriteLine("1. Thêm công việc");
    Console.WriteLine("2. Xem tất cả công việc");
    Console.WriteLine("3. Sửa công việc");
    Console.WriteLine("4. Xóa công việc");
    Console.WriteLine("5. Lọc công việc theo trạng thái");
    Console.WriteLine("6. Lọc công việc theo hạn");
    Console.WriteLine("7. Thống kê công việc hoàn thành theo tuần/tháng");
    Console.WriteLine("0. Thoát");
    Console.Write("Chọn: ");
}

void AddToDo()
{
    Console.Write("Nhập tiêu đề: ");
    var title = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(title)) { Console.WriteLine("Tiêu đề không được để trống!"); return; }
    Console.Write("Nhập hạn (yyyy-MM-dd): ");
    if (!DateTime.TryParse(Console.ReadLine(), out var dueDate)) { Console.WriteLine("Ngày không hợp lệ!"); return; }
    var todo = new ToDoItem { Title = title, DueDate = dueDate, IsCompleted = false };
    context.ToDoItems.Add(todo);
    context.SaveChanges();
    Console.WriteLine("Đã thêm!");
}

void ShowAll()
{
    var list = context.ToDoItems.OrderBy(x => x.DueDate).ToList();
    if (!list.Any()) { Console.WriteLine("Không có công việc nào!"); return; }
    var table = new ConsoleTable("ID", "Tiêu đề", "Hạn", "Hoàn thành");
    foreach (var t in list)
        table.AddRow(t.Id, t.Title, t.DueDate.ToString("yyyy-MM-dd"), t.IsCompleted ? "✔" : "✗");
    table.Write();
}

void EditToDo()
{
    ShowAll();
    Console.Write("Nhập ID công việc cần sửa: ");
    if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("ID không hợp lệ!"); return; }
    var todo = context.ToDoItems.Find(id);
    if (todo == null) { Console.WriteLine("Không tìm thấy!"); return; }
    Console.Write($"Tiêu đề ({todo.Title}): ");
    var title = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(title)) todo.Title = title;
    Console.Write($"Hạn ({todo.DueDate:yyyy-MM-dd}): ");
    var dueStr = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(dueStr) && DateTime.TryParse(dueStr, out var dueDate)) todo.DueDate = dueDate;
    Console.Write($"Hoàn thành (y/n, hiện tại: {(todo.IsCompleted ? "✔" : "✗")}): ");
    var done = Console.ReadLine();
    if (done?.ToLower() == "y") todo.IsCompleted = true;
    if (done?.ToLower() == "n") todo.IsCompleted = false;
    context.SaveChanges();
    Console.WriteLine("Đã cập nhật!");
}

void DeleteToDo()
{
    ShowAll();
    Console.Write("Nhập ID công việc cần xóa: ");
    if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("ID không hợp lệ!"); return; }
    var todo = context.ToDoItems.Find(id);
    if (todo == null) { Console.WriteLine("Không tìm thấy!"); return; }
    context.ToDoItems.Remove(todo);
    context.SaveChanges();
    Console.WriteLine("Đã xóa!");
}

void FilterByStatus()
{
    Console.Write("Chọn trạng thái (1: Hoàn thành, 0: Chưa): ");
    var input = Console.ReadLine();
    bool isCompleted = input == "1";
    var list = context.ToDoItems.Where(x => x.IsCompleted == isCompleted).ToList();
    if (!list.Any()) { Console.WriteLine("Không có công việc nào!"); return; }
    var table = new ConsoleTable("ID", "Tiêu đề", "Hạn", "Hoàn thành");
    foreach (var t in list)
        table.AddRow(t.Id, t.Title, t.DueDate.ToString("yyyy-MM-dd"), t.IsCompleted ? "✔" : "✗");
    table.Write();
}

void FilterByDueDate()
{
    Console.Write("Nhập hạn cần lọc (yyyy-MM-dd): ");
    if (!DateTime.TryParse(Console.ReadLine(), out var dueDate)) { Console.WriteLine("Ngày không hợp lệ!"); return; }
    var list = context.ToDoItems.Where(x => x.DueDate.Date == dueDate.Date).ToList();
    if (!list.Any()) { Console.WriteLine("Không có công việc nào!"); return; }
    var table = new ConsoleTable("ID", "Tiêu đề", "Hạn", "Hoàn thành");
    foreach (var t in list)
        table.AddRow(t.Id, t.Title, t.DueDate.ToString("yyyy-MM-dd"), t.IsCompleted ? "✔" : "✗");
    table.Write();
}

void Stats()
{
    var now = DateTime.Now;
    var weekStart = now.AddDays(-(int)now.DayOfWeek);
    var monthStart = new DateTime(now.Year, now.Month, 1);
    var weekCount = context.ToDoItems.Count(x => x.IsCompleted && x.DueDate >= weekStart && x.DueDate <= now);
    var monthCount = context.ToDoItems.Count(x => x.IsCompleted && x.DueDate >= monthStart && x.DueDate <= now);
    Console.WriteLine($"Số công việc hoàn thành tuần này: {weekCount}");
    Console.WriteLine($"Số công việc hoàn thành tháng này: {monthCount}");
}

while (true)
{
    ShowMenu();
    var choice = Console.ReadLine();
    switch (choice)
    {
        case "1": AddToDo(); break;
        case "2": ShowAll(); break;
        case "3": EditToDo(); break;
        case "4": DeleteToDo(); break;
        case "5": FilterByStatus(); break;
        case "6": FilterByDueDate(); break;
        case "7": Stats(); break;
        case "0": return;
        default: Console.WriteLine("Chọn không hợp lệ!"); break;
    }
}
