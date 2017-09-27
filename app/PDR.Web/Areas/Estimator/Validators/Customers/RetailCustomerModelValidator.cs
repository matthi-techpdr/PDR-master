using FluentValidation;

using PDR.Web.Areas.Estimator.Models.Estimates;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Estimator.Validators.Customers
{
	public class RetailCustomerModelValidator : AbstractValidator<RetailCustomerModel>
	{
		public RetailCustomerModelValidator()
		{
		    this.RuleFor(x => x.FirstName)
		        .NotEmpty() //.WithMessage(RetailCustomerModelValidatorRes.FirstNameMustBeNotEmpty)
		        .Length(1, 25) //.WithMessage(RetailCustomerModelValidatorRes.FirstNameMaxLengthWasReached)
		        .Letters();
                //.When(model => model.CustomerType == EstimateCustomerType.Retail); //.WithMessage(RetailCustomerModelValidatorRes.FirstNameInvalidSymbolsPresented);
			this.RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(1, 35)
                .Letters();
                //.When(model => model.CustomerType == EstimateCustomerType.Retail);
		    this.RuleFor(x => x.Address1)
		        .Length(0, 40)
                .LettersDigitsAndOtherSymbols();
                //.When(model => model.CustomerType == EstimateCustomerType.Retail);
			this.RuleFor(x => x.Address2)
                .Length(0, 40)
                .LettersDigitsAndOtherSymbols();
		    //.When(model => model.CustomerType == EstimateCustomerType.Retail);
		    this.RuleFor(x => x.Zip).Length(0, 10);
                //.When(model => model.CustomerType == EstimateCustomerType.Retail);
		    this.RuleFor(x => x.City)
                .Length(0, 40)
                .LettersDigitsAndOtherSymbols();
                //.When(model => model.CustomerType == EstimateCustomerType.Retail);
            this.RuleFor(x => x.Phone).Matches(@"[0-9]{3} [0-9]{3} [0-9]{4}").WithMessage("Phone must be like ХХХ ХХХ ХХХX");
                //.When(model => model.CustomerType == EstimateCustomerType.Retail);
			this.RuleFor(x => x.Fax)
                .Length(0, 20)
                .LettersDigitsAndOtherSymbols();
                //.When(model => model.CustomerType == EstimateCustomerType.Retail);
			this.RuleFor(x => x.Email)
                .EmailAddress()
                .Length(0, 255);
		        //.When(model => model.CustomerType == EstimateCustomerType.Retail);

		    this.RuleFor(x => x.AffiliateId).NotEmpty().NotEqual("0"); 
		}
	}
}