using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.LogReceiverService;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Services.ObjectsComparer;
using PDR.Web.Areas.SuperAdmin.Models;

namespace PDR.Web.Core.NLog.FileLoggers.Superadmin
{
    public static class CompanyLogger
    {
        private static readonly Logger Loggger = LogManager.GetCurrentClassLogger();

        public static void New(CompanyViewModel company)
         {
            var msg = "Create company - ";

            var param = new List<string>
                {
                    company.Name,
                    company.Address1,
                    company.Address2,
                    ((StatesOfUSA)company.State).ToString(),
                    company.City,
                    company.Zip,
                    company.PhoneNumber,
                    company.Email,
                    company.AdminLogin,
                    company.AdminName,
                    company.Url,
                    company.MobileLicensesNumber.ToString(),
                    company.ActiveUsersNumber.ToString(),
                    company.Comment
                };

            msg = msg + string.Join("; ", param.Where(x => !string.IsNullOrEmpty(x)));
            Loggger.Info(msg);
         }

        public static void Suspend(Company company)
        {
            var msg = "Suspend company – " + company.Name + "; Suspended.";
            Loggger.Info(msg);
        }

        public static void Reactivate(Company company)
        {
            var msg = "Re-activate company – " + company.Name + "; Active.";
            Loggger.Info(msg);
        }

        public static void Edit(CompanyViewModel oldModel, CompanyViewModel newModel)
        {
            var changes = new LogHelper().Compare(oldModel, newModel, new StringCollection { "State" });
            var msg = "Edit company -";
            msg = msg + changes;
            if (oldModel.State != newModel.State)
            {
                msg = msg + string.Format("State: {0};", ((StatesOfUSA)newModel.State).ToString());
            }

            Loggger.Info(msg);
        }
    }
}