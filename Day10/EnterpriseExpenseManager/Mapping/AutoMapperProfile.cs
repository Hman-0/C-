using AutoMapper;
using EnterpriseExpenseManager.Data;
using EnterpriseExpenseManager.DTOs;

namespace EnterpriseExpenseManager.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Expense, ExpenseDto>().ReverseMap();
            CreateMap<CreateExpenseDto, Expense>();
            CreateMap<UpdateExpenseDto, Expense>();
        }
    }
} 