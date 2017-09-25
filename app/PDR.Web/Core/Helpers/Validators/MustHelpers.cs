using System;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Web.Areas.Accountant.Models.Employee;
using PDR.Web.Areas.SuperAdmin.Models;

using SmartArch.Data;

namespace PDR.Web.Core.Helpers.Validators
{
    public static class MustHelpers
    {
        public static bool CompanyMustBeUnique(IViewModel model, Func<Company, bool> query)
        {
            var entities = ServiceLocator.Current.GetInstance<IRepository<Company>>().AsQueryable();
            return CheckUniqiue(entities, query, model);
        }

        public static bool EmployeeMustBeUnique(IViewModel model, Func<Employee, bool> query)
        {
            var entities = ServiceLocator.Current.GetInstance<ICompanyRepository<Employee>>().AsQueryable();
            var removedEmployees = ServiceLocator.Current.GetInstance<ICompanyRepository<Employee>>().Where(x => x.Status != Statuses.Removed).Select(x => x.Id).ToList();
            entities = entities.Where(x => removedEmployees.Contains(x.Id));
            return CheckUniqiue(entities, query, model);
        }

        public static bool MustBeUnique<T>(IViewModel model, Func<T, bool> query) where T : CompanyEntity
        {
            var entities = ServiceLocator.Current.GetInstance<ICompanyRepository<T>>().AsQueryable();
            return CheckUniqiue(entities, query, model);
        }

        public static bool CheckUniqiue<T>(IQueryable<T> entities, Func<T, bool> query, IViewModel model) where T : Entity
        {
            var entityByProperty = entities.SingleOrDefault(query);
            if (entityByProperty == null)
            {
                return true;
            }

            var id = model.Id == "null" ? null : model.Id;
            if (id != null)
            {
                return entityByProperty.Id.ToString() == id;
            }

            return false;
        }
    }
}
