using System.Collections.Generic;
using System.Linq;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.RepositoryExtenssions;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.PushNotification;

using PDR.Resources.Web.WebApi;

using SmartArch.Data;
using SmartArch.Data.Fetching;
using SmartArch.Data.Proxy;

namespace PDR.Domain.Helpers
{
    public class ReassignHelper
    {
        private readonly ICompanyRepository<Estimate> estimatesRepository;

        private readonly ICompanyRepository<Manager> managersRepository;

        private readonly ICompanyRepository<Employee> employeesRepository;

        private readonly ICompanyRepository<TeamEmployee> teamEmployees;

        private readonly ICompanyRepository<RepairOrder> roRepository;

        private readonly ILogger logger;

        private readonly IPushNotification push;

        public ReassignHelper(
            ICompanyRepository<Estimate> estimatesRepository,
            ICompanyRepository<RepairOrder> roRepository,
            ICompanyRepository<Manager> managersRepository,
            ICompanyRepository<Employee> employeesRepository,
            ILogger logger,
            ICompanyRepository<TeamEmployee> teamEmployees,
            IPushNotification push)
        {
            this.estimatesRepository = estimatesRepository;
            this.managersRepository = managersRepository;
            this.roRepository = roRepository;
            this.logger = logger;
            this.teamEmployees = teamEmployees;
            this.employeesRepository = employeesRepository;
            this.push = push;
        }

        public IEnumerable<Employee> GetReassignEmployees(Employee employee, long[] estimatesIds, bool forConvert)
        {
            switch (employee.Role)
            {
                case UserRoles.Technician:
                    return this.GetTeamEmployees((TeamEmployee)employee, estimatesIds, forConvert);
                case UserRoles.Manager:
                    return this.GetTeamEmployees((TeamEmployee)employee, estimatesIds, forConvert);
                case UserRoles.Admin:
                    return this.GetAllEmployyesForReassign((TeamEmployee)employee, estimatesIds, forConvert);
                case UserRoles.Estimator:
                    var employees = this.GetEstimateManagers(estimatesIds);
                    return employees != null ? employees.Cast<Employee>().ToList() : null;
                default:
                    return null;
            }
        }

        public IList<TeamEmployee> GetEstimateManagers(long[] estimatesIds)
        {
            List<TeamEmployee> managers;
            var allEstimates = this.estimatesRepository.Where(e => estimatesIds.Contains(e.Id)).Fetch(x => x.Customer).ToList();
            var firstEstimateCustomer = allEstimates.First().Customer;
            if (firstEstimateCustomer.CustomerType == CustomerType.Wholesale)
            {
                if (allEstimates.Any(estimate => estimate.Customer != firstEstimateCustomer))
                {
                    return null;
                }

                managers = firstEstimateCustomer.Teams.SelectMany(x => x.Employees).Where(x => x is Manager).Distinct().ToList();
            }
            else
            {
                if (allEstimates.Any(estimate => estimate.Customer is WholesaleCustomer))
                {
                    return null;
                }

                managers = this.managersRepository.Cast<TeamEmployee>().ToList();
            }

            managers.AddRange(this.teamEmployees.Where(x => x is Admin).ToList());
            return managers.OnlyActive();
        }

        public void ReassignEstimate(long[] estimatesIds, long employeeId, Employee currentEmployee)
        {
            var employee = this.employeesRepository.Get(employeeId);
            var allEstimates = this.estimatesRepository.Where(x => estimatesIds.Contains(x.Id)).ToList();
            var setNew = employeeId != currentEmployee.Id;
            foreach (var estimate in allEstimates)
            {
                var oldEmployee = estimate.Employee;
                oldEmployee.PreviousEstimates.Add(estimate);


                estimate.Employee = employee;
                estimate.New = setNew;
                if (estimate.Customer is RetailCustomer && employee is Manager)
                {
                    var manager = (Manager)employee;
                    foreach (var mTeam in manager.Teams)
                    {
                        estimate.Customer.Teams.Add(mTeam);
                    }
                }

                this.estimatesRepository.Save(estimate);
                this.employeesRepository.Save(oldEmployee);
                this.logger.LogReassign(estimate, oldEmployee, currentEmployee);
                if (setNew)
                {
                    this.push.Send(employee, string.Format(NotificationMessages.AssignedToEstimate, estimate.Id));
                }
            }
        }

        public bool ConvertToRepairOrder(Estimate estimate, TeamEmployee teamEmployee, Employee currentEmployee)
        {
            if (estimate.EstimateStatus == EstimateStatus.Approved)
            {
                estimate.EstimateStatus = EstimateStatus.Converted;
                estimate.Archived = true;

                var setNew = currentEmployee.Id != teamEmployee.Id;

                var repairOrder = new RepairOrder(true)
                    {
                        Estimate = estimate,
                        TeamEmployee = teamEmployee,
                        New = setNew,
                        Customer = estimate.Customer,
                        WorkByThemselve = estimate.WorkByThemselve
                    };

                repairOrder.TeamEmployeePercents.Add(
                    new TeamEmployeePercent(true)
                        {
                            EmployeePart = 100,
                            RepairOrder = repairOrder,
                            TeamEmployee = teamEmployee
                        });

                this.roRepository.Save(repairOrder);
                this.estimatesRepository.Save(estimate);
                this.logger.LogConvert(estimate, repairOrder.TeamEmployee, currentEmployee);
                if (setNew)
                {
                    this.push.Send(teamEmployee, string.Format(NotificationMessages.AssignedToRepairOrder, repairOrder.Id));
                }
                
                return true;
            }

            return false;
        }

        public bool ConvertToRepairOrder(long estimateId, long managerId, Employee currentEmploye)
        {
            TeamEmployee teamEmployee = this.teamEmployees.Get(managerId);
            var estimate = this.estimatesRepository.Get(estimateId);
            var result = this.ConvertToRepairOrder(estimate, teamEmployee, currentEmploye);
            return result;
        }

        public IList<Employee> GetAllEmployyesForReassign(TeamEmployee employee, long[] estimatesIds, bool forConvert)
        {
            var estimates = this.estimatesRepository.Where(x => estimatesIds.Contains(x.Id)).Fetch(x => x.Customer).ToList();
            var firstEstimateCustomer = estimates.First().Customer;
            List<Employee> employees;

            if (firstEstimateCustomer.CustomerType == CustomerType.Wholesale)
            {
                if (estimates.Any(estimate => estimate.Customer != firstEstimateCustomer))
                {
                    return null;
                }

                employees = firstEstimateCustomer.Teams.SelectMany(x => x.Employees).Where(x => x.Status == Statuses.Active).Cast<Employee>().ToList();
            }
            else
            {
                if (estimates.Any(estimate => estimate.Customer is WholesaleCustomer))
                {
                    return null;
                }

                employees = this.employeesRepository.Where(x => x is Manager || x is Technician || x is RITechnician).ToList();
            }

            employees.AddRange(this.teamEmployees.Where(x => x is Admin && x.Status == Statuses.Active).ToList());
            if (!forConvert)
            {
                var allEstimators = this.employeesRepository.Where(x => x is Estimator).ToList();
                employees = employees.Union(allEstimators).ToList();

                var firstEmployee = estimates.First().Employee;
                if (estimates.Select(x => x.Employee).All(x => x.Id == firstEmployee.Id))
                {
                    employees = employees.Except(new[] { firstEmployee }).ToList();
                }
            }
            return employees.Distinct().ToList().OnlyActive();
        }

        public IList<TeamEmployee> GetTeamEmployees(TeamEmployee teamEmployee, long[] estimatesIds, bool forConvert)
        {
            List<TeamEmployee> employees;
            var allEstimates = this.estimatesRepository.Where(x => estimatesIds.Contains(x.Id)).Fetch(x => x.Customer).ToList();
            var firstEstimateCustomer = allEstimates.First().Customer;

            var managerTeams = teamEmployee.Teams;
            var customerTeams = firstEstimateCustomer.Teams;
            var teams = customerTeams.Count == 0 ? managerTeams : customerTeams.Intersect(managerTeams);

            employees = teams.SelectMany(x => x.Employees
                .Where(y => y.Role == UserRoles.Technician
                        || y.Role == UserRoles.Manager || y.Role == UserRoles.RITechnician))
                .Where(x => x.Status == Statuses.Active).Distinct().ToList();

            //if (firstEstimateCustomer.CustomerType == CustomerType.Wholesale)
            //{
            //    if (allEstimates.Any(estimate => estimate.Customer != firstEstimateCustomer))
            //    {
            //        return null;
            //    }

            //    employees = firstEstimateCustomer.Teams.Intersect(teamEmployee.Teams).SelectMany(x => x.Employees).Where(x => x.Status == Statuses.Active).ToList();
            //}
            //else
            //{
            //    if (allEstimates.Any(estimate => estimate.Customer is WholesaleCustomer))
            //    {
            //        return null;
            //    }

            //    employees = teamEmployee.Teams.SelectMany(x => x.Employees).Where(x => x.Status == Statuses.Active).ToList();
            //}

            var currentEmployee = allEstimates.First().Employee.ToPersist<TeamEmployee>();
            var currentEmployeeId = currentEmployee != null ? currentEmployee.Id : 0;
            var estimateEmployees = allEstimates.Select(x => x.Employee).ToList();
            var allEstimatesForCurrentEmployee = estimateEmployees.All(e => e.Id == currentEmployeeId);
            employees.AddRange(this.teamEmployees.Where(x => x is Admin && x.Status == Statuses.Active).ToList());

            if (!forConvert && allEstimatesForCurrentEmployee)
            {
                employees = employees.Except(new List<TeamEmployee> { currentEmployee }).ToList();
            }
            return employees.Distinct().ToList().OnlyActive();
        }

        public void AddEmployeesToRepairOrder(RepairOrder ro, IEnumerable<TeamEmployee> employees, Employee currentEmp)
        {
            var currentEmployees = ro.TeamEmployeePercents.Select(x => x.TeamEmployee);
            var newEmployees = employees.Except(currentEmployees).ToList();
            foreach (var teamEmployee in newEmployees)
            {
                ro.TeamEmployeePercents.Add(new TeamEmployeePercent(true) { RepairOrder = ro, TeamEmployee = teamEmployee });
            }
            var emp = ro.TeamEmployeePercents.Where(x => x.TeamEmployee.Role != UserRoles.RITechnician);
            if (emp.Count() != 0)
            {
                double part = (double)100 / emp.Count();
                emp.ToList().ForEach(x => x.EmployeePart = part);
            }
            this.roRepository.Save(ro);
            this.logger.LogAssignToRo(ro);

            foreach (var teamEmployee in newEmployees.Where(x => x.Licenses.Any(y => y.Status == LicenseStatuses.Active)))
            {
                if (teamEmployee.Id != currentEmp.Id)
                {
                    ro.New = true;
                    this.push.Send(teamEmployee, string.Format(NotificationMessages.AssignedToRepairOrder, ro.Id));
                }
            }
        }

        public void RemoveEmployeesFromRepairOrder(RepairOrder ro, IEnumerable<TeamEmployee> employees)
        {
            foreach (var teamEmployee in employees)
            {
                var percent = ro.TeamEmployeePercents.SingleOrDefault(x => x.TeamEmployee == teamEmployee);
                ro.TeamEmployeePercents.Remove(percent);
                this.roRepository.Save(ro);
            }

            if (ro.TeamEmployeePercents.Count() != 0)
            {
                var ri = ro.TeamEmployeePercents.Where(x => x.TeamEmployee.Role == UserRoles.RITechnician);
                if (!ri.Any())
                {
                    ro.IsFlatFee = null;
                    ro.Payment = null;
                }

                var emp = ro.TeamEmployeePercents.Where(x => x.TeamEmployee.Role != UserRoles.RITechnician);
                if (emp.Count() != 0)
                {
                    double part = (double) 100 / emp.Count();
                    emp.ToList().ForEach(x => x.EmployeePart = part);
                }

                this.roRepository.Save(ro);
                if (employees.Any())
                {
                    this.logger.LogAssignToRo(ro);
                }
            }
        }
    }
}