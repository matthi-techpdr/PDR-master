using PDR.Domain.Model;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiInsuranceModel
    {
        public ApiInsuranceModel()
        {
        }

        public ApiInsuranceModel(Insurance insurance)
        {
            if (insurance != null)
            {
                this.Claim = insurance.Claim;
                this.CompanyName = insurance.CompanyName;
                this.Policy = insurance.Policy;
                this.ClaimDate = insurance.ClaimDate.HasValue ? insurance.ClaimDate.Value.ToShortDateString() : null;
                this.AccidentDate = insurance.AccidentDate.HasValue ? insurance.AccidentDate.Value.ToShortDateString() : null;
                this.Phone = insurance.Phone;
                this.ContactName = insurance.ContactName;
                this.InsuredName = insurance.InsuredName;
            }
        }

        public string InsuredName { get; set; }

        public string CompanyName { get; set; }

        public string Policy { get; set; }

        public string Claim { get; set; }

        public string ClaimDate { get; set; }

        public string AccidentDate { get; set; }

        public string Phone { get; set; }

        public string ContactName { get; set; } 
    }
}