using FluentValidation;

namespace SmartArch.Core.Validators.Extensions
{
    public static class MatchExtensions
    {
         public static IRuleBuilderOptions<T, string> Letters<T>(this IRuleBuilder<T, string> ruleBuilder)
         {
             return ruleBuilder.Matches(@"^[A-Za-z]*$").WithMessage("Enter only letters");
         }

         public static IRuleBuilderOptions<T, string> Digits<T>(this IRuleBuilder<T, string> ruleBuilder)
         {
             return ruleBuilder.Matches(@"^\d*$").WithMessage("Enter only digits");
         }

         public static IRuleBuilderOptions<T, string> Double<T>(this IRuleBuilder<T, string> ruleBuilder)
         {
             return ruleBuilder.Matches(@"^[0-9]{1,4}\.?[0-9]{0,2}$").WithMessage("Enter only fractional positive number.");
         }
         public static IRuleBuilderOptions<T, string> Double<T>(this IRuleBuilder<T, string> ruleBuilder, int? digitBeforeDot, int? digitAfterDot)
         { 
             var regBefore = @"^[0-9]+";
             var regAfter = @"(\.[0-9]{1,2})?$";
             
             if(digitBeforeDot.HasValue)
             {
                 regBefore = string.Format(@"^[0-9]{{1,{0}}}", digitBeforeDot.Value);
             }

             if(digitAfterDot.HasValue)
             {
                 regAfter = string.Format(@"(\.[0-9]{{1,{0}}})?$", digitAfterDot.Value);
             }

             var reg = regBefore + regAfter;
             
             return ruleBuilder.Matches(reg).WithMessage("Enter only fractional positive number.");
         }

         public static IRuleBuilderOptions<T, string> OnlyIntPercents<T>(this IRuleBuilder<T, string> ruleBuilder)
         {
             return ruleBuilder.Matches(@"0*(100|[0-9]?[0-9])").WithMessage("Enter only percents.");
         }

         public static IRuleBuilderOptions<T, string> DoubleFormat<T>(this IRuleBuilder<T, string> ruleBuilder)
         {
             return ruleBuilder.Matches(@"^[0-9]{1,2}\.[0-9]{1,4}$").WithMessage("Enter only fractional positive number with dot.");
         }

         public static IRuleBuilderOptions<T, string> TwoDigits<T>(this IRuleBuilder<T, string> ruleBuilder)
         {
             return ruleBuilder.Matches(@"^[0-9]{0,4}$").WithMessage("Enter only fractional positive number.");
         }

         public static IRuleBuilderOptions<T, string> LettersAndDigits<T>(this IRuleBuilder<T, string> ruleBuilder)
         {
             return ruleBuilder.Matches(@"^\w*$").WithMessage("Enter only letters and digits.");
         }

         public static IRuleBuilderOptions<T, string> LettersSpacesAndDigits<T>(this IRuleBuilder<T, string> ruleBuilder)
         {
             return ruleBuilder.Matches(@"^[A-Za-z0-9]+([ ]?[A-Za-z0-9]+)+$").WithMessage("Enter only letters, spaces and digits.");
         }

         public static IRuleBuilderOptions<T, string> LettersDigitsAndOtherSymbols<T>(this IRuleBuilder<T, string> ruleBuilder)
         {
             return ruleBuilder.Matches(@"^[A-Za-z0-9]+([^<>]?[A-Za-z0-9]+)+$").WithMessage("Enter only letters, digits, spaces and other symbols.\nDo not use the following symbols - <, >.");
         }

         public static IRuleBuilderOptions<T, string> CheckUnusedLettersAndDigits<T>(this IRuleBuilder<T, string> ruleBuilder)
         {
             return ruleBuilder.Matches(@"^[^OoQqIi\W_]*$").WithMessage("Enter only letters and digits.\nDo not use the following letters - O, Q, I.");
         }

         public static IRuleBuilderOptions<T, string> CheckEmails<T>(this IRuleBuilder<T, string> ruleBuilder)
         {
             return ruleBuilder.Matches(@"^([\w\.\-\']+@\w+[\w\.\-]*?\.\w{1,6}[\, ]*\s*)*$").WithMessage("Enter only email addresses.");
         }

         public static IRuleBuilderOptions<T, string> Url<T>(this IRuleBuilder<T, string> ruleBuilder)
         {
             return ruleBuilder.Matches(@"^[A-Za-z0-9]+([_-]?[A-Za-z0-9]+)+$").WithMessage("Enter only letters, digits, hyphen and underscope.");
         }

         public static IRuleBuilderOptions<T, string> LettersAndSpaces<T>(this IRuleBuilder<T, string> ruleBuilder)
         {
             return ruleBuilder.Matches(@"^[A-Za-z]+([ ]?[A-Za-z]+)+$").WithMessage("Enter only letters and spaces.");
         }
    }
}