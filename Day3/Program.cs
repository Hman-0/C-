using System;
using System.Collections.Generic;
using System.Linq;

// Class Book với các thuộc tính yêu cầu
public class Book
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public int Quantity { get; set; }

    public Book(int id, string title, string author, string genre, int quantity)
    {
        ID = id;
        Title = title;
        Author = author;
        Genre = genre;
        Quantity = quantity;
    }

    public override string ToString()
    {
        return $"ID: {ID}, Tên: {Title}, Tác giả: {Author}, Thể loại: {Genre}, Số lượng: {Quantity}";
    }
}

class Program
{
    // Danh sách các cuốn sách
    static List<Book> books = new List<Book>();
    
    // Lịch sử mượn/trả sách
    static Stack<string> history = new Stack<string>();
    
    // Quản lý người dùng mượn sách
    static Dictionary<string, List<int>> borrowers = new Dictionary<string, List<int>>();
    
    // Hàng chờ mượn sách
    static Queue<int> waitingQueue = new Queue<int>();
    
    // Danh sách thể loại sách (không trùng lặp)
    static HashSet<string> genres = new HashSet<string>();

    static void Main()
    {
        // Thêm một số sách mẫu
        ThemSachMau();
        
        while (true)
        {
            Console.WriteLine("\n===== HỆ THỐNG QUẢN LÝ KHO SÁCH THƯ VIỆN =====");
            Console.WriteLine("1. Thêm sách mới");
            Console.WriteLine("2. Tìm kiếm sách");
            Console.WriteLine("3. Phân loại sách theo thể loại");
            Console.WriteLine("4. Mượn sách");
            Console.WriteLine("5. Trả sách");
            Console.WriteLine("6. Hiển thị danh sách người mượn");
            Console.WriteLine("7. Hiển thị top 3 sách nhiều nhất");
            Console.WriteLine("8. Hiển thị lịch sử mượn/trả");
            Console.WriteLine("9. Hiển thị tất cả sách");
            Console.WriteLine("0. Thoát");
            Console.Write("Chọn chức năng: ");
            
            string choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1": ThemSach(); break;
                case "2": TimKiemSach(); break;
                case "3": ThongKeTheLoai(); break;
                case "4": XuLyMuonSach(); break;
                case "5": XuLyTraSach(); break;
                case "6": HienThiNguoiMuon(); break;
                case "7": HienThiTopSach(); break;
                case "8": HienThiLichSu(); break;
                case "9": HienThiTatCaSach(); break;
                case "0": return;
                default: Console.WriteLine("Lựa chọn không hợp lệ!"); break;
            }
        }
    }

    static void ThemSachMau()
    {
        books.Add(new Book(1, "Lập trình C# cơ bản", "Nguyễn Văn A", "Công nghệ", 5));
        books.Add(new Book(2, "Tôi thấy hoa vàng trên cỏ xanh", "Nguyễn Nhật Ánh", "Văn học", 3));
        books.Add(new Book(3, "Đắc nhân tâm", "Dale Carnegie", "Tâm lý", 7));
        books.Add(new Book(4, "Clean Code", "Robert Martin", "Công nghệ", 4));
        books.Add(new Book(5, "Sapiens", "Yuval Noah Harari", "Lịch sử", 2));
        
        // Cập nhật danh sách thể loại
        foreach (var book in books)
        {
            genres.Add(book.Genre);
        }
    }

    static void ThemSach()
    {
        Console.Write("Nhập ID sách: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID không hợp lệ!");
            return;
        }
        
        // Kiểm tra sách đã tồn tại
        var existingBook = books.FirstOrDefault(b => b.ID == id);
        if (existingBook != null)
        {
            Console.Write("Sách đã tồn tại. Nhập số lượng thêm: ");
            if (int.TryParse(Console.ReadLine(), out int addQuantity))
            {
                existingBook.Quantity += addQuantity;
                Console.WriteLine($"Đã tăng số lượng sách '{existingBook.Title}' thêm {addQuantity}. Tổng: {existingBook.Quantity}");
            }
            return;
        }
        
        Console.Write("Nhập tên sách: ");
        string title = Console.ReadLine();
        
        Console.Write("Nhập tác giả: ");
        string author = Console.ReadLine();
        
        Console.Write("Nhập thể loại: ");
        string genre = Console.ReadLine();
        
        Console.Write("Nhập số lượng: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            Console.WriteLine("Số lượng không hợp lệ!");
            return;
        }
        
        books.Add(new Book(id, title, author, genre, quantity));
        genres.Add(genre);
        Console.WriteLine("Đã thêm sách thành công!");
    }

    static void TimKiemSach()
    {
        Console.WriteLine("1. Tìm theo thể loại");
        Console.WriteLine("2. Tìm theo tác giả");
        Console.Write("Chọn cách tìm kiếm: ");
        
        string choice = Console.ReadLine();
        
        if (choice == "1")
        {
            Console.Write("Nhập thể loại: ");
            string genre = Console.ReadLine();
            
            var result = books.Where(b => b.Genre.ToLower().Contains(genre.ToLower()))
                             .Select(b => b)
                             .ToList();
            
            if (result.Any())
            {
                Console.WriteLine($"\nTìm thấy {result.Count} sách thuộc thể loại '{genre}':");
                foreach (var book in result)
                {
                    Console.WriteLine(book);
                }
            }
            else
            {
                Console.WriteLine($"Không tìm thấy sách nào thuộc thể loại '{genre}'");
            }
        }
        else if (choice == "2")
        {
            Console.Write("Nhập tên tác giả: ");
            string author = Console.ReadLine();
            
            var result = books.Where(b => b.Author.ToLower().Contains(author.ToLower()))
                             .Select(b => b)
                             .ToList();
            
            if (result.Any())
            {
                Console.WriteLine($"\nTìm thấy {result.Count} sách của tác giả '{author}':");
                foreach (var book in result)
                {
                    Console.WriteLine(book);
                }
            }
            else
            {
                Console.WriteLine($"Không tìm thấy sách nào của tác giả '{author}'");
            }
        }
        else
        {
            Console.WriteLine("Lựa chọn không hợp lệ!");
        }
    }

    static void ThongKeTheLoai()
    {
        var groupedBooks = books.GroupBy(b => b.Genre)
                               .Select(g => new { Genre = g.Key, TotalQuantity = g.Sum(b => b.Quantity) })
                               .OrderByDescending(x => x.TotalQuantity);
        
        Console.WriteLine("\n===== THỐNG KÊ SÁCH THEO THỂ LOẠI =====");
        foreach (var group in groupedBooks)
        {
            Console.WriteLine($"{group.Genre}: {group.TotalQuantity} cuốn");
        }
        
        Console.WriteLine($"\nTổng số thể loại: {genres.Count}");
        Console.WriteLine($"Các thể loại: {string.Join(", ", genres)}");
    }

    static void XuLyMuonSach()
    {
        Console.Write("Nhập ID sách muốn mượn: ");
        if (!int.TryParse(Console.ReadLine(), out int bookId))
        {
            Console.WriteLine("ID không hợp lệ!");
            return;
        }
        
        var book = books.FirstOrDefault(b => b.ID == bookId);
        if (book == null)
        {
            Console.WriteLine("Không tìm thấy sách!");
            return;
        }
        
        Console.Write("Nhập tên người mượn: ");
        string borrowerName = Console.ReadLine();
        
        if (book.Quantity > 0)
        {
            book.Quantity--;
            history.Push($"Mượn: {bookId}");
            
            // Cập nhật danh sách người mượn
            if (!borrowers.ContainsKey(borrowerName))
            {
                borrowers[borrowerName] = new List<int>();
            }
            borrowers[borrowerName].Add(bookId);
            
            Console.WriteLine($"Đã cho {borrowerName} mượn sách '{book.Title}'. Còn lại: {book.Quantity}");
        }
        else
        {
            waitingQueue.Enqueue(bookId);
            Console.WriteLine($"Sách '{book.Title}' đã hết. Đã thêm vào hàng chờ.");
        }
    }

    static void XuLyTraSach()
    {
        Console.Write("Nhập ID sách muốn trả: ");
        if (!int.TryParse(Console.ReadLine(), out int bookId))
        {
            Console.WriteLine("ID không hợp lệ!");
            return;
        }
        
        var book = books.FirstOrDefault(b => b.ID == bookId);
        if (book == null)
        {
            Console.WriteLine("Không tìm thấy sách!");
            return;
        }
        
        Console.Write("Nhập tên người trả: ");
        string borrowerName = Console.ReadLine();
        
        // Kiểm tra người này có mượn sách này không
        if (borrowers.ContainsKey(borrowerName) && borrowers[borrowerName].Contains(bookId))
        {
            book.Quantity++;
            history.Push($"Trả: {bookId}");
            borrowers[borrowerName].Remove(bookId);
            
            if (borrowers[borrowerName].Count == 0)
            {
                borrowers.Remove(borrowerName);
            }
            
            Console.WriteLine($"{borrowerName} đã trả sách '{book.Title}'. Số lượng hiện tại: {book.Quantity}");
            
            // Kiểm tra hàng chờ
            if (waitingQueue.Count > 0 && waitingQueue.Peek() == bookId)
            {
                waitingQueue.Dequeue();
                Console.WriteLine("Đã xử lý người đầu tiên trong hàng chờ.");
            }
        }
        else
        {
            Console.WriteLine($"{borrowerName} không mượn sách này!");
        }
    }

    static void HienThiNguoiMuon()
    {
        if (!borrowers.Any())
        {
            Console.WriteLine("Không có ai đang mượn sách.");
            return;
        }
        
        Console.WriteLine("\n===== DANH SÁCH NGƯỜI MƯỢN SÁCH =====");
        
        var borrowerInfo = from borrower in borrowers
                          from bookId in borrower.Value
                          join book in books on bookId equals book.ID
                          select new { BorrowerName = borrower.Key, Book = book };
        
        foreach (var info in borrowerInfo.GroupBy(x => x.BorrowerName))
        {
            Console.WriteLine($"\n{info.Key} đang mượn:");
            foreach (var item in info)
            {
                Console.WriteLine($"  - {item.Book.Title} (ID: {item.Book.ID})");
            }
        }
    }

    static void HienThiTopSach()
    {
        var topBooks = books.OrderByDescending(b => b.Quantity)
                           .Take(3)
                           .ToList();
        
        Console.WriteLine("\n===== TOP 3 SÁCH CÓ SỐ LƯỢNG NHIỀU NHẤT =====");
        for (int i = 0; i < topBooks.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {topBooks[i]}");
        }
    }

    static void HienThiLichSu()
    {
        if (history.Count == 0)
        {
            Console.WriteLine("Chưa có lịch sử mượn/trả.");
            return;
        }
        
        Console.WriteLine("\n===== LỊCH SỬ MƯỢN/TRẢ SÁCH (MỚI NHẤT TRƯỚC) =====");
        foreach (var entry in history)
        {
            Console.WriteLine(entry);
        }
    }

    static void HienThiTatCaSach()
    {
        if (!books.Any())
        {
            Console.WriteLine("Không có sách nào trong kho.");
            return;
        }
        
        Console.WriteLine("\n===== DANH SÁCH TẤT CẢ SÁCH =====");
        foreach (var book in books)
        {
            Console.WriteLine(book);
        }
    }
}
