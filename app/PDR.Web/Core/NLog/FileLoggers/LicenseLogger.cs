using NLog;

using PDR.Domain.Model;

namespace PDR.Web.Core.NLog.FileLoggers
{
    public static class LicenseLogger
    {
        private static readonly Logger Loggger = LogManager.GetCurrentClassLogger();

        public static void Suspend(License license)
        {
            var msg = "Suspend license - " + license.Id;
            Loggger.Info(msg);
        }

        public static void Reactivate(License license)
        {
            var msg = "Re-activate license - " + license.Id;
            Loggger.Info(msg);
        }

        private static void Log(License license, string action)
        {
            var msg = string.Format(
                "{6} license - {0};{1};{2};{3};{4};{5} min.",
                license.Id,
                license.DeviceName,
                license.Employee.Name,
                license.DeviceType,
                license.PhoneNumber,
                license.GpsReportFrequency,
                action);
            Loggger.Info(msg);
        }

        public static void Create(License license)
        {
            Log(license, "Create");
        }

        public static void Edit(License license)
        {
            Log(license, "Edit");
        }

        public static void ClearDeviceId(License license)
        {
            var msg = string.Format("License {0} - clear Device ID and Token", license.Id);
            Loggger.Info(msg);
        }
    }
}