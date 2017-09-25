using PDR.Domain.Model;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiEstimateLightModel : BaseIPhoneModel
    {
        public ApiEstimateLightModel()
        {
        }

        public ApiEstimateLightModel(Estimate estimate)
            : this()
        {
            if (estimate != null)
            {
                this.Id = estimate.Id;
                this.CreationDate = estimate.CreationDate.ToShortDateString();
                this.Archived = estimate.Archived;
                this.Status = estimate.EstimateStatus.ToString();
                this.CalculatedSum = estimate.TotalAmount;
                this.Customer = new ApiCustomerDocumentModel<Estimate>(estimate);
                this.Car = new ApiCarLightModel(estimate.Car);
                this.Type = estimate.Type.ToString();
                this.New = estimate.New;
                this.EmployeeName = estimate.Employee.Name;
            }
        }

        public string CreationDate { get; set; }

        public bool? Archived { get; set; }

        public double? CalculatedSum { get; set; }

        public ApiCustomerDocumentModel<Estimate> Customer { get; set; }

        public string EmployeeName { get; set; }

        public ApiCarLightModel Car { get; set; }

        public string Status { get; set; }

        public string Type { get; set; }

        public bool New { get; set; }
    }
}