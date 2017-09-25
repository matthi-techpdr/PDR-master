using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;
using SmartArch.Data;

using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Manager.Controllers
{
    public class RepairOrderReportsController : Common.Controllers.ReportsControllers.RepairOrderReportsController
    {
        protected readonly IRepositoryFactory repositoryFactory;

        public RepairOrderReportsController(IRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory;
        }
    }
}
