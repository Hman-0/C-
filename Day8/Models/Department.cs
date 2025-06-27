using System.ComponentModel.DataAnnotations;

namespace Day8.Models;

public class Department
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Budget limit must be non-negative")]
    public decimal BudgetLimit { get; set; }
    
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}