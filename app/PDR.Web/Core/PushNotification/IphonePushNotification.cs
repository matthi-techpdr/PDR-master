using System;
using System.IO;
using System.Linq;

using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.PushNotification;
using PDR.Web.WebAPI.IphoneModels;

using PushSharp;
using PushSharp.Apple;


namespace PDR.Web.Core.PushNotification
{
    using PDR.Domain.StoredProcedureHelpers;

    public class IphonePushNotification : IPushNotification
    {
        private readonly ApplePushService service;

        private readonly int badges = 0;

        public IphonePushNotification()
        {
            var settings = (IPushSettings)System.Configuration.ConfigurationManager.GetSection("pushNotification");
            //Current certificare HTPDRPushProd.p12 is valid till the 9th of January than it should be replaced by the new one
            var cert = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "content/certificates", settings.Certificate));
            try
            {
                this.service = new ApplePushService(new ApplePushChannelSettings(settings.Production, cert, settings.Password));
            }
            catch (Exception)
            {
            }
        }

        public void Send(Employee employee, string message, string customKey = null, object[] customObjects = null, bool isNewBuildNotificaions = false)
        {
            var license = employee.Licenses.SingleOrDefault(x => x.Status == LicenseStatuses.Active);
            if (license == null)
            {
                return;
            }

            var deviceToken = license.DeviceToken;

            if (deviceToken != null && this.service != null)
            {
                var notification = new 
                         AppleNotification()
                        .ForDeviceToken(deviceToken)
                        .WithAlert(message)
                        .WithSound("default")
                        .WithBadge(this.badges)
                        .WithCustomItem(customKey, customObjects);
                this.service
                    .QueueNotification(notification);
            }
        }
        
        public object GetNewActivities(string teamSelector, Employee currentEmployee)
        {
            var onlyOwn = false;
            long teamId;
            Int64.TryParse(teamSelector, out teamId);
            onlyOwn = !String.IsNullOrEmpty(teamSelector) && teamId == 0;

            var invoicesSpHelper = new InvoicesStoredProcedureHelper(currentEmployee.Id, isArchive: false, filterByTeam: teamId, isOnlyOwn: onlyOwn);

            var repairOrdersSpHelper = new RepairOrdersStoredProcedureHelper(currentEmployee.Id, isForReport:true, filterByTeam: teamId, isOnlyOwn: onlyOwn);

            var estimatesSpHelper = new EstimatesStoredProcedureHelper(currentEmployee.Id, teamId, isOnlyOwn: onlyOwn);

            var model = new NewActivityAmountModel
                {
                    Estimates = estimatesSpHelper.NewActivityAmount,
                    RepairOrders = repairOrdersSpHelper.NewActivityAmount,
                    Invoices = invoicesSpHelper.NewActivityAmount
                };

            return model;
        }
    }
}
