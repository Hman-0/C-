using System.ComponentModel.DataAnnotations;

namespace StudentManager.Models
{
    public class Student
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string StudentCode { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Class { get; set; } = string.Empty;
        
        [Range(0, 10)]
        public double GPA { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedAt { get; set; }
    }
}