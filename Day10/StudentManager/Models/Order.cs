using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManager.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã đơn hàng là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã đơn hàng không được vượt quá 50 ký tự")]
        public string OrderCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount => (decimal)Quantity * Price;

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string? Description { get; set; }

        // Foreign key
        [Required(ErrorMessage = "Sinh viên là bắt buộc")]
        public int StudentId { get; set; }

        // Navigation property
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;
    }
}