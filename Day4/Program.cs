using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// 1. Delegate và Event
public delegate void OrderStatusChangedHandler(Order order, string newStatus);

// 4. EventArgs tùy chỉnh
public class OrderEventArgs : EventArgs
{
    public Order Order { get; set; }
    public string NewStatus { get; set; }
    public string OldStatus { get; set; }
    public DateTime Timestamp { get; set; }

    public OrderEventArgs(Order order, string newStatus, string oldStatus)
    {
        Order = order;
        NewStatus = newStatus;
        OldStatus = oldStatus;
        Timestamp = DateTime.Now;
    }
}

// Class Order với events
public class Order
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public string FoodItems { get; set; }
    public string Status { get; private set; }
    public DateTime OrderTime { get; set; }
    public decimal TotalAmount { get; set; }

    // Event sử dụng delegate tùy chỉnh
    public event OrderStatusChangedHandler StatusChanged;
    
    // Event sử dụng EventHandler chuẩn
    public event EventHandler<OrderEventArgs> StatusChangedStandard;

    public Order(int orderId, string customerName, string foodItems, decimal totalAmount)
    {
        OrderId = orderId;
        CustomerName = customerName;
        FoodItems = foodItems;
        TotalAmount = totalAmount;
        Status = "Đã tạo";
        OrderTime = DateTime.Now;
    }

    public void UpdateStatus(string newStatus)
    {
        string oldStatus = Status;
        Status = newStatus;
        
        // Raise events
        StatusChanged?.Invoke(this, newStatus);
        StatusChangedStandard?.Invoke(this, new OrderEventArgs(this, newStatus, oldStatus));
    }

    public override string ToString()
    {
        return $"Đơn #{OrderId} - {CustomerName} - {FoodItems} - {Status} - {TotalAmount:C}";
    }
}

// 5. Observer pattern - Kitchen
public class Kitchen
{
    public string Name { get; set; }

    public Kitchen(string name)
    {
        Name = name;
    }

    public void Subscribe(Order order)
    {
        order.StatusChangedStandard += OnOrderStatusChanged;
    }

    private void OnOrderStatusChanged(object sender, OrderEventArgs e)
    {
        if (e.NewStatus == "Đã tạo")
        {
            Console.WriteLine($"[BẾP {Name}] Nhận đơn mới #{e.Order.OrderId}: {e.Order.FoodItems}");
            Console.WriteLine($"[BếP {Name}] Bắt đầu chuẩn bị món ăn...");
        }
        else if (e.NewStatus == "Đang chuẩn bị")
        {
            Console.WriteLine($"[BẾP {Name}] Đang nấu đơn #{e.Order.OrderId}...");
        }
        else if (e.NewStatus == "Sẵn sàng")
        {
            Console.WriteLine($"[BếP {Name}] Hoàn thành đơn #{e.Order.OrderId}. Sẵn sàng giao hàng!");
        }
    }
}

// 5. Observer pattern - Delivery
public class Delivery
{
    public string ShipperName { get; set; }

    public Delivery(string shipperName)
    {
        ShipperName = shipperName;
    }

    public void Subscribe(Order order)
    {
        order.StatusChangedStandard += OnOrderStatusChanged;
    }

    private void OnOrderStatusChanged(object sender, OrderEventArgs e)
    {
        if (e.NewStatus == "Sẵn sàng")
        {
            Console.WriteLine($"[SHIPPER {ShipperName}] Nhận đơn #{e.Order.OrderId} để giao hàng");
        }
        else if (e.NewStatus == "Đang giao")
        {
            Console.WriteLine($"[SHIPPER {ShipperName}] Đang giao đơn #{e.Order.OrderId} đến {e.Order.CustomerName}");
        }
        else if (e.NewStatus == "Hoàn tất")
        {
            Console.WriteLine($"[SHIPPER {ShipperName}] Đã giao thành công đơn #{e.Order.OrderId}!");
        }
        else if (e.NewStatus == "Giao thất bại")
        {
            Console.WriteLine($"[SHIPPER {ShipperName}] Giao hàng thất bại đơn #{e.Order.OrderId}. Liên hệ khách hàng.");
        }
    }
}

// 5. Observer pattern - Customer Service
public class CustomerService
{
    public string AgentName { get; set; }

    public CustomerService(string agentName)
    {
        AgentName = agentName;
    }

    public void Subscribe(Order order)
    {
        order.StatusChangedStandard += OnOrderStatusChanged;
    }

    private void OnOrderStatusChanged(object sender, OrderEventArgs e)
    {
        if (e.NewStatus == "Hủy")
        {
            Console.WriteLine($"[CSKH {AgentName}] Đơn #{e.Order.OrderId} đã bị hủy. Liên hệ khách hàng {e.Order.CustomerName} để xử lý hoàn tiền.");
        }
        else if (e.NewStatus == "Giao thất bại")
        {
            Console.WriteLine($"[CSKH {AgentName}] Đơn #{e.Order.OrderId} giao thất bại. Liên hệ {e.Order.CustomerName} để sắp xếp lại.");
        }
        else if (e.NewStatus == "Khiếu nại")
        {
            Console.WriteLine($"[CSKH {AgentName}] Nhận khiếu nại từ đơn #{e.Order.OrderId}. Xử lý ngay!");
        }
    }
}

// Logger để ghi file
public class OrderLogger
{
    private string logFilePath;

    public OrderLogger(string filePath = "order_log.txt")
    {
        logFilePath = filePath;
    }

    public void Subscribe(Order order)
    {
        order.StatusChangedStandard += LogOrderStatus;
    }

    private void LogOrderStatus(object sender, OrderEventArgs e)
    {
        string logEntry = $"[{e.Timestamp:yyyy-MM-dd HH:mm:ss}] Đơn #{e.Order.OrderId} - {e.OldStatus} → {e.NewStatus} - Khách: {e.Order.CustomerName}";
        
        try
        {
            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi ghi log: {ex.Message}");
        }
    }
}

class Program
{
    // 2. Func, Action, Predicate
    static Predicate<Order> isDelivering = order => order.Status == "Đang giao";
    static Func<Order, string> orderToString = order => $"#{order.OrderId}: {order.CustomerName} - {order.Status}";
    static Action<string> logToConsole = message => Console.WriteLine($"[LOG] {message}");

    static List<Order> orders = new List<Order>();
    static Kitchen kitchen = new Kitchen("Bếp Chính");
    static Delivery delivery = new Delivery("Nguyễn Văn A");
    static CustomerService customerService = new CustomerService("Trần Thị B");
    static OrderLogger logger = new OrderLogger();

    static void Main()
    {
        Console.WriteLine("===== HỆ THỐNG GIÁM SÁT ĐƠN HÀNG GIAO ĐỒ ĂN =====");
        
        // Tạo đơn hàng mẫu
        CreateSampleOrders();
        
        // Demo thay đổi trạng thái
        SimulateOrderProcessing();
        
        // Thống kê với LINQ + Predicate
        ShowStatistics();
        
        Console.WriteLine("\nNhấn phím bất kỳ để thoát...");
        Console.ReadKey();
    }

    static void CreateSampleOrders()
    {
        var order1 = new Order(1001, "Nguyễn Văn C", "Phở bò, Chả cá", 150000);
        var order2 = new Order(1002, "Trần Thị D", "Cơm gà, Trà sữa", 120000);
        var order3 = new Order(1003, "Lê Văn E", "Bánh mì, Cà phê", 50000);
        
        orders.AddRange(new[] { order1, order2, order3 });
        
        // Đăng ký observers cho tất cả đơn hàng
        foreach (var order in orders)
        {
            kitchen.Subscribe(order);
            delivery.Subscribe(order);
            customerService.Subscribe(order);
            logger.Subscribe(order);
            
            // 3. Anonymous method và Lambda
            order.StatusChanged += (o, status) => 
            {
                logToConsole($"Đơn #{o.OrderId} chuyển sang: {status}");
            };
            
            // Lambda với EventHandler
            order.StatusChangedStandard += (sender, e) => 
            {
                Console.WriteLine($"[SYSTEM] {orderToString(e.Order)} lúc {e.Timestamp:HH:mm:ss}");
            };
        }
    }

    static void SimulateOrderProcessing()
    {
        Console.WriteLine("\n===== MÔ PHỎNG XỬ LÝ ĐƠN HÀNG =====");
        
        // Đơn 1: Quy trình bình thường
        Console.WriteLine("\n--- Đơn hàng #1001 ---");
        orders[0].UpdateStatus("Đang chuẩn bị");
        System.Threading.Thread.Sleep(1000);
        orders[0].UpdateStatus("Sẵn sàng");
        System.Threading.Thread.Sleep(1000);
        orders[0].UpdateStatus("Đang giao");
        System.Threading.Thread.Sleep(1000);
        orders[0].UpdateStatus("Hoàn tất");
        
        // Đơn 2: Giao thất bại
        Console.WriteLine("\n--- Đơn hàng #1002 ---");
        orders[1].UpdateStatus("Đang chuẩn bị");
        System.Threading.Thread.Sleep(1000);
        orders[1].UpdateStatus("Sẵn sàng");
        System.Threading.Thread.Sleep(1000);
        orders[1].UpdateStatus("Đang giao");
        System.Threading.Thread.Sleep(1000);
        orders[1].UpdateStatus("Giao thất bại");
        
        // Đơn 3: Bị hủy
        Console.WriteLine("\n--- Đơn hàng #1003 ---");
        orders[2].UpdateStatus("Đang chuẩn bị");
        System.Threading.Thread.Sleep(1000);
        orders[2].UpdateStatus("Hủy");
    }

    static void ShowStatistics()
    {
        Console.WriteLine("\n===== THỐNG KÊ =====");
        
        // Sử dụng Predicate
        var deliveringOrders = orders.Where(o => isDelivering(o)).ToList();
        Console.WriteLine($"Số đơn đang giao: {deliveringOrders.Count}");
        
        // Đếm theo trạng thái với LINQ
        var completedCount = orders.Count(o => o.Status == "Hoàn tất");
        var cancelledCount = orders.Count(o => o.Status == "Hủy");
        var failedCount = orders.Count(o => o.Status == "Giao thất bại");
        
        Console.WriteLine($"Đơn hoàn tất: {completedCount}");
        Console.WriteLine($"Đơn bị hủy: {cancelledCount}");
        Console.WriteLine($"Đơn giao thất bại: {failedCount}");
        
        // Tổng doanh thu đơn hoàn tất
        var totalRevenue = orders.Where(o => o.Status == "Hoàn tất")
                                .Sum(o => o.TotalAmount);
        Console.WriteLine($"Tổng doanh thu: {totalRevenue:C}");
        
        // Hiển thị tất cả đơn hàng
        Console.WriteLine("\nTất cả đơn hàng:");
        foreach (var order in orders)
        {
            Console.WriteLine($"  {orderToString(order)}");
        }
    }
}
