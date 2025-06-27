using AutoMapper;
using EnterpriseExpenseManager.DTOs;
using EnterpriseExpenseManager.Repositories;

namespace EnterpriseExpenseManager.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _repo;
        private readonly IMapper _mapper;
        public ExpenseService(IExpenseRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<List<ExpenseDto>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();
            return _mapper.Map<List<ExpenseDto>>(data);
        }
        public async Task<ExpenseDto?> GetByIdAsync(int id)
        {
            var data = await _repo.GetByIdAsync(id);
            return data == null ? null : _mapper.Map<ExpenseDto>(data);
        }
        public async Task AddAsync(CreateExpenseDto dto)
        {
            var entity = _mapper.Map<Data.Expense>(dto);
            await _repo.AddAsync(entity);
        }
        public async Task UpdateAsync(int id, UpdateExpenseDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return;
            _mapper.Map(dto, entity);
            await _repo.UpdateAsync(entity);
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return;
            await _repo.DeleteAsync(entity);
        }
        public async Task<List<ExpenseDto>> GetByFilterAsync(string? type, string? category, DateTime? from, DateTime? to)
        {
            var data = await _repo.GetByFilterAsync(type, category, from, to);
            return _mapper.Map<List<ExpenseDto>>(data);
        }
        public async Task<decimal> GetTotalByTypeAsync(string type) => await _repo.GetTotalByTypeAsync(type);
        public async Task<decimal> GetTotalByCategoryAsync(string category) => await _repo.GetTotalByCategoryAsync(category);
        public async Task<Dictionary<string, decimal>> GetTotalByMonthAsync(string type) => await _repo.GetTotalByMonthAsync(type);
        public async Task<decimal> GetProfitAsync()
        {
            var income = await _repo.GetTotalByTypeAsync("Income");
            var expense = await _repo.GetTotalByTypeAsync("Expense");
            return income - expense;
        }
        public async Task<decimal> GetCashFlowAsync(DateTime from, DateTime to)
        {
            var income = (await _repo.GetByFilterAsync("Income", null, from, to)).Sum(x => x.Amount);
            var expense = (await _repo.GetByFilterAsync("Expense", null, from, to)).Sum(x => x.Amount);
            return income - expense;
        }
        public async Task<decimal> GetBudgetAsync()
        {
            // Giả lập ngân sách là tổng income - tổng expense
            return await GetProfitAsync();
        }
    }
} 