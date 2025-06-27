using Microsoft.AspNetCore.Mvc;
using Day8.DTOs;
using Day8.Services;

namespace Day8.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    /// <summary>
    /// Tạo giao dịch mới
    /// </summary>
    /// <param name="transactionDto">Thông tin giao dịch</param>
    /// <returns>Giao dịch đã được tạo</returns>
    [HttpPost]
    public async Task<ActionResult<TransactionReadDto>> CreateTransaction([FromBody] TransactionCreateDto transactionDto)
    {
        try
        {
            var result = await _transactionService.CreateTransactionAsync(transactionDto);
            return CreatedAtAction(nameof(GetTransactions), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Lấy danh sách giao dịch với bộ lọc
    /// </summary>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <param name="departmentId">ID phòng ban</param>
    /// <param name="category">Danh mục</param>
    /// <returns>Danh sách giao dịch</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionReadDto>>> GetTransactions(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? departmentId = null,
        [FromQuery] string? category = null)
    {
        var transactions = await _transactionService.GetTransactionsAsync(startDate, endDate, departmentId, category);
        return Ok(transactions);
    }

    /// <summary>
    /// Lấy tổng hợp giao dịch theo danh mục
    /// </summary>
    /// <param name="startDate">Ngày bắt đầu</param>
    /// <param name="endDate">Ngày kết thúc</param>
    /// <param name="departmentId">ID phòng ban</param>
    /// <returns>Tổng hợp giao dịch</returns>
    [HttpGet("summary")]
    public async Task<ActionResult<IEnumerable<TransactionSummaryDto>>> GetTransactionSummary(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? departmentId = null)
    {
        var summary = await _transactionService.GetTransactionSummaryAsync(startDate, endDate, departmentId);
        return Ok(summary);
    }

    /// <summary>
    /// Kiểm tra ngân sách phòng ban
    /// </summary>
    /// <param name="departmentId">ID phòng ban</param>
    /// <param name="amount">Số tiền chi tiêu dự kiến</param>
    /// <returns>Kết quả kiểm tra ngân sách</returns>
    [HttpGet("validate-budget/{departmentId}")]
    public async Task<ActionResult<bool>> ValidateBudget(int departmentId, [FromQuery] decimal amount)
    {
        var canAfford = await _transactionService.ValidateBudgetLimitAsync(departmentId, amount);
        return Ok(new { CanAfford = canAfford, DepartmentId = departmentId, Amount = amount });
    }
}