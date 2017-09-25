using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PDR.Domain.Helpers;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.Admin.Models.MobileLicense
{
    public class LicenseJsonModel : IJsonModel
    {
        public LicenseJsonModel(Domain.Model.License license)
        {
            this.Id = license.Id;
            this.DeviceName = string.IsNullOrWhiteSpace(license.DeviceName) ? string.Empty : license.DeviceName;
            this.CreationDate = license.CreationDate.ToString("MM/dd/yyyy");
            this.DeviceId = string.IsNullOrWhiteSpace(license.DeviceId) ? string.Empty : license.DeviceId;
            this.DeviceType = license.DeviceType.ToString();
            this.Status = license.Status.ToString();
            this.Owner = license.Employee.With(x => x.Name);
            this.PhoneNumber = license.PhoneNumber;
        }

        public long Id { get; set; }

        public string CreationDate { get; set; }

        public string Status { get; set; }

        public string DeviceId { get; set; }

        public string DeviceType { get; set; }

        public string DeviceName { get; set; }

        public string PhoneNumber { get; set; }

        public string Owner { get; set; }
    }
}