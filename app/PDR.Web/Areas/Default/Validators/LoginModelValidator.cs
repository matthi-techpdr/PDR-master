using FluentValidation;

using PDR.Resources.Web.Area.Default;

using PDR.Web.Areas.Default.Models;

namespace PDR.Web.Areas.Default.Validators
{
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(x => x.Login).NotEmpty().WithMessage(Login.LoginErrorMessage).Length(4, 50);
            RuleFor(x => x.Password).NotEmpty().WithMessage(Login.PasswordErrorMessage).Length(6, 25);
        }
    }
}
