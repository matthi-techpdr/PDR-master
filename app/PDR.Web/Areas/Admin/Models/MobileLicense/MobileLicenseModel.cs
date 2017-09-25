using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PDR.Domain.Model.Enums;

namespace PDR.Web.Areas.Admin.Models.MobileLicense
{
    public class MobileLicenseModel
    {
        public long Id { get; set; }

        public string LicenseNumber { get; set; }

        public DateTime CreationDate { get; set; }

        public LicenseStatuses Status { get; set; }

        public string GpsFrequency { get; set; }

        public List<SelectListItem> GpsReportFrequencies { get; set; }

        public string DeviceId { get; set; }

        public DeviceTypes DeviceType { get; set; }

        public string DeviceName { get; set; }
        
        public string PhoneNumber { get; set; }

        public long Employee { get; set; }

        public List<SelectListItem> Employees { get; set; }

        public bool Edit { get; set; }

        public string EmployeeName { get; set; }
    }
}