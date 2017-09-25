using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using SmartArch.Core.Helpers.ObjectDictionaryConvertors;

namespace PDR.Web.Core.Helpers.Html
{
    public static class InputsExtensions
    {
        private static int readOnlyDeep;

        public static InputsContext<TModel> Inputs<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            return new InputsContext<TModel>(htmlHelper);
        }

        public static MvcHtmlString TextBoxFor<TModel, TProperty>(this InputsContext<TModel> context, Expression<Func<TModel, TProperty>> propertyExpression, object htmlAttributes = null)
        {
            if (typeof(TProperty) == typeof(DateTime) || typeof(TProperty) == typeof(DateTime?))
            {
                return InnerDateFor(context, propertyExpression, htmlAttributes);
            }

            return InputFor(context, attr => context.Html.TextBoxFor(propertyExpression, attr), htmlAttributes);
        }

        public static MvcHtmlString DateFor<TModel>(this InputsContext<TModel> context, Expression<Func<TModel, DateTime>> propertyExpression, object htmlAttributes = null)
        {
            return InnerDateFor(context, propertyExpression, htmlAttributes);
        }

        public static MvcHtmlString DateFor<TModel>(this InputsContext<TModel> context, Expression<Func<TModel, DateTime?>> propertyExpression, object htmlAttributes = null)
        {
            return InnerDateFor(context, propertyExpression, htmlAttributes);
        }

        private static MvcHtmlString InnerDateFor<TModel, TDateTime>(this InputsContext<TModel> context, Expression<Func<TModel, TDateTime>> propertyExpression, object htmlAttributes = null)
        {
            var dateObj = propertyExpression.Compile().Invoke(context.Html.ViewData.Model);
            DateTime? date;
            if (typeof(TDateTime) == typeof(DateTime?) || typeof(TDateTime) == typeof(DateTime))
            {
                date = (DateTime?)(object)dateObj;
            }
            else
            {
                throw new ArgumentException("'TDateType' must be DateTime or DateTime?");
            }
            
            var attributes = (htmlAttributes ?? new { }).ToDictionary();
            if (date != null)
            {
                object formatObj;
                attributes.TryGetValue("format", out formatObj);
                string format = formatObj as string ?? "MM/dd/yyyy";
                attributes.Add("Value", date.Value.ToString(format));
            }

            return InputFor(context, attr => context.Html.TextBoxFor(propertyExpression, attr), attributes);
        }

        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this InputsContext<TModel> context, Expression<Func<TModel, TProperty>> propertyExpression, object value, object htmlAttributes = null)
        {
            return InputFor(context, attr => context.Html.RadioButtonFor(propertyExpression, value, attr), htmlAttributes, "disabled");
        }

        public static MvcHtmlString CheckBoxFor<TModel>(this InputsContext<TModel> context, Expression<Func<TModel, bool>> propertyExpression, object htmlAttributes = null)
        {
            return InputFor(context, attr => context.Html.CheckBoxFor(propertyExpression, attr), htmlAttributes, "disabled");
        }

        public static MvcHtmlString DropDownForEnum<TModel, TProperty>(this InputsContext<TModel> context, Expression<Func<TModel, TProperty>> propertyExpression, object htmlAttributes = null, bool order = false)
        {
            if (IsReadOnlyMode(context.Html))
            {
                return context.TextBoxFor(propertyExpression, htmlAttributes);
            }

            return order ? context.Html.DropDownForEnum(propertyExpression, htmlAttributes) :
                context.Html.DropDownForEnumNotOrder(propertyExpression, null, htmlAttributes);
        }

        public static MvcHtmlString DropDownForEnum<TModel, TProperty>(this InputsContext<TModel> context, Expression<Func<TModel, TProperty>> propertyExpression, string optionLabel, object htmlAttributes = null, bool order = false)
        {
            if (IsReadOnlyMode(context.Html))
            {
                return context.TextBoxFor(propertyExpression, htmlAttributes);
            }

            return order ? context.Html.DropDownForEnum(propertyExpression, optionLabel, htmlAttributes) :
                context.Html.DropDownForEnumNotOrder(propertyExpression, optionLabel, htmlAttributes);
        }

        #region ReadOnlyMode
        public static bool IsReadOnlyMode(this HtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.TempData["readonly-display-mode"] != null;
        }

        public static void ReadOnlyModeOnIgnoreDeep(this HtmlHelper htmlHelper)
        {
            htmlHelper.ViewContext.TempData["readonly-display-mode"] = true;
        }

        public static void ReadOnlyModeOffIgnoreDeep(this HtmlHelper htmlHelper)
        {
            htmlHelper.ViewContext.TempData["readonly-display-mode"] = null;
        }

        public static void ReadOnlyModeOn(this HtmlHelper htmlHelper)
        {
            readOnlyDeep++;
            htmlHelper.ViewContext.TempData["readonly-display-mode"] = true;
        }

        public static void ReadOnlyModeOff(this HtmlHelper htmlHelper)
        {
            readOnlyDeep--;
            htmlHelper.ViewContext.TempData["readonly-display-mode"] = readOnlyDeep > 0 ? (object)true : null;
        }
        #endregion

        private static MvcHtmlString InputFor<TModel>(this InputsContext<TModel> context, Func<IDictionary<string, object>, MvcHtmlString> nativeInputGenerator, object htmlAttributes, string readOnlyAttr = "readonly")
        {
            var attributes = (htmlAttributes ?? new { }).ToDictionary();
            var input = InputFor(context, nativeInputGenerator, attributes, readOnlyAttr);
            
            return input;
        }

        private static MvcHtmlString InputFor<TModel>(this InputsContext<TModel> context, Func<IDictionary<string, object>, MvcHtmlString> nativeInputGenerator, IDictionary<string, object> htmlAttributes, string readOnlyAttr = "readonly")
        {            
            if (IsReadOnlyMode(context.Html) && !htmlAttributes.ContainsKey(readOnlyAttr))
            {
                htmlAttributes.Add(readOnlyAttr, readOnlyAttr);                
            }

            var invalidAttributes = htmlAttributes.Where(x => x.Key.Contains("_")).ToList();
            foreach (var attribute in invalidAttributes)
            {
                htmlAttributes.Remove(attribute);
                htmlAttributes.Add(attribute.Key.Replace("_", "-"), attribute.Value);
            }

            var input = nativeInputGenerator.Invoke(htmlAttributes);

            return input;
        }
    }
}