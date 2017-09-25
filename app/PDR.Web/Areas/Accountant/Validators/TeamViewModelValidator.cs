using FluentValidation;

using PDR.Web.Areas.Accountant.Models.Team;

namespace PDR.Web.Areas.Accountant.Validators
{
    public class TeamViewModelValidator : AbstractValidator<TeamViewModel>
    {
        public TeamViewModelValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Please specify team title").Length(1, 50);
            RuleFor(x => x.Comments).Length(1, 100);
        }
    }
}