using FluentValidation;

using PDR.Web.Areas.Admin.Models.Vehicle;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Admin.Validators
{
    public class VehicleSectionModelValidator : AbstractValidator<VehicleSectionModel>
    {
        public VehicleSectionModelValidator()
        {
            RuleFor(x => x.Price).Double(null, 2).WithMessage("Enter only fractional positive number, format 'nn.nn'");
        }
    }
}