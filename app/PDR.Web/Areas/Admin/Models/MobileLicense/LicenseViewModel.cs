using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Admin.Models.MobileLicense
{
    public class LicenseViewModel : IViewModel
    {
        public string Id { get; set; }

        public string LicenseNumber { get; set; }

        public DateTime CreationDate { get; set; }

        public LicenseStatuses Status { get; set; }

        public string GpsFrequency { get; set; }

        public List<SelectListItem> GpsReportFrequencies { get; set; }

        public string DeviceId { get; set; }

        public DeviceTypes DeviceType { get; set; }

        public string DeviceName { get; set; }

        public string PhoneNumber { get; set; }

        public string Employee { get; set; }

        public List<SelectListItem> Employees { get; set; }
    }
}