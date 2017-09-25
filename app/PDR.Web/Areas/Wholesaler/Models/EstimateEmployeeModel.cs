using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PDR.Web.Areas.Wholesaler.Models
{
    public class EstimateEmployeeModel
    {
        public EstimateEmployeeModel()
        {
            this.Employees = new List<SelectListItem>();
        }

        public EstimateEmployeeModel(IEnumerable<PDR.Domain.Model.Users.Employee> employees)
        {
            this.InitEmployees(employees);
        }

        protected void InitEmployees(IEnumerable<PDR.Domain.Model.Users.Employee> customers)
        {
            this.Employees = customers.OrderBy(m => m.Name).Select(x => new SelectListItem
                                                                            {
                                                                                Value = x.Id.ToString(),
                                                                                Text = x.Name.Length > 37
                                                                                        ? x.Name.Substring(0, 37) + "..."
                                                                                        : x.Name
                                                                            }).ToList();
            this.Employees.Insert(0, new SelectListItem { Value = "0", Text = string.Empty });
        }

        public long EmployeeId { get; set; }

        public IList<SelectListItem> Employees { get; private set; }

    }
}