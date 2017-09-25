using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace PDR.Web.Core.Helpers
{
    public static class CustomHtmlHelper
    {
        public static MvcHtmlString DisplayForWithLabel<TModel, TValue>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression,
            string label)
        {
            MvcHtmlString baseDisplay = htmlHelper.DisplayFor(expression);
            var baseString = baseDisplay.ToString();
            if (string.IsNullOrEmpty(baseString))
            {
                return null;
            }

            return new MvcHtmlString(label + baseDisplay);
        }
     }
}