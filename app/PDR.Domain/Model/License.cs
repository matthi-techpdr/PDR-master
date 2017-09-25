using System;

using Iesi.Collections.Generic;

using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Logging;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model
{
    public class License : CompanyEntity
    {
        public virtual string LicenseNumber { get; set; }

        public virtual DateTime CreationDate { get; set; }

        public virtual LicenseStatuses Status { get; set; }

        public virtual int GpsReportFrequency { get; set; }

        public virtual string DeviceId { get; set; }

        public virtual DeviceTypes DeviceType { get; set; }

        public virtual string DeviceName { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual ISet<Location> Locations { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual string DeviceToken { get; set; }

        public License()
        {
            this.CreationDate = DateTime.Now;
            this.Locations = new HashedSet<Location>();
        }
    }
}
