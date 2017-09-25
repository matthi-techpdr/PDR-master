// -----------------------------------------------------------------------
// <copyright file="EditRules.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NixJqGridFramework.Entities
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using NixJqGridFramework.Helpers;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EditRules
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is required.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is number.
        /// </summary>
        public bool IsNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is integer.
        /// </summary>
        public bool IsInteger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is email.
        /// </summary>
        public bool IsEmail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is URL.
        /// </summary>
        public bool IsUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is date.
        /// </summary>
        public bool IsDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is time.
        /// </summary>
        public bool IsTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [custom validation].
        /// </summary>
        public bool CustomValidation { get; set; }

        /// <summary>
        /// Gets or sets CustomValidationFunctionName.
        /// </summary>
        public string CustomValidationFunctionName { get; set; }

        /// <summary>
        /// Gets or sets MinValue.
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// Gets or sets the max value.
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// The string.
        /// </returns>
        public override string ToString()
        {
            var url = string.Format("url:{0}", this.IsUrl.BoolToString());
            var required = string.Format("required:{0}", this.IsRequired.BoolToString());
            var number = string.Format("number:{0}", this.IsNumber.BoolToString());
            var integer = string.Format("integer:{0}", this.IsInteger.BoolToString());
            var email = string.Format("email:{0}", this.IsEmail.BoolToString());
            var date = string.Format("date:{0}", this.IsDate.BoolToString());
            var time = string.Format("time:{0}", this.IsTime.BoolToString());
            var custom = string.Format("custom:{0}", this.CustomValidation.BoolToString());
            var customFunction = this.CustomValidationFunctionName != null
                                     ? string.Format("custom_func:{0}", this.CustomValidationFunctionName)
                                     : null;
            var minValue = this.MinValue != 0 ? string.Format("minValue:{0}", this.MinValue) : null;
            var maxValue = this.MaxValue != 0 ? string.Format("maxValue:{0}", this.MaxValue) : null;

            var editRulesColection = new List<string>
                {
                    url, required, number, integer, email, date, time, custom, customFunction, minValue, maxValue
                };
            var editRulesString = new StringBuilder();
            foreach (var rule in editRulesColection)
            {
                if (rule != null)
                {
                    editRulesString.AppendFormat("{0},", rule);
                }
            }

            return editRulesString.ToString();
        }
    }
}
