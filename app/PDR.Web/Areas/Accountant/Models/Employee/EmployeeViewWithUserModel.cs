using FluentValidation.Attributes;
using PDR.Web.Areas.Accountant.Validators;

namespace PDR.Web.Areas.Accountant.Models.Employee
{
    public class EmployeeViewWithUserModel: EmployeeViewModel
    {
        public Domain.Model.Users.Employee CurrentUser { get; set; }

        public EmployeeViewWithUserModel()
        {
        }
        public EmployeeViewWithUserModel(bool forAdmin, Domain.Model.Users.Employee currentUser) : base(forAdmin)
        {
            this.CurrentUser = currentUser;
        }
        public EmployeeViewWithUserModel(Domain.Model.Users.Employee employee, bool forAdmin,Domain.Model.Users.Employee currentUser)
            : base(employee, forAdmin)
        {
            this.CurrentUser = currentUser;
        }
    }
}
