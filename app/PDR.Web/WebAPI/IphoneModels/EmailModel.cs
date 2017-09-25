using System.Web.Mvc;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class EmailModel
    {
        public string Addresses { get; set; }
        [AllowHtml]
        public string Message { get; set; }

        public string Subject { get; set; }

        public long EstimateId { get; set; }

        public long RepairOrderId { get; set; }

        public long InvoiceId { get; set; }

        public bool IsBasic { get; set; }
    }
}