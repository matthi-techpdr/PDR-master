using System.IO;
using System.Web.Mvc;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

using PDR.Web.Areas.Admin.Models.ComapnyInfo;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.NLog.FileLoggers;

using SmartArch.Web.Attributes;

namespace PDR.Web.Areas.Admin.Controllers
{
    [PDRAuthorize(Roles = "Admin")]
    public class CompanyInfoController : Controller
    {
        private readonly Employee currentEmployee;

        private readonly INativeRepository<Company> companiesrepository;

        private FileContentResult TemporaryLogo
        {
            get
            {
                var session = this.Session["logoEx"];
                return session != null ? (FileContentResult)session : null;
            }
            set
            {
                this.Session["logoEx"] = value;
            }
        }

        public CompanyInfoController(
            ICurrentWebStorage<Employee> storage,
            INativeRepository<Company> companiesrepository)
        {
            this.companiesrepository = companiesrepository;
            this.currentEmployee = storage.Get();
        }

        public ActionResult Index()
        {
            this.TemporaryLogo = null;
            var currentCompany = this.currentEmployee.Company;
            var model = new CompanyInfoModel(currentCompany);
            return this.View(model);
        }

        [HttpPost]
        [Transaction]
        [ValidateInput(false)] 
        public ActionResult Index(CompanyInfoModel model)
        {
            if (ModelState.IsValid)
            {
                var company = this.currentEmployee.Company;
                company.Name = model.Name;
                company.Address1 = model.Address1;
                company.Address2 = model.Address2;
                company.State = (StatesOfUSA)model.State;
                company.Zip = model.Zip;
                company.City = model.City;
                company.PhoneNumber = model.PhoneNumber;
                company.Email = model.Email;
                company.DefaultHourlyRate = model.DefaultHourlyRate;
                company.LimitForBodyPartEstimate = model.LimitForBodyPartEstimate;
                company.EstimatesEmailSubject = model.EstimateEmailSubject;
                company.RepairOrdersEmailSubject = model.RepairOrderEmailSubject;
                company.InvoicesEmailSubject = model.InvoiceEmailSubject;
                company.Notes = model.Notes;
                if (this.TemporaryLogo != null)
                {
                    company.Logo.PhotoFull = this.TemporaryLogo.FileContents;
                    company.Logo.ContentType = this.TemporaryLogo.ContentType;
                }

                CommonLogger.UpdateCompanyInfo(model, this.TemporaryLogo != null);
                this.companiesrepository.Save(company);
                this.TemporaryLogo = null;
                @ViewBag.Success = "Update was successfull!";
            }

            return this.View(model);
        }

        public FileContentResult RenderLogo()
        {
            if (this.TemporaryLogo != null)
            {
                return this.TemporaryLogo;
            }

            var logo = this.currentEmployee.Company.Logo;
            return this.File(logo.PhotoFull, logo.ContentType); 
        }

        public void SaveExample()
        {
            var files = HttpContext.Request.Files;
            var file = files[0];
            var logoContentType = file.ContentType;
            var memoryStream = new MemoryStream();
            file.InputStream.CopyTo(memoryStream);
            var logo = memoryStream.ToArray();
            this.TemporaryLogo = this.File(logo, logoContentType);
        }
    }
}
