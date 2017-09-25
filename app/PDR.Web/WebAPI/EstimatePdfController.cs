using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.PDFConverters;
using PDR.Web.Core.Attributes;
using SmartArch.Data;

namespace PDR.Web.WebAPI
{
    public class EstimatePdfController : ApiController
    {
        private readonly IPdfConverter pdfConverter;

        private readonly ICompanyRepository<Estimate> estimates;

        private readonly ILogger logger;

        public EstimatePdfController()
        {
            this.pdfConverter = ServiceLocator.Current.GetInstance<IPdfConverter>();
            this.estimates = ServiceLocator.Current.GetInstance<ICompanyRepository<Estimate>>();
            this.logger = ServiceLocator.Current.GetInstance<ILogger>();
        }

        [WebApiTransaction]
        public HttpResponseMessage Get(long id, bool detailed = false)
        {
            var estimate = this.estimates.Get(id);

            var pdf = this.pdfConverter.Convert(estimate, detailed);


            var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(new MemoryStream(pdf))
                };

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = string.Format("{0}.pdf", estimate.Id) 
                };

            this.logger.Log(estimate, EstimateLogActions.Print);
            return response;
        }

        private static string ResolveUrl(string secondUrlPart)
        {
            return string.Format(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, secondUrlPart));
        }
    }
}