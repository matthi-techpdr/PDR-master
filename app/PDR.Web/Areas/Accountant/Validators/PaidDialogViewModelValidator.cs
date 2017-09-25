using FluentValidation;

using PDR.Web.Areas.Accountant.Models.Invoice;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Accountant.Validators
{
    public class PaidDialogViewModelValidator : AbstractValidator<PaidDialogViewModel>
    {
        public PaidDialogViewModelValidator()
        {
            this.RuleFor(x => x.PaymentDate)
                .NotEmpty()
                .IsTodayOrEarly();
			this.RuleFor(x => x.PaidSum)
                .NotEmpty()
                .Matches(@"^\d*\.?\d{1,}?$")
                .Length(1, 100);
        }
    }
}