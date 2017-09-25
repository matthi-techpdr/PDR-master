using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using PDR.Domain.Model;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;
using PDR.Web.Areas.Admin.Models.ComapnyInfo;
using PDR.Web.Areas.SuperAdmin.Models;

namespace PDR.Web.Core.NLog.FileLoggers
{
    public static class CommonLogger
    {
        private static readonly Logger Loggger = LogManager.GetCurrentClassLogger();

        public static void View(Entity entity)
        {
            var typeName = entity.GetType().Name;
            typeName = typeName == "RepairOrder" ? "repair order" : typeName.ToLower();
            var msg = string.Format("View {0} - {1}.", typeName, entity.Id);
            Loggger.Info(msg);
        }

        public static void UpdateInvoicePaid(Invoice invoice)
        {
            var msg = string.Format("Update invoice paid total - {0}; ${1}.", invoice.Id, invoice.PaidSum);
            Loggger.Info(msg);
        }

        public static void MarkInvoiceAsPaid(Invoice invoice)
        {
            var msg = string.Format("Mark invoice as paid  - {0}; {1}; ${2}. Paid full.", invoice.Id, invoice.PaidDate, invoice.PaidSum);
            Loggger.Info(msg);
        }

        public static void ExportInvoiceToQuickBook(Invoice invoice)
        {
            var msg = string.Format("Export invoice to QuickBooks - {0}.", invoice.Id);
            Loggger.Info(msg);
        }

        public static void EditRepairOrder(RepairOrder ro)
        {
            var msg = new StringBuilder(string.Format("Edit repair order - {0}.", ro.Id));
            var supplements = ro.Supplements;
            var photos = ro.AdditionalPhotos;

            if (supplements.Count() != 0)
            {
                var joinedSupplements = string.Join("; ", supplements.Select(x => string.Format("{0} - ${1}", x.Description, x.Sum)));
                msg.Append(string.Format("Supplements: {0}.", joinedSupplements));
            }

            if (photos.Count() != 0)
            {
                msg.Append(string.Format("Photos: {0}.", photos.Count()));
            }

            msg.Append(string.Format("Status: {0}.", ro.RepairOrderStatus.ToString()));

            Loggger.Info(msg);
        }
        
        public static void UpdateCompanyInfo(CompanyInfoModel company, bool changedPhoto)
        {
            var msg = "Update company information - ";

            var param = new List<string>
                {
                    company.Name,
                    changedPhoto ? "Changed logo" : string.Empty,
                    company.Address1,
                    company.Address2,
                    ((StatesOfUSA)company.State).ToString(),
                    company.City,
                    company.Zip,
                    company.PhoneNumber,
                    company.Email,
                    company.EstimateEmailSubject,
                    company.InvoiceEmailSubject,
                    company.DefaultHourlyRate.ToString(),
                    company.LimitForBodyPartEstimate.ToString()
                };

            msg = msg + string.Join("; ", param.Where(x => !string.IsNullOrEmpty(x)));
            Loggger.Info(msg);
        }

        public static void UpdateWorkerInfo(WorkerViewModel model)
        {
            var msg = "Update worker information - ";

            var param = new List<string>
                {
                    model.Login,
                    model.Password
                };
            msg = msg + string.Join("; ", param.Where(x => !string.IsNullOrEmpty(x)));
            Loggger.Info(msg);
        }
    }
}