using FluentValidation;

using PDR.Web.Areas.Estimator.Models.Estimates;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Estimator.Validators.Estimates
{
    public class SendEmailViewModelValidator : AbstractValidator<SendEmailViewModel>
    {
        public SendEmailViewModelValidator()
        {
            this.RuleFor(x => x.Message)
                .Length(0, 1000);
            this.RuleFor(x => x.Subject)
                .Length(1, 50)
                .NotEmpty();
			this.RuleFor(x => x.To)
                .NotEmpty()
                .CheckEmails()
                .Length(1, 100);
		}
    }
}