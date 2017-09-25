using PDR.Domain.Helpers;
using PDR.Domain.Model;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiInvoiceModel
    {
        public ApiInvoiceModel()
        {
        }

        public ApiInvoiceModel(Invoice invoice)
        {
            this.Id = invoice.Id;
            this.CreationDate = invoice.CreationDate.ToShortDateString();
            this.PaidDate = invoice.PaidDate.ToShortDateString();
            this.Status = invoice.Status.ToString();
            this.InvoiceSum = invoice.InvoiceSum;
            this.Customer = new ApiCustomerModel
            {
                CustomerType = invoice.Customer.CustomerType.ToString(),
                Name = invoice.Customer.GetCustomerName(),
                Id = invoice.Customer.Id
            };


            this.RepairOrder = new ApiRepairOrderModel(invoice.RepairOrder);
            this.Employee = new ApiEmployeeModel { Id = invoice.TeamEmployee.Id, Name = invoice.TeamEmployee.Name, Login = invoice.TeamEmployee.Login };
            this.Archived = invoice.Archived;
            this.New = invoice.New;
            this.PaidSum = invoice.PaidSum;
        }

        public long Id { get; set; }

        public bool New { get; set; }

        public string CreationDate { get; set; }

        public ApiRepairOrderModel RepairOrder { get; set; }

        public string Status { get; set; }

        public double InvoiceSum { get; set; }

        public double PaidSum { get; set; }

        public bool Archived { get; set; }

        public ApiEmployeeModel Employee { get; set; }

        public ApiCustomerModel Customer { get; set; }

        public string PaidDate { get; set; }
    }
}