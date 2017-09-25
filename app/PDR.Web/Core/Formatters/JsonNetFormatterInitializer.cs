using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PDR.Web.Core.Formatters
{
    public static class JsonNetFormatterInitializer
    {
        public static void Init()
        {
            var serializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            serializerSettings.Converters.Add(new IsoDateTimeConverter());
            GlobalConfiguration.Configuration.Formatters[0] = new JsonNetFormatter(serializerSettings);
        }
    }
}
