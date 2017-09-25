using System.Web.Mvc;

namespace PDR.Web.Core.Helpers.Html
{
    public class InputsContext<TModel>
    {
        public HtmlHelper<TModel> Html { get; private set; }

        public InputsContext(HtmlHelper<TModel> htmlHelper)
        {
            this.Html = htmlHelper;
        }
    }
}