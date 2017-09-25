using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using NLog;
using NLog.LogReceiverService;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Web.Areas.Accountant.Models.Customer;
using PDR.Web.Areas.Estimator.Models.Estimates;

namespace PDR.Web.Core.NLog.FileLoggers
{
    public class CustomerLogger
    {
        private static readonly Logger Loggger = LogManager.GetCurrentClassLogger();

        private static string GetMessage(CustomerViewModel model, string action)
        {
            var matrixes = ServiceLocator.Current
                .GetInstance<ICompanyRepository<Matrix>>()
                .Where(x => model.MatricesIds.Contains(x.Id)).ToList();

            var teams = ServiceLocator.Current
                .GetInstance<ICompanyRepository<Team>>()
                .Where(x => model.TeamsIds.Contains(x.Id)).ToList();

            var info = new List<string>
                {
                    model.Name, model.Address1, model.Address2, model.City, model.Zip, ((StatesOfUSA)int.Parse(model.State)).ToString(), model.Phone, model.Fax, model.ContactName, 
                    model.ContactTitle, model.Email, model.Password, model.Discount, model.LaborRate, model.HourlyRate, model.PartRate
                };

            info.RemoveAll(x => x == null);
            var infoString = string.Join("; ", info);
            var matrixString = string.Join("; ", matrixes.Select(x => x.Name));
            var teamsString = string.Join("; ", teams.Select(x => x.Title));
            var comment = !string.IsNullOrEmpty(model.Comment) ? model.Comment : "none";
            var settings = string.Empty;

            if (model.Insurance)
            {
                settings += "Insurance info is necessary in estimates; ";
            }

            if (model.EstimateSignature)
            {
                settings += "Signature is necessary for estimate; ";
            }

            if (model.OrderSignature)
            {
                settings += "Signature is necessary for work orders approval; ";
            }

            if (model.WorkByThemselve)
            {
                settings += "Customer perform R&R and R&l by themselves; ";
            }

            var settingsString = !string.IsNullOrEmpty(settings) ? settings : "none";
            var template =
                string.Format(
                    "{0} customer. Customer info: {1}. Price matrices: {2}. Teams: {3}. Comment: {4}. Settings: {5}", 
                    action, 
                    infoString, 
                    matrixString, 
                    teamsString, 
                    comment, 
                    settingsString);

            return template;
        }

        private static string GetMessage(AffiliatesViewModel model, string action)
        {
            var teams = ServiceLocator.Current
                .GetInstance<ICompanyRepository<Team>>()
                .Where(x => model.TeamsIds.Contains(x.Id)).ToList();

            var info = new List<string>
                {
                    model.Name, model.Address1, model.Address2, model.City, model.Zip, ((StatesOfUSA)int.Parse(model.State)).ToString(), model.Phone, model.Fax, model.ContactName, 
                    model.ContactTitle, model.Email, model.LaborRate, model.HourlyRate, model.PartRate
                };

            info.RemoveAll(x => x == null);
            var infoString = string.Join("; ", info);
            var teamsString = string.Join("; ", teams.Select(x => x.Title));
            var comment = !string.IsNullOrEmpty(model.Comment) ? model.Comment : "none";
            var settings = string.Empty;

            var settingsString = !string.IsNullOrEmpty(settings) ? settings : "none";
            var template =
                string.Format(
                    "{0} affiliate. Affiliate info: {1}. Teams: {2}. Comment: {3}. Settings: {4}",
                    action,
                    infoString,
                    teamsString,
                    comment,
                    settingsString);

            return template;
        }

        public static void Create(CustomerViewModel customer)
        {
            var msg = GetMessage(customer, "Create");
            Loggger.Info(msg);
        }

        public static void Create(AffiliatesViewModel affiliates)
        {
            var msg = GetMessage(affiliates, "Create");
            Loggger.Info(msg);
        }


        public static void Edit(CustomerViewModel customer)
        {
            var msg = GetMessage(customer, "Edit");
            Loggger.Info(msg);
        }

        public static void Edit(AffiliatesViewModel customer)
        {
            var msg = GetMessage(customer, "Edit");
            Loggger.Info(msg);
        }


        public static void Suspend(WholesaleCustomer customer)
        {
            var msg = string.Format("Suspend customer - {0}. Suspended.", customer.Name);
            Loggger.Info(msg);
        }

        public static void Suspend(Affiliate customer)
        {
            var msg = string.Format("Suspend customer - {0}. Suspended.", customer.Name);
            Loggger.Info(msg);
        }


        public static void Reactivate(WholesaleCustomer customer)
        {
            var msg = string.Format("Re-activate customer - {0}. Active.", customer.Name);
            Loggger.Info(msg);
        }

        public static void Reactivate(Affiliate customer)
        {
            var msg = string.Format("Re-activate customer - {0}. Active.", customer.Name);
            Loggger.Info(msg);
        }


        public static void SendEmail(SendEmailViewModel model)
        {
            var msg = string.Format("Send email to customer - {0}. {1}; {2}.", model.To, model.Subject, model.Message);
            Loggger.Info(msg);
        }
    }
}