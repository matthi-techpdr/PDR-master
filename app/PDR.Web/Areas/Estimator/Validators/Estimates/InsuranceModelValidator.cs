using FluentValidation;

using PDR.Web.Areas.Estimator.Models.Estimates;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Estimator.Validators.Estimates
{
	public class InsuranceModelValidator : AbstractValidator<InsuranceModel>
	{
        public InsuranceModelValidator()
		{
            this.RuleFor(x => x.InsuredName).Length(0, 50);
            this.RuleFor(x => x.CompanyName).NotEmpty().Length(1, 50);
			this.RuleFor(x => x.Policy).Length(0, 30);
            this.RuleFor(x => x.Claim).NotEmpty().Length(1, 30).LettersDigitsAndOtherSymbols();
            this.RuleFor(x => x.AccidentDate).IsTodayOrEarly();
            this.RuleFor(x => x.ClaimDate).IsTodayOrEarly();
            this.RuleFor(x => x.Phone).Matches(@"[0-9]{3} [0-9]{3} [0-9]{4}").WithMessage("Phone must be like ХХХ ХХХ ХХХX");
            this.RuleFor(x => x.ContactName).Length(0, 50).LettersDigitsAndOtherSymbols(); ;
		}
	}
}