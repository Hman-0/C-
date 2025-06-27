using EnterpriseExpenseManager.DTOs;
using EnterpriseExpenseManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseExpenseManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _service;
        public ExpenseController(IExpenseService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? type, [FromQuery] string? category, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var data = await _service.GetByFilterAsync(type, category, from, to);
            return Ok(data);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto)
        {
            await _service.AddAsync(dto);
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
        [HttpGet("summary/profit")]
        public async Task<IActionResult> GetProfit()
        {
            var profit = await _service.GetProfitAsync();
            return Ok(new { profit });
        }
        [HttpGet("summary/cashflow")]
        public async Task<IActionResult> GetCashFlow([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var cashflow = await _service.GetCashFlowAsync(from, to);
            return Ok(new { cashflow });
        }
        [HttpGet("summary/budget")]
        public async Task<IActionResult> GetBudget()
        {
            var budget = await _service.GetBudgetAsync();
            return Ok(new { budget });
        }
        [HttpGet("summary/monthly")]
        public async Task<IActionResult> GetMonthly([FromQuery] string type)
        {
            var data = await _service.GetTotalByMonthAsync(type);
            return Ok(data);
        }
    }
} 