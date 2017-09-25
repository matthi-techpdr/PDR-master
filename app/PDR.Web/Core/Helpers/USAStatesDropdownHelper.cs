using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using PDR.Domain.Model.Enums;

namespace PDR.Web.Core.Helpers
{
    public static class USAStatesDropdownHelper
    {
        private static readonly IList<SelectListItem> StatesSelectListItems;

        static USAStatesDropdownHelper()
        {
            StatesSelectListItems = Enum.GetValues(typeof(StatesOfUSA)).Cast<int>()
                .OrderBy(x => x)
                .Select(x => new SelectListItem { Text = ((StatesOfUSA)x).ToString(), Value = x.ToString(), Selected = false })
                .ToList();

            StatesSelectListItems.Insert(0, new SelectListItem { Text = @" ", Value = "-1" });
        }

        public static MvcHtmlString DropDownForUSAStates<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, StatesOfUSA>> expression, object htmlAttributes = null)
        {
            return htmlHelper.DropDownListFor(expression, StatesSelectListItems, htmlAttributes);
        }

        public static MvcHtmlString DropDownForEnum<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes = null)
        {
            var selectListItems = Enum.GetNames(typeof(TEnum))
                .OrderBy(x => x)
                .Select(x => new SelectListItem { Text = x, Value = x })
                .ToList();

            return htmlHelper.DropDownListFor(expression, selectListItems, htmlAttributes);
        }

        public static MvcHtmlString DropDownForEnum<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionLabel,  object htmlAttributes = null)
        {
            var selectListItems = Enum.GetNames(typeof(TEnum))
                .OrderBy(x => x)
                .Select(x => new SelectListItem { Text = x, Value = x })
                .ToList();

            return htmlHelper.DropDownListFor(expression, selectListItems, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString DropDownForEnumNotOrder<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionLabel, object htmlAttributes = null)
        {
            var selectListItems = Enum.GetNames(typeof(TEnum))
                .Select(x => new SelectListItem { Text = x, Value = x })
                .ToList();

            return htmlHelper.DropDownListFor(expression, selectListItems, optionLabel, htmlAttributes);
        }
    }
}