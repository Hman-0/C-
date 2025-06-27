using System.ComponentModel.DataAnnotations;

namespace Day8.Models;

public class Transaction
{
    public int Id { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }
    
    [Required]
    public TransactionType Type { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public int DepartmentId { get; set; }
    
    public Department Department { get; set; } = null!;
}

public enum TransactionType
{
    Income,
    Expense
}