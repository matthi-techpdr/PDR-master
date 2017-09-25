
namespace NixJqGridFramework.ScriptGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using System.IO;
    using NixJqGridFramework.Entities;
    using NixJqGridFramework.Entities.Enums;
    using NixJqGridFramework.NixJqGridCore;

    public class ScriptBuilder
    {
        /// <summary>
        /// </summary>
        /// <param name="jqGrid">
        /// The jq grid.
        /// </param>
        /// <returns>
        /// </returns>
        private TagBuilder CreateScript(NixJqGrid jqGrid)
        {
            var tagBuilder = new TagBuilder("script");
            tagBuilder.Attributes.Add("type", "text/javascript");
            var jsonSettingObject = string.Format(
                "var settings = {0}",
                JsonConvert.SerializeObject(jqGrid, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            var scriptTemplate = this.GetJsTempalte("NixJqGridFramework.Scripts.Main.js");
            scriptTemplate += this.GetJsTempalte("NixJqGridFramework.Scripts.Functions.js");
            tagBuilder.InnerHtml = string.Format("{0}{1}{2}", jsonSettingObject, Environment.NewLine, scriptTemplate);
            return tagBuilder; 
        }

        /// <summary>
        /// Gets the js tempalte.
        /// </summary>
        /// <returns>
        /// Js template.
        /// </returns>
        private string GetJsTempalte(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var jsStream = assembly.GetManifestResourceStream(path);
            if (jsStream != null)
            {
                var streamReader = new StreamReader(jsStream);
                return streamReader.ReadToEnd();
            }
            return string.Empty;
        }

        /// <summary>
        /// Generate NixJqGridScript script
        /// </summary>
        /// <param name="jqGrid">JqGrid object for script generation</param>
        /// <returns></returns>
        public string CreateNixJqGrid(NixJqGrid jqGrid)
        {
            var tableTagBuilder = this.CreateTable(jqGrid.TableId);
            var pagerTagBuilder = this.CreatePager(jqGrid.PagerId);
            return tableTagBuilder + Environment.NewLine +
                   pagerTagBuilder + Environment.NewLine +
                   this.CreateScript(jqGrid) + Environment.NewLine;
        }

        /// <summary>
        /// Create the table for NixJqGrid
        /// </summary>
        /// <returns>table in html format</returns>
        private TagBuilder CreateTable(string tableName)
        {
            var tagBuilder = new TagBuilder("table");
            tagBuilder.Attributes.Add("id", tableName);
            return tagBuilder;
        }

        /// <summary>
        /// Create the div(pager) for NixJqGrid
        /// </summary>
        /// <param name="pagerName">Name of the pager.</param>
        /// <returns>
        /// pager in html format
        /// </returns>
        private TagBuilder CreatePager(string pagerName)
        {
            var tagBuilder = new TagBuilder("div");
            tagBuilder.Attributes.Add("id", pagerName);
            return tagBuilder;
        }
    }
}
