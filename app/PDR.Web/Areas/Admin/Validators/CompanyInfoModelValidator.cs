using FluentValidation;

using PDR.Web.Areas.Admin.Models.ComapnyInfo;
using PDR.Web.Core.Helpers.Validators;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Admin.Validators
{
    public class CompanyInfoModelValidator : AbstractValidator<CompanyInfoModel>
    {
        public CompanyInfoModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(1, 50).Must(NameBeUnique).WithMessage("The company already exists.");
            RuleFor(x => x.Address1).Length(0, 40);
            RuleFor(x => x.Address2).Length(0, 40);
            RuleFor(x => x.City).LettersAndSpaces().Length(0, 50);
            RuleFor(x => x.Zip).Length(0, 10);
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Please specify phone number").Matches(@"[0-9]{3} [0-9]{3} [0-9]{4}").WithMessage("Phone must be like ХХХ ХХХ ХХХX");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Please specify e-mail").EmailAddress().Length(1, 255);

            //RuleFor(x => x.DefaultHourlyRate).NotNull().WithMessage("Please specify default hourly rate").GreaterThan(0);
            RuleFor(x => x.LimitForBodyPartEstimate).NotNull().WithMessage("Please specify limit for body part estimate").LessThan(99).GreaterThan(0);

            RuleFor(x => x.EstimateEmailSubject).Length(0, 50);
            RuleFor(x => x.RepairOrderEmailSubject).Length(0, 50);
            RuleFor(x => x.InvoiceEmailSubject).Length(0, 50);
            RuleFor(x => x.Notes).Length(0, 5000);
        }

        private static bool NameBeUnique(CompanyInfoModel model, string name)
        {
            return MustHelpers.CompanyMustBeUnique(model, company => company.Name == name);
        }
    }
}
