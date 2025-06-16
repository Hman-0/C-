using System;

class Program
{
    // Mảng lưu trữ thông tin các loại bánh (tên và giá tương ứng)
    static string[] tenBanh = { "Bánh mì", "Bánh bông lan", "Bánh kem", "Bánh croissant", "Bánh donut", "Bánh pizza" };
    static double[] giaBanh = { 15000, 25000, 120000, 35000, 20000, 80000 };
    
    // Các biến theo dõi số liệu thống kê của cửa hàng
    static int tongSoDonHang = 0;      
    static double tongDoanhThu = 0;    
    static int soDonLon = 0;           
    static int soDonThuong = 0;    

    static void Main()
    {
        // Hiển thị giao diện chào mừng và hướng dẫn sử dụng
        Console.WriteLine("=== CHÀO MỪNG ĐÉN TIỆM BÁNH NGỌT ===");
        Console.WriteLine("Hệ thống quản lý đơn hàng mini");
        Console.WriteLine("Hướng dẫn sử dụng:");
        Console.WriteLine("- Chọn loại bánh theo số thứ tự");
        Console.WriteLine("- Nhập số lượng muốn mua");
        Console.WriteLine("- Nhập 'exit' để thoát chương trình");
        Console.WriteLine("=====================================\n");

        // Vòng lặp xử lý đơn hàng liên tục cho đến khi người dùng chọn thoát
        while (true)
        {
            // Hiển thị menu bánh cho khách hàng lựa chọn
            HienThiDanhSachBanh();

            // Nhận lựa chọn bánh từ người dùng
            Console.Write("Nhập số thứ tự bánh muốn mua (hoặc 'exit' để thoát): ");
            string luaChon = Console.ReadLine();

            // Kiểm tra nếu người dùng muốn thoát chương trình
            if (luaChon.ToLower() == "exit")
            {
                break;
            }

            // Kiểm tra tính hợp lệ của số thứ tự bánh được chọn
            if (!int.TryParse(luaChon, out int soBanh) || soBanh < 1 || soBanh > tenBanh.Length)
            {
                Console.WriteLine("Lựa chọn không hợp lệ! Vui lòng thử lại.\n");
                continue;
            }

            // Nhận và kiểm tra số lượng bánh cần mua
            Console.Write("Nhập số lượng: ");
            string soLuongStr = Console.ReadLine();

            if (!int.TryParse(soLuongStr, out int soLuong) || soLuong <= 0)
            {
                Console.WriteLine("Số lượng không hợp lệ! Vui lòng thử lại.\n");
                continue;
            }

            // Xử lý thông tin đơn hàng
            XuLyDonHang(soBanh - 1, soLuong);

            Console.WriteLine();
        }

        // Hiển thị báo cáo thống kê khi kết thúc phiên làm việc
        HienThiThongKe();
        Console.WriteLine("\nCảm ơn bạn đã sử dụng hệ thống!");
        Console.WriteLine("Nhấn phím bất kỳ để thoát...");
        Console.ReadKey();
    }

    // Hiển thị danh sách bánh kèm giá
    static void HienThiDanhSachBanh()
    {
        Console.WriteLine("DANH SÁCH BÁNH:");
        for (int i = 0; i < tenBanh.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {tenBanh[i]} - {giaBanh[i]:N0} VNĐ");
        }
        Console.WriteLine();
    }

    // Xử lý thông tin một đơn hàng mới
    static void XuLyDonHang(int indexBanh, int soLuong)
    {
        string tenBanhDaChon = tenBanh[indexBanh];
        
        // Tính tổng tiền cho đơn hàng
        double tongTien = TinhTien(tenBanhDaChon, soLuong);
        
        // Phân loại đơn hàng dựa trên giá trị
        string loaiDon = XepLoaiDon(tongTien);
        
        // Hiển thị chi tiết đơn hàng
        HienThiThongTin(tenBanhDaChon, soLuong, tongTien, loaiDon);
        
        // Cập nhật số liệu thống kê
        CapNhatThongKe(tongTien, loaiDon);
    }

    // Tính tổng tiền dựa trên tên bánh và số lượng
    static double TinhTien(string tenBanh, int soLuong)
    {
        // Tìm giá bánh tương ứng trong danh sách
        for (int i = 0; i < Program.tenBanh.Length; i++)
        {
            if (Program.tenBanh[i] == tenBanh)
            {
                return giaBanh[i] * soLuong;
            }
        }
        return 0;
    }

    // Tính tổng tiền dựa trên giá và số lượng (phương thức nạp chồng)
    static double TinhTien(double gia, int soLuong)
    {
        return gia * soLuong;
    }

    // Phân loại đơn hàng dựa trên tổng giá trị
    static string XepLoaiDon(double tongTien)
    {
        bool laDonLon = tongTien > 100000;
        
        if (laDonLon)
            return "Đơn lớn";
        else
            return "Đơn thường";
    }

    // In thông tin chi tiết của đơn hàng
    static void HienThiThongTin(string tenBanh, int soLuong, double tongTien, string loaiDon)
    {
        Console.WriteLine("--- THÔNG TIN ĐƠN HÀNG ---");
        Console.WriteLine($"Bánh: {tenBanh}");
        Console.WriteLine($"Số lượng: {soLuong}");
        Console.WriteLine($"Tổng tiền: {tongTien:N0} VNĐ");
        Console.WriteLine($"Loại đơn: {loaiDon}");
        Console.WriteLine("-------------------------");
    }

    // Cập nhật các số liệu thống kê sau mỗi đơn hàng
    static void CapNhatThongKe(double tongTien, string loaiDon)
    {
        tongSoDonHang++;
        tongDoanhThu += tongTien;
        
        if (loaiDon == "Đơn lớn")
            soDonLon++;
        else
            soDonThuong++;
    }

    // Hiển thị báo cáo thống kê tổng hợp
    static void HienThiThongKe()
    {
        Console.WriteLine("\n=== THỐNG KÊ TRONG NGÀY ===");
        Console.WriteLine($"Tổng số đơn hàng: {tongSoDonHang}");
        Console.WriteLine($"Tổng doanh thu: {tongDoanhThu:N0} VNĐ");
        Console.WriteLine($"Số đơn lớn: {soDonLon}");
        Console.WriteLine($"Số đơn thường: {soDonThuong}");
        Console.WriteLine("============================");
    }
}