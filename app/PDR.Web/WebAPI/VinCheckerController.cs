using System.Net;
using System.Net.Http;
using System.Web.Http;
using PDR.Domain.Services.VINDecoding;
using PDR.Web.WebAPI.Authorization;

namespace PDR.Web.WebAPI
{
    [ApiAuthorize]
    public class VinCheckerController : ApiController
    {
        private readonly IVINDecode vinDecoder;

        public VinCheckerController()
        {
            this.vinDecoder = new VINDecode();
        }

        public HttpResponseMessage Get(string vin)
        {
            CheckerVinModel checkerVinModel = new CheckerVinModel();
            var model = checkerVinModel.GetVinModel(vin);
            model.VinInfo = this.vinDecoder.Decode(vin.ToUpper());

            HttpStatusCode code = HttpStatusCode.OK;
            if (model.IsExistDocument)
            {
                code = HttpStatusCode.Accepted;
            }
            return Request.CreateResponse(code, model);
        }
    }
}