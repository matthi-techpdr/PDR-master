using System;
using System.Web.Mvc;

namespace PDR.Web.Core.Helpers
{
    public static class ValidationTooltipHelper
    {
        public static MvcHtmlString TooltipValidation(this HtmlHelper htmlHelper, string property,string msg)
        {
            var span = new TagBuilder("span");
            span.Attributes.Add("class", "field-validation-error");
            span.Attributes.Add("data-valmsg-replace", "true");
            span.Attributes.Add("data-valmsg-for", property);

            TagBuilder generate = new TagBuilder("span");
            generate.Attributes.Add("for", property);
            generate.Attributes.Add("generated", "true");

            TagBuilder cont = new TagBuilder("span");
            cont.Attributes.Add("class", "tooltip-validation-bg ui-state-error");
            
            TagBuilder message = new TagBuilder("span");
            message.Attributes.Add("class", "tooltip-validation");
            message.InnerHtml = msg + new TagBuilder("span");

            TagBuilder icon = new TagBuilder("span");
            icon.Attributes.Add("onmouseover", "showError(this)");
            icon.Attributes.Add("onmouseout", "hideError(this)");

            generate.InnerHtml = string.Format("{0}{1}{2}", cont, message, icon);
            span.InnerHtml = generate.ToString(); 
            
            var spanStr = span.ToString();


            return new MvcHtmlString(spanStr);
        }
    }
}