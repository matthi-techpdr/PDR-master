﻿using PDR.Web.Core.Authorization;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

namespace PDR.Web.Areas.Accountant.Controllers
{
    [PDRAuthorize(Roles = "Accountant")]
    public class SettingsController : Common.Controllers.SettingsController
    {
        public SettingsController(ICurrentWebStorage<Employee> storage, ICompanyRepository<License> licensesRepository, ICompanyRepository<Employee> employeeRepository)
            : base(storage, licensesRepository, employeeRepository)
        {
            
        }
    }
}