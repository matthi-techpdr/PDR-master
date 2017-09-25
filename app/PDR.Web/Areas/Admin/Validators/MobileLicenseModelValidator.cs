using FluentValidation;

using PDR.Web.Areas.Admin.Models.MobileLicense;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Admin.Validators
{
    public class MobileLicenseModelValidator : AbstractValidator<MobileLicenseModel>
    {
        public MobileLicenseModelValidator()
        {
            RuleFor(x => x.DeviceId).NotEmpty().Length(1, 20);
            RuleFor(x => x.LicenseNumber).NotEmpty().Length(1, 20);
            RuleFor(x => x.DeviceName).NotEmpty().Length(1, 25);
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Please specify phone number").Matches(@"[0-9]{3} [0-9]{3} [0-9]{4}").WithMessage("Phone must be like ХХХ ХХХ ХХХX");
        }
    }
}