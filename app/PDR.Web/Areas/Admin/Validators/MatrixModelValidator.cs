using FluentValidation;

using PDR.Domain.Model.Matrixes;
using PDR.Web.Areas.Admin.Models.Matrix;
using PDR.Web.Core.Helpers.Validators;

using SmartArch.Core.Validators.Extensions;

namespace PDR.Web.Areas.Admin.Validators
{
    public class MatrixModelValidator : AbstractValidator<MatrixModel>
    {
        public MatrixModelValidator()
        {
            RuleFor(x => x.Name).NotNull().Length(1, 25).Must(NameBeUnique).WithMessage("The matrix already exists.");
            RuleFor(x => x.Description).Length(0, 50);
            RuleFor(x => x.AluminumPanel).NotNull().OnlyIntPercents();
            RuleFor(x => x.DoubleLayeredPanels).NotNull().OnlyIntPercents();
            RuleFor(x => x.Max).NotNull().OnlyIntPercents();
            RuleFor(x => x.Suv).NotNull().OnlyIntPercents();
            RuleFor(x => x.PerWholeCar).Double().NotNull();
            RuleFor(x => x.PerBodyPart).Double().NotNull();
            RuleFor(x => x.OversizedDents).Double().NotNull();
        }

        private static bool NameBeUnique(MatrixModel model, string name)
        {
            return MustHelpers.MustBeUnique<Matrix>(model, matrix => matrix.Name == name);
        }
    }
}