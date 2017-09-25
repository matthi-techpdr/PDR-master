using System;
using System.Linq;
using AutoMapper;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Users;
using PDR.Web.Areas.Accountant.Models.Customer;
using PDR.Web.Areas.Accountant.Models.Employee;
using PDR.Web.Areas.SuperAdmin.Models;
using PDR.Web.Core.Helpers;

namespace PDR.Web.Core
{
    public static class AutomapperInitializer
    {
        public static void RegisterAutoMappers()
        {
            Mapper.CreateMap<string, long>().ConvertUsing(Convert.ToInt64);
            CreateMaps();
        }

        private static void CreateMaps()
        {
            Mapper.CreateMap<Company, CompanyViewModel>()
                .ForMember(x => x.AdminLogin,
                opt =>
                opt.MapFrom(x => x.Employees.Where(e => e.Role == UserRoles.Admin).OrderBy(i => i.Id).FirstOrDefault().Login))
                .ForMember(x => x.AdminName,
                opt =>
                opt.MapFrom(
                    x => x.Employees.Where(e => e.Role == UserRoles.Admin).OrderBy(i => i.Id).FirstOrDefault().Name))
                    .ForMember(x => x.States, opt => opt.MapFrom(x => ListsHelper.GetStates((int?) x.State)));

            Mapper.CreateMap<CompanyViewModel, Company>().ForMember(x => x.Id, y => y.Ignore());
            Mapper.CreateMap<EmployeeViewModel, Admin>().ForMember(x => x.Id, y => y.Ignore());
            Mapper.CreateMap<EmployeeViewModel, Estimator>().ForMember(x => x.Id, y => y.Ignore());
            Mapper.CreateMap<EmployeeViewModel, Accountant>().ForMember(x => x.Id, y => y.Ignore());
            Mapper.CreateMap<EmployeeViewModel, Technician>().ForMember(x => x.Id, y => y.Ignore());
            Mapper.CreateMap<EmployeeViewModel, RITechnician>().ForMember(x => x.Id, y => y.Ignore());
            Mapper.CreateMap<EmployeeViewModel, Manager>().ForMember(x => x.Id, y => y.Ignore());
            Mapper.CreateMap<CustomerViewModel, WholesaleCustomer>().ForMember(x => x.Id, y => y.Ignore());
            Mapper.CreateMap<AffiliatesViewModel, Affiliate>().ForMember(x => x.Id, y => y.Ignore());
            Mapper.CreateMap<Matrix, Matrix>().ForMember(x => x.Id, y => y.Ignore());
        }
    }
}