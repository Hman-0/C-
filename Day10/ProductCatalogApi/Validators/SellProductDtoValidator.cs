using FluentValidation;
using ProductCatalogApi.DTOs;

namespace ProductCatalogApi.Validators
{
    public class SellProductDtoValidator : AbstractValidator<SellProductDto>
    {
        public SellProductDtoValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0")
                .LessThanOrEqualTo(1000).WithMessage("Số lượng không được vượt quá 1,000 trong một lần bán");
            
            RuleFor(x => x.Note)
                .MaximumLength(200).WithMessage("Ghi chú không được vượt quá 200 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Note));
        }
    }
}