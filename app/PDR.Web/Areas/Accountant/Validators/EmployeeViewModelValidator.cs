using FluentValidation;
using PDR.Domain.Model.Enums;
using PDR.Web.Areas.Accountant.Models.Employee;
using PDR.Web.Core.Helpers.Validators;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Accountant.Validators
{
    public class EmployeeViewModelValidator : AbstractValidator<EmployeeViewModel>
    {
        public EmployeeViewModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Please specify name").LettersAndSpaces().Length(1, 50);
            RuleFor(x => x.Address).Length(0, 25);
            RuleFor(x => x.SignatureName).LettersAndSpaces().Length(0, 25);
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Please specify phone number").Matches(@"[0-9]{3} [0-9]{3} [0-9]{4}").WithMessage("Phone must be like ХХХ ХХХ ХХХX");
            RuleFor(x => x.Email).EmailAddress().Length(0, 255);
            RuleFor(x => x.Login).NotEmpty().WithMessage("Please specify user name").Length(4, 25).Must(LoginBeUnique).WithMessage("Employee with this username already exists.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Please specify password").Length(6, 25);
            RuleFor(x => x.Commission).LessThan(99);
            RuleFor(x => x.Comment).Length(0, 1000);
            RuleFor(x => x.City).Length(0, 40);
            RuleFor(x => x.Zip).Length(0, 10);
            RuleFor(x => x.CanEditTeamMembers).Must(VerifyRole);
        }

        private static bool VerifyRole(EmployeeViewModel model, bool? canEditTeamMembers)
        {
            //Employee currentUser = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
            if (canEditTeamMembers != null && model.Role == (int)UserRoles.Manager)
            {
                return true;
            }
            if (canEditTeamMembers == null && model.Role != (int)UserRoles.Manager)
            {
                return true;
            }
            return false;
        }

        private static bool LoginBeUnique(EmployeeViewModel model, string login)
        {
            return MustHelpers.EmployeeMustBeUnique(model, emp => emp.Login == login);
        }
    }
}
