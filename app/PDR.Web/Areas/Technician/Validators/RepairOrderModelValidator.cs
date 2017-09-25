using FluentValidation;

using PDR.Web.Areas.Technician.Models;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Technician.Validators
{
    public class RepairOrderModelValidator : AbstractValidator<RepairOrderModel>
    {
         public RepairOrderModelValidator()
         {
             RuleFor(x => x.RetailDiscount).Digits().WithMessage("Enter only positive number").Length(0,3);
             RuleFor(x => x.AdditionalDiscount).Double().WithMessage("Enter only fractional positive number, format 'nn.nn'");
         }
    }
}