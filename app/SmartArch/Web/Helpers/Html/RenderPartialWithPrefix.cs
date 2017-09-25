using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using SmartArch.Core.Helpers;

namespace SmartArch.Web.Helpers.Html
{
    /// <summary>
    /// this extention for Html Render Partial With Prefix helper.
    /// contains saved model state 
    /// </summary>
    public static class RenderPartialWithPrefix
    {
        /// <summary>
        /// HTMLs the state of the render partial with prefix and.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="partialViewName">Partial name of the view.</param>
        /// <param name="model">The model.</param>
        /// <param name="prefix">The prefix.</param>
        public static void RenderPartialWithPrefixAndState(this HtmlHelper helper, string partialViewName, object model, string prefix)
        {
            var globalPrefix = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
            var previousPrefixPart = string.IsNullOrEmpty(globalPrefix) ? string.Empty : globalPrefix + ".";
            var actualPrefix = previousPrefixPart + prefix;
            var view = new ViewDataDictionary { TemplateInfo = new TemplateInfo { HtmlFieldPrefix = actualPrefix } };

            foreach (var state in helper.ViewData.ModelState)
            {
                view.ModelState.Add(state.Key, state.Value);
            }

            helper.RenderPartial(partialViewName, model, view);
        }

        /// <summary>
        /// HTMLs the state of the render partial with prefix and.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="partialViewName">Partial name of the view.</param>
        /// <param name="partialModelExpression">The expression to partial model.</param>
        public static void RenderPartialWithPrefixAndState<TModel, TProperty>(this HtmlHelper<TModel> helper, string partialViewName, Expression<Func<TModel, TProperty>> partialModelExpression)
        {
            var partialModel = partialModelExpression.Compile().Invoke(helper.ViewData.Model);
            var prefix = Reflector.Property(partialModelExpression).Name;

            helper.RenderPartialWithPrefixAndState(partialViewName, partialModel, prefix);
        }
    }
}