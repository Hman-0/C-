using AutoMapper;
using Day8.Models;
using Day8.DTOs;

namespace Day8.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Transaction mappings
        CreateMap<TransactionCreateDto, Transaction>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Department, opt => opt.Ignore());

        CreateMap<Transaction, TransactionReadDto>()
            .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name));

        // Department mappings
        CreateMap<Department, DepartmentDto>();
        CreateMap<DepartmentDto, Department>();

        // Transaction summary mapping
        CreateMap<IGrouping<string, Transaction>, TransactionSummaryDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Key))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Sum(t => t.Amount)))
            .ForMember(dest => dest.TransactionCount, opt => opt.MapFrom(src => src.Count()))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.First().Type));
    }
}