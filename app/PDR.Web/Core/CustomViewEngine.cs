using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PDR.Web.Core
{
    /// <summary>
    /// Represents custom view engine.
    /// This engine search views in '~/Areas/Common/Views/' as common folder for shared views.
    /// </summary>
    public class CustomViewEngine : RazorViewEngine
    {
        /// <summary>
        /// The relative path to common folder with shared between areas views.
        /// </summary>
        public static readonly string CommonViewsFolder = "~/Areas/Common/Views/";

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomViewEngine"/> class.
        /// </summary>
        public CustomViewEngine()
        {
            this.AreaMasterLocationFormats = this.GetCustomAreaMasterLocationFormats();
            this.AreaViewLocationFormats = this.GetCustomAreaViewLocationFormats();
            this.AreaPartialViewLocationFormats = this.AreaViewLocationFormats;
        }

        /// <summary>
        /// Places the template paths in common view folder.
        /// </summary>
        /// <param name="filePathTemplates">The file path templates.</param>
        /// <returns>The template paths placed in common view folder.</returns>
        private static IEnumerable<string> PlaceInCommonViewFolder(params string[] filePathTemplates)
        {
            var result = filePathTemplates.Select(s => CommonViewsFolder + s);

            return result;
        }

        /// <summary>
        /// get view  for http ajax or(post:get) mode
        /// </summary>
        /// <param name="controllerContext">this controllerContext</param>
        /// <param name="viewPath">this viewPath</param>
        /// <param name="masterPath">this masterPath</param>
        /// <returns>result view</returns>
        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            if (controllerContext.HttpContext.Request.IsAjaxRequest())
            {
                return this.CreatePartialView(controllerContext, viewPath);
            }

            return base.CreateView(controllerContext, viewPath, masterPath);
        }

        /// <summary>
        /// Gets the custom area master location formats.
        /// {0} - action name, {1} - controller name in templates.
        /// </summary>
        /// <returns>The custom area master location formats.</returns>
        private string[] GetCustomAreaMasterLocationFormats()
        {
            var newPathTemplates = PlaceInCommonViewFolder("{1}/{0}.master", "Shared/{0}.master");
            var result = this.AreaMasterLocationFormats.Union(newPathTemplates).ToArray();
            return result;
        }

        /// <summary>
        /// Gets the custom area view location formats.
        /// {0} - action name, {1} - controller name in templates.
        /// </summary>
        /// <returns>The custom area view location formats.</returns>
        private string[] GetCustomAreaViewLocationFormats()
        {
            var newPathTemplates = PlaceInCommonViewFolder("{1}/{0}.view.tt", "{1}/{0}.aspx", "{1}/{0}.cshtml", "{1}/{0}.vbhtml", "Shared/{0}.view.tt", "Shared/{0}.aspx", "Shared/{0}.ascx", "Shared/{0}.cshtml", "Shared/{0}.vbhtml");
            var result = this.AreaViewLocationFormats.Union(newPathTemplates).ToArray();

            return result;
        }
    }
}