using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SmartArch.Web.Helpers
{
    public static class ModelStateErrorFilter
    {
         public static IDictionary<string, ModelState> GetInvalidItems(this ModelStateDictionary modelState)
         {
             return modelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(x => x.Key, x => x.Value);
         }
    }
}