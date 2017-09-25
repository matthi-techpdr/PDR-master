using FluentValidation;
using PDR.Web.Areas.Admin.Models.Matrix;

namespace PDR.Web.Areas.Admin.Validators
{
    public class PriceModelValidator : AbstractValidator<PriceModel>
    {
        public PriceModelValidator()
        {
            this.RuleFor(x => x.Cost).NotEmpty();
        }
    }
}