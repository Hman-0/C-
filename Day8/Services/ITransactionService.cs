using Day8.DTOs;
using Day8.Models;

namespace Day8.Services;

public interface ITransactionService
{
    Task<TransactionReadDto> CreateTransactionAsync(TransactionCreateDto transactionDto);
    Task<IEnumerable<TransactionReadDto>> GetTransactionsAsync(
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        int? departmentId = null, 
        string? category = null);
    Task<IEnumerable<TransactionSummaryDto>> GetTransactionSummaryAsync(
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        int? departmentId = null);
    Task<bool> ValidateBudgetLimitAsync(int departmentId, decimal expenseAmount);
}