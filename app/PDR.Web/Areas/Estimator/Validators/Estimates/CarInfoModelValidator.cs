using FluentValidation;

using PDR.Web.Areas.Estimator.Models.Estimates;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Estimator.Validators.Estimates
{
    public class CarInfoModelValidator : AbstractValidator<CarInfoModel>
    {
        public CarInfoModelValidator()
        {
            this.RuleFor(x => x.VIN)
                .NotEmpty()
                .Length(17)
                .CheckUnusedLettersAndDigits();
            this.RuleFor(x => x.Make)
                .NotEmpty()
                .Length(1, 20);
			this.RuleFor(x => x.Model)
                .NotEmpty()
                .Length(1, 20);
            this.RuleFor(x => x.Year)
                .NotNull()
                .Digits()
                .Length(4);
			this.RuleFor(x => x.Trim)
                .Length(0, 20);
            this.RuleFor(x => x.Mileage)
                .NotEmpty()
                .Length(1, 7).WithMessage("'Miles' must be between 1 and 7 characters.")
                .Digits();
            this.RuleFor(x => x.LicensePlate)
                .Length(0, 10);
            this.RuleFor(x => x.Color)
                .Length(0, 20);
            this.RuleFor(x => x.CustRO)
                .Length(0, 20);
            this.RuleFor(x => x.Stock)
                .Length(0, 20);
		}
    }
}