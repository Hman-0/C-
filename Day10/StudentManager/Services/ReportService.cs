using iTextSharp.text;
using iTextSharp.text.pdf;
using StudentManager.Models;
using System.IO;
using System.Text;

namespace StudentManager.Services
{
    public class ReportService
    {
        public async Task ExportStudentsToPdf(List<Student> students)
        {
            var fileName = $"DanhSachSinhVien_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, stream);
                
                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.BLACK);
                var title = new Paragraph("DANH SÁCH SINH VIÊN", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);

                // Date
                var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);
                var date = new Paragraph($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}", dateFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 20
                };
                document.Add(date);

                // Table
                var table = new PdfPTable(6) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 1, 2, 1.5f, 2, 1.5f, 1 });

                // Header
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                var headers = new[] { "STT", "Họ tên", "Mã SV", "Email", "Lớp", "Điểm" };
                
                foreach (var header in headers)
                {
                    var cell = new PdfPCell(new Phrase(header, headerFont))
                    {
                        BackgroundColor = BaseColor.DARK_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 8
                    };
                    table.AddCell(cell);
                }

                // Data
                var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);
                for (int i = 0; i < students.Count; i++)
                {
                    var student = students[i];
                    
                    table.AddCell(new PdfPCell(new Phrase((i + 1).ToString(), dataFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(student.Name, dataFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(student.StudentCode, dataFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(student.Email, dataFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(student.Class, dataFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(student.Grade?.ToString("F1") ?? "N/A", dataFont)) { Padding = 5 });
                }

                document.Add(table);

                // Summary
                var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK);
                var summary = new Paragraph($"\nTổng số sinh viên: {students.Count}", summaryFont)
                {
                    SpacingBefore = 20
                };
                document.Add(summary);

                document.Close();
            }

            await Task.CompletedTask;
        }

        public async Task ExportStudentsToCsv(List<Student> students)
        {
            var fileName = $"DanhSachSinhVien_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            var csv = new StringBuilder();
            
            // Header
            csv.AppendLine("STT,Họ tên,Mã sinh viên,Email,Số điện thoại,Lớp,Điểm,Ngày tạo");

            // Data
            for (int i = 0; i < students.Count; i++)
            {
                var student = students[i];
                csv.AppendLine($"{i + 1},\"{student.Name}\",{student.StudentCode},\"{student.Email}\",\"{student.Phone}\",{student.Class},{student.Grade?.ToString("F1") ?? "N/A"},{student.CreatedDate:dd/MM/yyyy}");
            }

            await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);
        }

        public async Task ExportOrderReportToPdf(Student student, List<Order> orders)
        {
            var fileName = $"BaoCaoDonHang_{student.StudentCode}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, stream);
                
                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.BLACK);
                var title = new Paragraph("BÁO CÁO ĐƠN HÀNG", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);

                // Student Info
                var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                var studentInfo = new Paragraph($"Sinh viên: {student.Name} ({student.StudentCode})\nLớp: {student.Class}\nEmail: {student.Email}", infoFont)
                {
                    SpacingAfter = 20
                };
                document.Add(studentInfo);

                // Date
                var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);
                var date = new Paragraph($"Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm}", dateFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 20
                };
                document.Add(date);

                if (orders.Any())
                {
                    // Table
                    var table = new PdfPTable(6) { WidthPercentage = 100 };
                    table.SetWidths(new float[] { 1, 1.5f, 2, 1, 1.5f, 1.5f });

                    // Header
                    var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                    var headers = new[] { "STT", "Mã ĐH", "Sản phẩm", "SL", "Đơn giá", "Thành tiền" };
                    
                    foreach (var header in headers)
                    {
                        var cell = new PdfPCell(new Phrase(header, headerFont))
                        {
                            BackgroundColor = BaseColor.DARK_GRAY,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 8
                        };
                        table.AddCell(cell);
                    }

                    // Data
                    var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);
                    decimal totalAmount = 0;
                    
                    for (int i = 0; i < orders.Count; i++)
                    {
                        var order = orders[i];
                        totalAmount += order.TotalAmount;
                        
                        table.AddCell(new PdfPCell(new Phrase((i + 1).ToString(), dataFont)) { Padding = 5 });
                        table.AddCell(new PdfPCell(new Phrase(order.OrderCode, dataFont)) { Padding = 5 });
                        table.AddCell(new PdfPCell(new Phrase(order.ProductName, dataFont)) { Padding = 5 });
                        table.AddCell(new PdfPCell(new Phrase(order.Quantity.ToString(), dataFont)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                        table.AddCell(new PdfPCell(new Phrase(order.Price.ToString("#,##0"), dataFont)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                        table.AddCell(new PdfPCell(new Phrase(order.TotalAmount.ToString("#,##0"), dataFont)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                    }

                    document.Add(table);

                    // Summary
                    var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                    var summary = new Paragraph($"\nTổng số đơn hàng: {orders.Count}\nTổng tiền: {totalAmount:#,##0} VNĐ", summaryFont)
                    {
                        SpacingBefore = 20
                    };
                    document.Add(summary);
                }
                else
                {
                    var noDataFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 12, BaseColor.GRAY);
                    var noData = new Paragraph("Sinh viên này chưa có đơn hàng nào.", noDataFont)
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingBefore = 50
                    };
                    document.Add(noData);
                }

                document.Close();
            }

            await Task.CompletedTask;
        }
    }
}