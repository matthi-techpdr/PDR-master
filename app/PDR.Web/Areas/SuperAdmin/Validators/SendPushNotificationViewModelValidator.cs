using FluentValidation;

using PDR.Web.Areas.SuperAdmin.Models;

namespace PDR.Web.Areas.SuperAdmin.Validators
{
    public class SendPushNotificationViewModelValidator : AbstractValidator<SendPushNotificationViewModel>
    {
        public SendPushNotificationViewModelValidator()
        {
            this.RuleFor(x => x.Message).Length(1, 250).NotEmpty();
        }
    }
}