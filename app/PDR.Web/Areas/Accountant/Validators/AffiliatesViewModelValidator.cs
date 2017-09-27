using FluentValidation;

using PDR.Web.Areas.Accountant.Models.Customer;
using PDR.Web.Core.Helpers.Validators;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Accountant.Validators
{
    public class AffiliatesViewModelValidator : AbstractValidator<AffiliatesViewModel>
    {
        public AffiliatesViewModelValidator() 
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Please specify name").Length(1, 50);
            RuleFor(x => x.Address1).NotEmpty().WithMessage("Please specify address1 name").Length(1, 40);
            RuleFor(x => x.Address2).Length(0, 40);
            RuleFor(x => x.City).Length(0, 40);
            RuleFor(x => x.Zip).NotEmpty().WithMessage("Please specify zip").Length(0, 10);
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Please specify phone number").Matches(@"[0-9]{3} [0-9]{3} [0-9]{4}").WithMessage("Phone must be like ХХХ ХХХ ХХХX"); ;
            RuleFor(x => x.Fax).Length(0, 20);
            RuleFor(x => x.ContactName).Length(0, 25);
            RuleFor(x => x.ContactTitle).Length(0, 25);
            RuleFor(x => x.Email).NotEmpty().WithMessage("Please specify e-mail").EmailAddress().Must(EmailBeUnique).WithMessage("Email already exists.").Length(1, 255); 
            RuleFor(x => x.Comment).Length(0, 1000);
            RuleFor(x => x.LaborRate).Double(null, 4).WithMessage("Enter only fractional positive number with 4 digits after dot, format 'nn+.nnnn'");
            RuleFor(x => x.PartRate).Double(null, 4).WithMessage("Enter only fractional positive number, format 'nn+.nnnn'");
            RuleFor(x => x.HourlyRate).Double(null, null).WithMessage("Enter only fractional positive number, format 'nn+.nn'");
        }

        private static bool EmailBeUnique(AffiliatesViewModel model, string email)
        {
            return MustHelpers.MustBeUnique<Domain.Model.Customers.WholesaleCustomer>(model, x => x.Email == email);
        }
    }
}