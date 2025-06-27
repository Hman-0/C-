using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Day8.Data;
using Day8.DTOs;
using Day8.Models;

namespace Day8.Services;

public class TransactionService : ITransactionService
{
    private readonly FinancialDbContext _context;
    private readonly IMapper _mapper;

    public TransactionService(FinancialDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TransactionReadDto> CreateTransactionAsync(TransactionCreateDto transactionDto)
    {
        // Validate date
        if (transactionDto.Date > DateTime.Now)
        {
            throw new ArgumentException("Transaction date cannot be in the future");
        }

        // Validate department exists
        var department = await _context.Departments.FindAsync(transactionDto.DepartmentId);
        if (department == null)
        {
            throw new ArgumentException("Department not found");
        }

        // Check budget limit for expenses
        if (transactionDto.Type == TransactionType.Expense)
        {
            var canAfford = await ValidateBudgetLimitAsync(transactionDto.DepartmentId, transactionDto.Amount);
            if (!canAfford)
            {
                throw new InvalidOperationException("Transaction would exceed department budget limit");
            }
        }

        var transaction = _mapper.Map<Transaction>(transactionDto);
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Load the transaction with department for mapping
        var createdTransaction = await _context.Transactions
            .Include(t => t.Department)
            .FirstAsync(t => t.Id == transaction.Id);

        return _mapper.Map<TransactionReadDto>(createdTransaction);
    }

    public async Task<IEnumerable<TransactionReadDto>> GetTransactionsAsync(
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        int? departmentId = null, 
        string? category = null)
    {
        var query = _context.Transactions.Include(t => t.Department).AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        if (departmentId.HasValue)
            query = query.Where(t => t.DepartmentId == departmentId.Value);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(t => t.Category.Contains(category));

        var transactions = await query.OrderByDescending(t => t.Date).ToListAsync();
        return _mapper.Map<IEnumerable<TransactionReadDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionSummaryDto>> GetTransactionSummaryAsync(
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        int? departmentId = null)
    {
        var query = _context.Transactions.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        if (departmentId.HasValue)
            query = query.Where(t => t.DepartmentId == departmentId.Value);

        var groupedTransactions = await query
            .GroupBy(t => new { t.Category, t.Type })
            .Select(g => new TransactionSummaryDto
            {
                Category = g.Key.Category,
                Type = g.Key.Type,
                TotalAmount = g.Sum(t => t.Amount),
                TransactionCount = g.Count()
            })
            .ToListAsync();

        return groupedTransactions;
    }

    public async Task<bool> ValidateBudgetLimitAsync(int departmentId, decimal expenseAmount)
    {
        var department = await _context.Departments.FindAsync(departmentId);
        if (department == null) return false;

        var currentMonthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var currentMonthEnd = currentMonthStart.AddMonths(1).AddDays(-1);

        var currentMonthExpenses = await _context.Transactions
            .Where(t => t.DepartmentId == departmentId && 
                       t.Type == TransactionType.Expense &&
                       t.Date >= currentMonthStart && 
                       t.Date <= currentMonthEnd)
            .SumAsync(t => t.Amount);

        return (currentMonthExpenses + expenseAmount) <= department.BudgetLimit;
    }
}