using System.ComponentModel.DataAnnotations;

namespace Day7.Models;

public class TodoItem
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    public DateTime Deadline { get; set; }
    
    public bool IsCompleted { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}