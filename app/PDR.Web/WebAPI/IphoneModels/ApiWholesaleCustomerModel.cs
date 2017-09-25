using System.Collections.Generic;
using System.Linq;
using PDR.Domain.Model.Customers;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiWholesaleCustomerModel : ApiCustomerModel
    {
        public ApiWholesaleCustomerModel()
        {
        }

        public ApiWholesaleCustomerModel(WholesaleCustomer customer)
            : base(customer)
        {
            if (customer != null)
            {
                this.ContactTitle = customer.ContactTitle;
                this.Comments = customer.Comment;
                this.ContactName = customer.ContactName;
                this.Name = customer.Name;
                this.Matrixes = customer.Matrices.Select(x => new ApiMatrixModel(x)).ToList();
                this.HourlyRate = customer.HourlyRate;
                this.PartRate = customer.PartRate;
                this.WorkByThemselve = customer.WorkByThemselve;
                this.LaborRate = customer.LaborRate;
                this.RepairOrderSignature = customer.OrderSignature;
                this.EstimateSignature = customer.EstimateSignature;
                this.MustHaveInsurance = customer.Insurance;
                this.Status = customer.Status.ToString();
                this.Discount = customer.Discount;
            }
        }

        public string Status { get; set; }

        public bool EstimateSignature { get; set; }

        public bool RepairOrderSignature { get; set; }

        public string ContactName { get; set; }

        public string Comments { get; set; }

        public string ContactTitle { get; set; }

        public double HourlyRate { get; set; }

        public double PartRate { get; set; }

        public bool WorkByThemselve { get; set; }

        public double LaborRate { get; set; }

        public bool MustHaveInsurance { get; set; }

        public IEnumerable<ApiMatrixModel> Matrixes { get; set; }

        public double Discount { get; set; }
    }
}