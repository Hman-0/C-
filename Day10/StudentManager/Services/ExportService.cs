using CsvHelper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using StudentManager.Models;
using System.Globalization;
using System.Text;

namespace StudentManager.Services
{
    public class ExportService
    {
        public async Task<bool> ExportToPdfAsync(List<Student> students, string filePath)
        {
            try
            {
                using var fileStream = new FileStream(filePath, FileMode.Create);
                var document = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, fileStream);
                
                document.Open();
                
                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var title = new Paragraph("DANH SÁCH SINH VIÊN", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);
                
                // Export info
                var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var exportInfo = new Paragraph($"Xuất ngày: {DateTime.Now:dd/MM/yyyy HH:mm} | Tổng số sinh viên: {students.Count}", infoFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 15
                };
                document.Add(exportInfo);
                
                // Table
                var table = new PdfPTable(7) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 1f, 2.5f, 1.5f, 2f, 1.5f, 1f, 1.5f });
                
                // Header
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var headers = new[] { "STT", "Họ và Tên", "Mã SV", "Email", "Lớp", "GPA", "Ngày sinh" };
                
                foreach (var header in headers)
                {
                    var cell = new PdfPCell(new Phrase(header, headerFont))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 5
                    };
                    table.AddCell(cell);
                }
                
                // Data rows
                var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                for (int i = 0; i < students.Count; i++)
                {
                    var student = students[i];
                    
                    table.AddCell(new PdfPCell(new Phrase((i + 1).ToString(), dataFont)) { Padding = 3, HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(student.FullName, dataFont)) { Padding = 3 });
                    table.AddCell(new PdfPCell(new Phrase(student.StudentCode, dataFont)) { Padding = 3, HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(student.Email, dataFont)) { Padding = 3 });
                    table.AddCell(new PdfPCell(new Phrase(student.Class, dataFont)) { Padding = 3, HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(student.GPA.ToString("F2"), dataFont)) { Padding = 3, HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(student.DateOfBirth.ToString("dd/MM/yyyy"), dataFont)) { Padding = 3, HorizontalAlignment = Element.ALIGN_CENTER });
                }
                
                document.Add(table);
                document.Close();
                
                Console.WriteLine($"Exported PDF successfully to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting to PDF: {ex.Message}");
                return false;
            }
        }
        
        public async Task<bool> ExportToCsvAsync(List<Student> students, string filePath)
        {
            try
            {
                using var writer = new StringWriter();
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                
                // Write headers
                csv.WriteField("STT");
                csv.WriteField("Họ và Tên");
                csv.WriteField("Mã Sinh Viên");
                csv.WriteField("Email");
                csv.WriteField("Số Điện Thoại");
                csv.WriteField("Lớp");
                csv.WriteField("GPA");
                csv.WriteField("Ngày Sinh");
                csv.WriteField("Ngày Tạo");
                csv.NextRecord();
                
                // Write data
                for (int i = 0; i < students.Count; i++)
                {
                    var student = students[i];
                    
                    csv.WriteField(i + 1);
                    csv.WriteField(student.FullName);
                    csv.WriteField(student.StudentCode);
                    csv.WriteField(student.Email);
                    csv.WriteField(student.PhoneNumber ?? "");
                    csv.WriteField(student.Class);
                    csv.WriteField(student.GPA.ToString("F2"));
                    csv.WriteField(student.DateOfBirth.ToString("dd/MM/yyyy"));
                    csv.WriteField(student.CreatedAt.ToString("dd/MM/yyyy HH:mm"));
                    csv.NextRecord();
                }
                
                // Write to file with UTF-8 BOM for Excel compatibility
                var csvContent = writer.ToString();
                var utf8WithBom = new UTF8Encoding(true);
                await File.WriteAllTextAsync(filePath, csvContent, utf8WithBom);
                
                Console.WriteLine($"Exported CSV successfully to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting to CSV: {ex.Message}");
                return false;
            }
        }
    }
}