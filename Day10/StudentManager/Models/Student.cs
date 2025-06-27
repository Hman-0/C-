using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManager.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sinh viên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã sinh viên là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã sinh viên không được vượt quá 20 ký tự")]
        public string StudentCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Lớp là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên lớp không được vượt quá 50 ký tự")]
        public string Class { get; set; } = string.Empty;

        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        public double? Grade { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? UpdatedDate { get; set; }

        // Navigation property for orders
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}