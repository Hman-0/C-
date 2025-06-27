using FluentValidation;
using ProductCatalogApi.DTOs;

namespace ProductCatalogApi.Validators
{
    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên sản phẩm là bắt buộc")
                .MaximumLength(100).WithMessage("Tên sản phẩm không được vượt quá 100 ký tự")
                .MinimumLength(2).WithMessage("Tên sản phẩm phải có ít nhất 2 ký tự");
            
            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Danh mục là bắt buộc")
                .MaximumLength(50).WithMessage("Danh mục không được vượt quá 50 ký tự")
                .MinimumLength(2).WithMessage("Danh mục phải có ít nhất 2 ký tự");
            
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Giá phải lớn hơn 0")
                .LessThanOrEqualTo(999999999).WithMessage("Giá không được vượt quá 999,999,999");
            
            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng tồn kho phải >= 0")
                .LessThanOrEqualTo(999999).WithMessage("Số lượng tồn kho không được vượt quá 999,999");
            
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}