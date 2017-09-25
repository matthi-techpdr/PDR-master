using System;

using FluentValidation;

using SmartArch.Core.Helpers;

namespace SmartArch.Core.Validators.Extensions
{
    public static class DateTimeExtensions
    {
        public static IRuleBuilderOptions<T, DateTime> IsTodayOrEarly<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
        {
            return ruleBuilder.Must(date => IsTodayOrEarly(date)).WithMessage("Can enter today or early");
        }

        public static IRuleBuilderOptions<T, DateTime?> IsTodayOrEarly<T>(this IRuleBuilder<T, DateTime?> ruleBuilder)
        {
            return ruleBuilder.Must(IsTodayOrEarly).WithMessage("Can enter today or early");
        }

        private static bool IsTodayOrEarly(DateTime? dateTime)
        {
            var isTodayOrEarly = true;
            if (dateTime.HasValue)
            {
                isTodayOrEarly = dateTime.Value.Date <= SystemTime.Now().Date;
            }

            return isTodayOrEarly;
        }
    }
}