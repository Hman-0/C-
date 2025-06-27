using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Day8.Data;
using Day8.DTOs;
using Day8.Models;

namespace Day8.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly FinancialDbContext _context;
    private readonly IMapper _mapper;

    public DepartmentsController(FinancialDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Lấy danh sách tất cả phòng ban
    /// </summary>
    /// <returns>Danh sách phòng ban</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
    {
        var departments = await _context.Departments.ToListAsync();
        return Ok(_mapper.Map<IEnumerable<DepartmentDto>>(departments));
    }

    /// <summary>
    /// Lấy thông tin phòng ban theo ID
    /// </summary>
    /// <param name="id">ID phòng ban</param>
    /// <returns>Thông tin phòng ban</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        
        if (department == null)
        {
            return NotFound($"Department with ID {id} not found");
        }

        return Ok(_mapper.Map<DepartmentDto>(department));
    }

    /// <summary>
    /// Tạo phòng ban mới
    /// </summary>
    /// <param name="departmentDto">Thông tin phòng ban</param>
    /// <returns>Phòng ban đã được tạo</returns>
    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> CreateDepartment([FromBody] DepartmentDto departmentDto)
    {
        var department = _mapper.Map<Department>(departmentDto);
        department.Id = 0; // Ensure new entity
        
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<DepartmentDto>(department);
        return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, result);
    }

    /// <summary>
    /// Cập nhật thông tin phòng ban
    /// </summary>
    /// <param name="id">ID phòng ban</param>
    /// <param name="departmentDto">Thông tin phòng ban mới</param>
    /// <returns>Phòng ban đã được cập nhật</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<DepartmentDto>> UpdateDepartment(int id, [FromBody] DepartmentDto departmentDto)
    {
        var existingDepartment = await _context.Departments.FindAsync(id);
        if (existingDepartment == null)
        {
            return NotFound($"Department with ID {id} not found");
        }

        existingDepartment.Name = departmentDto.Name;
        existingDepartment.BudgetLimit = departmentDto.BudgetLimit;

        await _context.SaveChangesAsync();

        return Ok(_mapper.Map<DepartmentDto>(existingDepartment));
    }

    /// <summary>
    /// Xóa phòng ban
    /// </summary>
    /// <param name="id">ID phòng ban</param>
    /// <returns>Kết quả xóa</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDepartment(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null)
        {
            return NotFound($"Department with ID {id} not found");
        }

        // Check if department has transactions
        var hasTransactions = await _context.Transactions.AnyAsync(t => t.DepartmentId == id);
        if (hasTransactions)
        {
            return BadRequest("Cannot delete department that has transactions. Please delete all transactions first.");
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}