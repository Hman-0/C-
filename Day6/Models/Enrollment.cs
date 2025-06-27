using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Day6.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "StudentId là bắt buộc")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "CourseId là bắt buộc")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Ngày đăng ký là bắt buộc")]
        [DataType(DataType.Date)]
        public DateTime EnrollDate { get; set; } = DateTime.Now;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;
    }
}