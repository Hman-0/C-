using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Day6.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề khóa học là bắt buộc")]
        [MaxLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cấp độ là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Cấp độ không được vượt quá 50 ký tự")]
        public string Level { get; set; } = string.Empty;

        [Required(ErrorMessage = "Thời lượng là bắt buộc")]
        [Range(1, 1000, ErrorMessage = "Thời lượng phải từ 1 đến 1000 giờ")]
        public int Duration { get; set; } // Thời lượng tính bằng giờ

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}