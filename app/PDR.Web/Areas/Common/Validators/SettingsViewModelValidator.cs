using FluentValidation;
using PDR.Web.Core.Helpers.Validators;
using SmartArch.Core.Validators.Extensions;
using PDR.Web.Areas.Common.Models;

namespace PDR.Web.Areas.Common.Validators
{
    public class SettingsViewModelValidator : AbstractValidator<SettingsViewModel>
    {
        public SettingsViewModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Please specify name").LettersAndSpaces().Length(1, 50);
            RuleFor(x => x.Address).Length(0, 25);
            RuleFor(x => x.SignatureName).LettersAndSpaces().Length(0, 25);
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Please specify phone number").Matches(@"[0-9]{3} [0-9]{3} [0-9]{4}").WithMessage("Phone must be like ХХХ ХХХ ХХХX");
            RuleFor(x => x.Email).EmailAddress().Length(0, 255); 
            RuleFor(x => x.Login).NotEmpty().WithMessage("Please specify user name").Length(4, 25).Must(LoginBeUnique).WithMessage("Employee with this username already exists.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Please specify password").Length(6, 25);
            RuleFor(x => x.Commission).LessThan(99);
            RuleFor(x => x.City).Length(0, 40);
            RuleFor(x => x.Zip).Length(0, 10);
        }

        private static bool LoginBeUnique(SettingsViewModel model, string login)
        {
            return MustHelpers.EmployeeMustBeUnique(model, emp => emp.Login == login);
        }
    }
}
