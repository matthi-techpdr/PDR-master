using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PDR.Web.Core.Formatters
{
    public class JsonNetFormatter : MediaTypeFormatter
    {
        private readonly JsonSerializerSettings jsonSerializerSettings;

        private Encoding Encoding { get; set; }

        public JsonNetFormatter(JsonSerializerSettings jsonSerializerSettings)
        {
            this.jsonSerializerSettings = jsonSerializerSettings ?? new JsonSerializerSettings();
            // Fill out the mediatype and encoding we support
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            Encoding = new UTF8Encoding(false, true);
        }

        public override bool CanReadType(Type type)
        {
            // don't serialize JsonValue structure use default for that
            if (type == typeof(JValue) || type == typeof(JObject) || type == typeof(JArray))
                return false;

            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
        {
            // Create a serializer
            var serializer = JsonSerializer.Create(this.jsonSerializerSettings);

            // Create task reading the content
            return Task.Factory.StartNew(() =>
                {
                    using (var streamReader = new StreamReader(stream, Encoding))
                    {
                        using (var jsonTextReader = new JsonTextReader(streamReader))
                        {
                            return serializer.Deserialize(jsonTextReader, type);
                        }
                    }
                });
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            // Create a serializer
                JsonSerializer serializer = JsonSerializer.Create(this.jsonSerializerSettings);

                // Create task writing the serialized content
                var task = Task.Factory.StartNew(
                    () =>
                        {

                            using (
                                var jsonTextWriter = new JsonTextWriter(new StreamWriter(stream, Encoding))
                                                         {
                                                             CloseOutput = false
                                                         })
                            {
                                try
                                {
                                    serializer.Serialize(jsonTextWriter, value);
                                    jsonTextWriter.Flush();
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        });
            return task;
        }
    }
}