using System;

using PDR.Domain.Model;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiLicenseModel
    {
        public ApiLicenseModel()
        {
        }

        public ApiLicenseModel(License license)
        {
            if (license != null)
            {
                this.LicenseNumber = license.LicenseNumber;
                this.CreationDate = license.CreationDate;
                this.Status = license.Status.ToString();
                this.GpsReportFrequency = license.GpsReportFrequency;
                this.DeviceId = license.DeviceId;
                this.DeviceType = license.DeviceType.ToString();
                this.DeviceName = license.DeviceName;
                this.PhoneNumber = license.PhoneNumber;
            }
        }

        public string LicenseNumber { get; set; }

        public DateTime CreationDate { get; set; }

        public string Status { get; set; }

        public int GpsReportFrequency { get; set; }

        public string DeviceId { get; set; }

        public string DeviceType { get; set; }

        public string DeviceName { get; set; }

        public string PhoneNumber { get; set; }
    }
}