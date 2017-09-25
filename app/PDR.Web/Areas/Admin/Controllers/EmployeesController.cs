using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Areas.Accountant.Models.Employee;
using PDR.Web.Core.Authorization;

namespace PDR.Web.Areas.Admin.Controllers
{
    [PDRAuthorize(Roles = "Admin")]
    public class EmployeesController : Common.Controllers.EmployeesController
    {
        public EmployeesController(
            ICompanyRepository<Employee> employeeRepository,
            ICompanyRepository<Team> teamRepository,
            IGridMaster<Employee, EmployeeJsonData, EmployeeViewModel> employeeGridMaster,
            ICurrentWebStorage<Employee> storage,
            ReassignHelper reassignHelper,
            ICompanyRepository<Invoice> invoiceRepository,
            ICompanyRepository<FormerRI> formerRITechnicianRepository )
            : base(employeeRepository, teamRepository, employeeGridMaster, storage, reassignHelper, invoiceRepository, formerRITechnicianRepository)
        {
        }
    }
}
