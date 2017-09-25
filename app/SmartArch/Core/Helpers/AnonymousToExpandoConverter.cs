using System.Collections.Generic;
using System.Dynamic;
using System.Web.Routing;

namespace SmartArch.Core.Helpers
{
    /// <summary>
    /// Represents extensions for anonymous type.
    /// </summary>
    public static class AnonymousToExpandoConverter
    {
        /// <summary>
        /// Convert anonymous object to the expando object.
        /// </summary>
        /// <param name="anonymousObject">The anonymous object.</param>
        /// <returns>The expando object.</returns>
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = new RouteValueDictionary(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
            {
                expando.Add(item);
            }

            return (ExpandoObject)expando;
        }
    }
}