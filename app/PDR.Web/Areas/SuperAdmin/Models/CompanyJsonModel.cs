using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.SuperAdmin.Models
{
    using Domain.Model;

    public class CompanyJsonModel : IJsonModel
    {
        public CompanyJsonModel()
        {
        }

        public CompanyJsonModel(Company company)
        {
            this.Id = company.Id.ToString();
            this.Name = company.Name;
            this.PhoneNumber = company.PhoneNumber;
            this.Email = company.Email;
            this.Status = company.Status.ToString();
            this.Address1 = company.Address1;
            this.CreationDate = company.CreationDate.ToString("MM/dd/yyyy");
        }

        public string Id { get; protected set; }

        public string CreationDate { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Address1 { get; set; }

        public string Status { get; set; }
    }
}