using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using NixJqGridFramework.NixJqGridCore;
using NixJqGridFramework.ScriptGeneration;
using HtmlHelper = System.Web.Mvc.HtmlHelper;

namespace NixJqGridFramework.HtmlHelperExtentions
{
    public static class NixJqGridExtention
    {
        public static MvcHtmlString NixJqGrid(this HtmlHelper html, NixJqGrid jqGrid)
        {
            ScriptBuilder scriptBuilder = new ScriptBuilder();
            StringBuilder appendedScript = new StringBuilder();

            // script creation
            appendedScript.Append(scriptBuilder.CreateNixJqGrid(jqGrid));

            return new MvcHtmlString(appendedScript.ToString());
        }
    }

}
