using EnterpriseExpenseManager.DTOs;
using FluentValidation;

namespace EnterpriseExpenseManager.Validators
{
    public class UpdateExpenseDtoValidator : AbstractValidator<UpdateExpenseDto>
    {
        public UpdateExpenseDtoValidator()
        {
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Type).NotEmpty().Must(x => x == "Income" || x == "Expense");
            RuleFor(x => x.Category).NotEmpty();
        }
    }
} 