using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiCustomerDocumentModel<T> where T : CompanyEntity, IReportable
    {
        public ApiCustomerDocumentModel(){}

        public ApiCustomerDocumentModel(T document)
        {
            this.Name = document.Customer.GetCustomerName();
            this.Email = document.Customer.Email;
            this.Email2 = document.Customer.Email2;
            this.Email4 = document.Customer.Email4;
            this.InvoiceEmailSubject = document.Company.InvoicesEmailSubject;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public string Email4 { get; set; }
        public string InvoiceEmailSubject { get; set; }
    }
}