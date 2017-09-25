using FluentValidation;

using PDR.Web.Areas.Admin.Models.Vehicle;
using PDR.Web.Core.Helpers.Validators;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Admin.Validators
{
    public class VehicleInfoModelValidator : AbstractValidator<VehicleInfoModel>
    {
        public VehicleInfoModelValidator()
        {
            RuleFor(x => x.Make).NotEmpty().Must(MakeBeUnique).WithMessage("This vehicle is already exists in the database.").Length(1, 50);
            RuleFor(x => x.Model).NotEmpty().LettersSpacesAndDigits().Must(ModelBeUnique).WithMessage("This vehicle is already exists in the database.").Length(1, 50);
            RuleFor(x => x.YearFrom).NotEmpty().Digits().Length(4);
            RuleFor(x => x.YearTo).NotEmpty().Digits().Length(4);
        }

        private static bool ModelBeUnique(VehicleInfoModel infomodel, string model)
        {
            return MustHelpers.MustBeUnique<Domain.Model.Effort.CarModel>(
                infomodel,
                x => x.Model == model
                && x.Make == infomodel.Make
                && x.YearFrom.ToString() == infomodel.YearFrom
                && x.YearTo.ToString() == infomodel.YearTo);
        }

        private static bool MakeBeUnique(VehicleInfoModel infomodel, string make)
        {
            return MustHelpers.MustBeUnique<Domain.Model.Effort.CarModel>(
                infomodel,
                x => x.Model == infomodel.Model
                && x.Make == make
                && x.YearFrom.ToString() == infomodel.YearFrom
                && x.YearTo.ToString() == infomodel.YearTo);
        }
    }
}