using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;
using NLog;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.Mailing;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.PushNotification;
using PDR.Domain.Services.Webstorage;

using PDR.Resources.Web;

using PDR.Web.Core.Formatters;
using PDR.Web.WebAPI.Authorization;
using PDR.Web.WebAPI.IphoneModels;

using SmartArch.Web.Attributes;

using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.WebAPI.HelpControllers
{
    public class CompanyController : Controller
    {
        private readonly INativeRepository<VersioniPhoneApp> versions; 

        private readonly INativeRepository<Company> companies;

        private readonly IMailService mailService;

        private readonly IPdfConverter converter;

        private readonly IRepositoryFactory repositoryFactory;

        private readonly ILogger logger;

        private readonly string[] exceptionalBuildVersions = {"1.0.(null)"};

        public CompanyController(
            INativeRepository<VersioniPhoneApp> versions,
            INativeRepository<Company> companies,
            IMailService mailService,
            IPdfConverter converter,
            IRepositoryFactory repositoryFactory,
            ILogger logger)
        {
            this.versions = versions;
            this.companies = companies;
            this.mailService = mailService;
            this.converter = converter;
            this.repositoryFactory = repositoryFactory;
            this.logger = logger;
        }

        public ActionResult GetCompanyInfo(string name)
        {
            var company = this.companies.SingleOrDefault(x => x.Name == name);
            if (company != null)
            {
                var logo = company.Logo;
                this.Response.AddHeader("url", company.Url);
                return this.File(logo.PhotoFull, logo.ContentType);
            }

            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            ErrorModel error=new ErrorModel("Company with the same name not found.");
            return new JsonNetResult(new { error }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckVersionApp(string version)
        {
            var flag = false;
            if (!exceptionalBuildVersions.Contains(version))
            {
                var vers = versions.SingleOrDefault(x => x.Version == version);

                if (vers != null)
                {
                    flag = vers.IsWorkingBild;
                }
                else
                {
                    string warnigMsg = String.Format("App version {0} wasn't found in data base", version);
                    logger.Log(LogLevel.Warn, warnigMsg);
                }
            }
            
            return new JsonNetResult(new {IsWorkingVersion = flag }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Insurances()
        {
            var insurances = this.repositoryFactory.CreateForCompany<InsuranceCompany>().Select(x => x.Name).Distinct().ToList();
            insurances.Sort();
            CompanyListModel companyList=new CompanyListModel(insurances);

            return new JsonNetResult(new { companyList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ApiAuthorize]
        [Transaction]
        public ActionResult SendEmail(EmailModel model)
        {
            try
            {
                var from = string.Empty;
                string fileName;
                var subject = model.Subject ?? string.Empty;
                var attachments = new List<Attachment>();
                if (model.EstimateId != 0)
                {
                    var estimates = this.repositoryFactory.CreateForCompany<Estimate>();
                    var estimate = estimates.SingleOrDefault(x => x.Id == model.EstimateId);

                    if (estimate != null)
                    {
                        from = estimate.Company.Email;
                        var pdf = this.converter.Convert(estimate, !model.IsBasic);
                        fileName = string.Format("Estimate #{0}_{1}.pdf", estimate.Id, estimate.CreationDate.ToString("MM-dd-yyyy"));
                        attachments.Add(new Attachment(new MemoryStream(pdf), fileName, "application/pdf"));
                        subject = model.Subject ?? estimate.Company.EstimatesEmailSubject;
                        this.logger.Log(estimate, EstimateLogActions.Email, model.Addresses);
                    }
                }

                if (model.RepairOrderId != 0)
                {
                    var repairOrders = this.repositoryFactory.CreateForCompany<RepairOrder>();
                    var repairOrder = repairOrders.SingleOrDefault(x => x.Id == model.RepairOrderId);

                    if (repairOrder != null)
                    {
                        from = repairOrder.Company.Email;
                        var pdf = this.converter.ConvertRepairOrder(repairOrder, !model.IsBasic);
                        fileName = string.Format("Repair Order #{0}_{1}.pdf", repairOrder.Id, repairOrder.CreationDate.ToString("MM-dd-yyyy"));
                        attachments.Add(new Attachment(new MemoryStream(pdf), fileName, "application/pdf"));
                        this.logger.Log(repairOrder, RepairOrderLogActions.Email, model.Addresses);
                    }
                }

                if (model.InvoiceId != 0)
                {
                    var invoices = this.repositoryFactory.CreateForCompany<Invoice>();
                    var invoice = invoices.SingleOrDefault(x => x.Id == model.InvoiceId);

                    if (invoice != null)
                    {
                        from = invoice.Company.Email;
                        var pdf = this.converter.ConvertInvoice(invoice, !model.IsBasic);
                        fileName = string.Format("Invoice #{0}_{1}.pdf", invoice.Id, invoice.CreationDate.ToString("MM-dd-yyyy"));
                        attachments.Add(new Attachment(new MemoryStream(pdf), fileName, "application/pdf"));
                        subject = model.Subject ?? invoice.Company.InvoicesEmailSubject;
                        this.logger.Log(invoice, InvoiceLogActions.Email, model.Addresses);
                    }
                }

                this.mailService.Send(from, model.Addresses, subject, model.Message, attachments.Count > 0 ? attachments : null);

                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }

        [ApiAuthorize]
        [Transaction]
        [HttpPost]
        public HttpStatusCodeResult SetSignatureName(string name)
        {
            var employee = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
            if (employee != null)
            {
                employee.SignatureName = name;
                this.repositoryFactory.CreateForCompany<Employee>().Save(employee);
                return new HttpStatusCodeResult(200);
            }
            
            return new HttpStatusCodeResult(404, "The employee not found.");
        }

        [ApiAuthorize]
        public JsonResult NewActivityAmount()
        {
            var teamSelector = this.Request.Url.ParseQueryString()["team"];
            var push = ServiceLocator.Current.GetInstance<IPushNotification>();
            var employee = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
            var model = (NewActivityAmountModel)push.GetNewActivities(teamSelector, employee);
            return this.Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}