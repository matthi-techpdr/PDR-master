using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Logging;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;

using PDR.Web.Areas.Admin.Models.GPS;
using PDR.Web.Core.Authorization;

using SmartArch.Data;

namespace PDR.Web.Areas.Admin.Controllers
{
    [PDRAuthorize(Roles = "Admin")]
    public class LocationsController : Controller
    {
        private readonly IGridMaster<License, LocationGridJsonModel, LicenseViewModel> gpsGridMaster;

        private readonly ICompanyRepository<License> licenseRepository;

        private readonly ICompanyRepository<Team> teamRepository;

        public LocationsController(
            IGridMaster<License, LocationGridJsonModel, LicenseViewModel> gpsGridMaster,
            ICompanyRepository<License> licenseRepository,
            ICompanyRepository<Team> teamRepository
            )
        {
            this.gpsGridMaster = gpsGridMaster;
            this.licenseRepository = licenseRepository;
            this.teamRepository = teamRepository;
        }

        public ActionResult Index()
        {
            var teams = this.teamRepository.Select(x => new SelectListItem
                                                            {
                                                                Text = x.Title,
                                                                Value = x.Id.ToString()
                                                            }).ToList();
            teams.Insert(0, new SelectListItem { Text = @"All teams" });
            return this.View(teams);
        }

        public ActionResult GetData(string sidx, string sord, int page, int rows, long? team)
        {
            this.gpsGridMaster.ExpressionFilters.Add(x => !(x.Employee is Domain.Model.Users.Accountant)); ////&& !(x.Employee is Domain.Model.Users.Admin)
            this.gpsGridMaster.ExpressionFilters.Add(x => x.Status == LicenseStatuses.Active);
            if (team.HasValue)
            {
                var currentTeam = this.teamRepository.Get(team.Value);
                this.gpsGridMaster.ExpressionFilters.Add(x => ((TeamEmployee)x.Employee).Teams.Contains(currentTeam));
            }

            var data = this.gpsGridMaster.GetData(page, rows, sidx, sord, orderExpression: this.GetSortExpression(sidx));
            return this.Json(data, JsonRequestBehavior.AllowGet);
        }

        private Expression<Func<License, object>> GetSortExpression(string sortId)
        {
            switch (sortId)
            {
                case "LastReportDate":
                    return d => d.Locations.Select(x => x.Date).Max();
                case "Role":
                    return n => n.Employee.Role;
                default:
                    return null;
            }
        }

        public JsonResult GetRoutes(string licensesId, string from, string to)
        {
            var licenses = licensesId.Split(',').Select(x => Convert.ToInt64(x)).ToList();
            var currentLicenses = this.licenseRepository.Where(x => licenses.Contains(x.Id)).ToList();
            Func<Location, bool> func;

            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
            {
                func = x => x.Date >= DateTime.Parse(from) && x.Date <= DateTime.Parse(to);
            }
            else
            {
                func = x => x.Date >= DateTime.Now.AddDays(-3);
            }

            var routes = new List<RouteModel>();
            foreach (var license in currentLicenses)
            {
                var locations = license.Locations.Where(func).OrderByDescending(x => x.Date).ToList();
                if (locations.Count != 0)
                {
                    routes.Add(new RouteModel(locations));
                }
            }

            return this.Json(routes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLastLocations()
        {
            var lastData = DateTime.Now.Date.AddDays(-10);
            var licenses = this.licenseRepository.ToList();
            var lastLocations = new List<LastLocationModel>();

            foreach (var license in licenses)
            {
                var locations = license.Locations;
                
                if (!locations.Any())
                {
                    continue;
                }
                
                var lastTenDays = locations.Where(x => x.Date >= lastData);
                var last = lastTenDays.OrderByDescending(x => x.Date).FirstOrDefault();
                
                if (last != null)
                {
                    lastLocations.Add(new LastLocationModel(last));
                }
            }
            return this.Json(lastLocations, JsonRequestBehavior.AllowGet);
        }
    }
}
