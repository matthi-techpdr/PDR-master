using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Model.Enums;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Web.Areas.Accountant.Models.Employee;

namespace PDR.Web.Core.Helpers
{
    public static class EmployeeGridHelper
    {
        public static dynamic GetEmployeeGrid(UserRoles role)
        {
            if (role == UserRoles.Technician)
            {
                var grid =
                    ServiceLocator.Current.GetInstance<IGridMaster<Domain.Model.Users.Technician, EmployeeJsonData, EmployeeViewModel>>();
                grid.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                return grid;
            }
            if (role == UserRoles.Manager)
            {
                var grid =
                    ServiceLocator.Current.GetInstance<IGridMaster<Domain.Model.Users.Manager, EmployeeJsonData, EmployeeViewModel>>();
                grid.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                return grid;
            }
            if (role == UserRoles.Accountant)
            {
                var grid =
                    ServiceLocator.Current.GetInstance<IGridMaster<Domain.Model.Users.Accountant, EmployeeJsonData, EmployeeViewModel>>();
                grid.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                return grid;
            }
            if (role == UserRoles.Estimator)
            {
                var grid =
                    ServiceLocator.Current.GetInstance<IGridMaster<Domain.Model.Users.Estimator, EmployeeJsonData, EmployeeViewModel>>();
                grid.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                return grid;
            }
            if (role == UserRoles.Admin)
            {
                var grid =
                    ServiceLocator.Current.GetInstance<IGridMaster<Domain.Model.Users.Admin, EmployeeJsonData, EmployeeViewModel>>();
                grid.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                return grid;
            }
            if (role == UserRoles.RITechnician)
            {
                var grid =
                    ServiceLocator.Current.GetInstance<IGridMaster<Domain.Model.Users.RITechnician, EmployeeJsonData, EmployeeViewModel>>();
                grid.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
                return grid;
            }

            var employeeGrid =
                    ServiceLocator.Current.GetInstance<IGridMaster<Domain.Model.Users.Employee, EmployeeJsonData, EmployeeViewModel>>();
            employeeGrid.ExpressionFilters.Add(x => x.Status != Statuses.Removed);
            return employeeGrid;
        }
    }
}