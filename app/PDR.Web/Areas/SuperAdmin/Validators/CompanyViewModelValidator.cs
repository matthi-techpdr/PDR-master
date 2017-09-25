using FluentValidation;

using PDR.Web.Areas.SuperAdmin.Models;
using PDR.Web.Core.Helpers.Validators;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.SuperAdmin.Validators
{
    public class CompanyViewModelValidator : AbstractValidator<CompanyViewModel>
    {
        public CompanyViewModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(1, 50).Must(NameBeUnique).WithMessage("The company already exists.");
            RuleFor(x => x.Address1).Length(0, 40);
            RuleFor(x => x.Address2).Length(0, 40);
            RuleFor(x => x.City).LettersAndSpaces().Length(0, 50);
            RuleFor(x => x.Zip).Length(0, 10);
            RuleFor(x => x.PhoneNumber).Digits().NotEmpty().WithMessage("Please specify phone number").Length(1, 20);
            RuleFor(x => x.Email).NotEmpty().WithMessage("Please specify e-mail").EmailAddress().Length(1, 255);
            RuleFor(x => x.AdminLogin).NotEmpty().LettersAndDigits().WithMessage("Please specify admin username").Length(4, 25);

            RuleFor(x => x.AdminName).NotEmpty().LettersAndSpaces().WithMessage("Please specify contact person").Length(1, 50);
            RuleFor(x => x.Url).NotEmpty().WithMessage("Please specify url part").Url().Length(1, 25).Must(UrlBeUnique).WithMessage("The url already exists.");

            RuleFor(x => x.Comment).Length(0, 1000);
            RuleFor(x => x.MobileLicensesNumber).NotNull().WithMessage("Please specify mobile licenses number");
            RuleFor(x => x.ActiveUsersNumber).NotNull().WithMessage("Please specify active users number ");
        }

        private static bool UrlBeUnique(CompanyViewModel model, string url)
        {
            return MustHelpers.CompanyMustBeUnique(model, company => company.Url.ToLower() == url.ToLower());
        }

        private static bool NameBeUnique(CompanyViewModel model, string name)
        {
            return MustHelpers.CompanyMustBeUnique(model, company => company.Name == name);
        }
    }
}
