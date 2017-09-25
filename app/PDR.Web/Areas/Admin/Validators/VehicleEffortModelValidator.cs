using FluentValidation;

using PDR.Web.Areas.Admin.Models.Vehicle;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Admin.Validators
{
    public class VehicleEffortModelValidator : AbstractValidator<VehicleEffortModel>
    {
        public VehicleEffortModelValidator()
        {
            RuleFor(x => x.HoursR_I).Double(null, 2).WithMessage("Enter only fractional positive number, format 'nn.nn'");
            RuleFor(x => x.HoursR_R).Double(null, 2).WithMessage("Enter only fractional positive number, format 'nn.nn'");
        }
    }
}