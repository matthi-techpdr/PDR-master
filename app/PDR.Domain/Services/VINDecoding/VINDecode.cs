using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Xml;

namespace PDR.Domain.Services.VINDecoding
{
    public class VINDecode : IVINDecode
    {
        private string vin;

        private dynamic GetVinDecodeSettings()
        {
            dynamic settings = new ExpandoObject();
            settings.Login = ConfigurationManager.AppSettings["VinLinkLogin"];
            settings.Password = ConfigurationManager.AppSettings["VinLinkPassword"];
            settings.Url = ConfigurationManager.AppSettings["VinLinkUrl"] + "type=basic_light&vin=" + this.vin;
            return settings;
        }

        private WebResponse GetVinDecoderResponse()
        {
            var settings = this.GetVinDecodeSettings();
            var request = WebRequest.Create(settings.Url);
            request.Timeout = 20000;
            request.Credentials = new NetworkCredential(settings.Login, settings.Password);
            WebResponse response = request.GetResponse();
            return response;
        }

        public VINInfo Decode(string vinCode)
        {
            try
            {
                this.vin = vinCode;
                var response = this.GetVinDecoderResponse();
                var content = string.Empty;
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var reader = new StreamReader(stream);
                        content = reader.ReadToEnd();
                    }
                }

                content = content.Replace("&", "&amp;");

                using (var reader = XmlReader.Create(new StringReader(content)))
                {
                    reader.ReadToFollowing("DECODED");
                    var vinInfo = new VINInfo
                        {
                            Year = reader.GetAttribute("Model_Year"),
                            Make = reader.GetAttribute("Make"),
                            Model = reader.GetAttribute("Model")
                        };
                    return vinInfo;
                }
            }
            catch(WebException exception)
            {
                return null;
            }
        }
    }
}
