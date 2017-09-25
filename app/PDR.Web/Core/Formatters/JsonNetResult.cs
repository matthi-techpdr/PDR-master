using System;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace PDR.Web.Core.Formatters
{
    public class JsonNetResult : JsonResult
    {
        public JsonNetResult(object data, JsonRequestBehavior jsonRequestBehavior)
        {
            this.Data = data;
            this.JsonRequestBehavior = jsonRequestBehavior;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
     
           if ((JsonRequestBehavior == JsonRequestBehavior.DenyGet)
               && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
           {
               throw new InvalidOperationException("JsonRequest GetNotAllowed");
           }
    
          var response = context.HttpContext.Response;
            
          response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
    
           if (ContentEncoding != null)
           {
               response.ContentEncoding = ContentEncoding;
           }
    
           if (Data != null)
           {
               var json = JsonConvert.SerializeObject(Data);
               response.Write(json);
           }
       }
    }
}