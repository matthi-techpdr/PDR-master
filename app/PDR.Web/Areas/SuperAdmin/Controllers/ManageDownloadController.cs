using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

using Ionic.Zip;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.PushNotification;

using PDR.Web.Areas.SuperAdmin.Models;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.NLog.FileLoggers;

using SmartArch.Web.Attributes;

namespace PDR.Web.Areas.SuperAdmin.Controllers
{
    [PDRAuthorize(Roles = "Superadmin")]
    public class ManageDownloadController : Controller
    {
        private readonly INativeRepository<Domain.Model.Users.Worker> workers;

        private readonly ISuperadminGridMaster<VersioniPhoneApp, IphoneAppJsonModel, IphoneAppViewModel> iPhoneAppGridMaster;

        private readonly INativeRepository<Domain.Model.Users.Employee> employees;

        private readonly INativeRepository<VersioniPhoneApp> versioniPhoneAppRepository; 

        private readonly IPushNotification push;

        private string pathFilesstorage;

        private const string BuildeFileName = "PDRManage.ipa"; 

        public ManageDownloadController(INativeRepository<Domain.Model.Users.Worker> workers, 
            INativeRepository<Domain.Model.Users.Employee> employees, 
            INativeRepository<VersioniPhoneApp> versioniPhoneAppRepo, 
            ISuperadminGridMaster<VersioniPhoneApp, IphoneAppJsonModel, IphoneAppViewModel> iPhoneAppGridMaster )
        {
            this.iPhoneAppGridMaster = iPhoneAppGridMaster;
            this.workers = workers;
            this.push = ServiceLocator.Current.GetInstance<IPushNotification>();
            this.employees = employees;
            this.versioniPhoneAppRepository = versioniPhoneAppRepo;
            this.pathFilesstorage = ConfigurationManager.AppSettings["PathFilesStorage"];
        }

        public ActionResult Index()
        {
            var worker = this.workers.First();
            return View(new WorkerViewModel{ Login = worker.Login,  Password = worker.Password });
        }

        [HttpPost]
        [Transaction]
        public ActionResult Index(WorkerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var worker = this.workers.First();
            worker.Login = model.Login;
            worker.Password = model.Password;

            CommonLogger.UpdateWorkerInfo(model);
            this.workers.Save(worker);
            @ViewBag.Success = "Update was successfull!";

            return this.View(model);
        }

        [Transaction]
        public ActionResult SaveExample()
        {
            var files = HttpContext.Request.Files;
            var file = files[0];

            var unpackDirectory = Server.MapPath("~/Content/Builds/unpack/");
            var unpuckSubDirectory = "Payload/PDR Manage.app/";
            var tempDirectory = Server.MapPath("~/Content/Builds/temp/");
            var plistFile = Server.MapPath("~/Content/Builds/PDRManage.plist");

            EmptyDirectory(new DirectoryInfo(tempDirectory));
            EmptyDirectory(new DirectoryInfo(unpackDirectory));

            using (var memoryStream = new MemoryStream() )
            {
                if (file != null)
                {
                    file.SaveAs(this.Server.MapPath("~/Content/Builds/temp/" + BuildeFileName));
                    file.InputStream.CopyTo(memoryStream);
                }
            }

            if (!Directory.Exists(unpackDirectory))
            {
                Directory.CreateDirectory(unpackDirectory);
            }

            using (var zip = ZipFile.Read(Server.MapPath("~/Content/Builds/temp/" + BuildeFileName)))
            {
                foreach (var e in zip)
                {
                    e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            
            var infoPlistPath = string.Empty;
            foreach (var f in Directory.EnumerateFiles(unpackDirectory + unpuckSubDirectory, "Info.plist", SearchOption.TopDirectoryOnly))
            {
                infoPlistPath = f;
            }
           
            var xelement = XElement.Load(infoPlistPath);
            var bundleVersion = xelement.Elements().First().Elements().First(e => e.Value == "CFBundleVersion").ElementsAfterSelf().First();
            var bundleShortVersionString = xelement.Elements().First().Elements().First(e => e.Value == "CFBundleShortVersionString").ElementsAfterSelf().First();
           
            xelement = XElement.Load(plistFile);
            
            var urlManifestTag = xelement.Elements().First().Elements().First().ElementsAfterSelf().First().Elements().First().Elements()
                    .First().ElementsAfterSelf().First().Elements().First().Elements().First(e => e.Value == "url").ElementsAfterSelf().First();
            
            var aut = System.Web.HttpContext.Current.Request.Url.Authority;
            var downLoadUrlFromManifest = string.Format("https://{0}/Worker/Download/Build", aut);
            urlManifestTag.Value = downLoadUrlFromManifest;
            xelement.Save(plistFile); 
            
            var clearDerectory = new DirectoryInfo(unpackDirectory);
            EmptyDirectory(clearDerectory);

            var version = string.Format("{0}.{1}", bundleShortVersionString.Value, bundleVersion.Value);
            var existBuild = this.versioniPhoneAppRepository.FirstOrDefault(v => v.Version == version);

            if (existBuild != null)
            {
                existBuild.IsDownLoaded = true;
                EmptyDirectory( new DirectoryInfo(tempDirectory));
                return Json(new { msg = "Build of this version is already loaded on the server!", isError = true });
            }

            var allOldBuilds = this.versioniPhoneAppRepository.Where(a => a.IsDownLoaded).ToList();

            foreach (var versioniPhoneApp in allOldBuilds)
            {
                try
                {
                    var oldfileName = Path.GetFileNameWithoutExtension(versioniPhoneApp.LocalStoragePath);
                    var versionBuild = versioniPhoneApp.Version;
                    versionBuild = versionBuild.Replace(".", string.Empty);
                    var newfilename = string.Format("{0}{1}.ipa", oldfileName, versionBuild);
                    var newFilePath = this.Server.MapPath("~/Content/Builds/" + newfilename);

                    if (oldfileName != null && !System.IO.File.Exists(newFilePath))
                    {
                        System.IO.File.Move(this.Server.MapPath("~/Content/Builds/" + BuildeFileName), newFilePath);
                    }
                    versioniPhoneApp.LocalStoragePath = "~/Content/Builds/" + newfilename;
                    versioniPhoneApp.IsDownLoaded = false;
                }
                catch (Exception exp)
                {
                    System.Diagnostics.Debug.Write(exp.Message);
                    //this.Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    EmptyDirectory(new DirectoryInfo(tempDirectory));
                    EmptyDirectory(new DirectoryInfo(unpackDirectory));
                    //return Json(new { msg = exp.Message, isError = true });
                }
            }

            var newBild = new VersioniPhoneApp
            {
                IsWorkingBild = true,
                IsDownLoaded = true,
                DateUpload = DateTime.Now,
                LocalStoragePath = "~/Content/Builds/" + BuildeFileName,
                Version = version
            };
            try
            {
                this.versioniPhoneAppRepository.Save(newBild);
                System.IO.File.Move(this.Server.MapPath("~/Content/Builds/temp/" + BuildeFileName), this.Server.MapPath("~/Content/Builds/" + BuildeFileName));
            }
            catch (Exception exp)
            {
                //return Json(new { msg = exp.Message, isError = true });
            }
          
            //employees.ToList().ForEach(x => this.push.Send(x, string.Format(NotificationMessages.NewBuild, "ver. 1.0.545")));
            //var empLicense = employees.Where(x => x.Licenses.Any(y => y.Status == LicenseStatuses.Active) 
             //                                && x.Status == Statuses.Active && x.Id != 1 && x.Id != 2).ToList();
            //empLicense.ForEach(e=>this.push.Send(e, string.Format(NotificationMessages.NewBuild, version), null, null,true));

            return Json(new { msg = "Build uploaded successfully", isError = false });
        }

        [HttpGet]
        public JsonResult GetData(string sidx, string sord, int page, int rows, int? state)
        {
            var data = this.iPhoneAppGridMaster.GetData(page, rows, sidx, sord);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBuild(long? id)
        {
            var model = this.iPhoneAppGridMaster.GetEntityViewModel(id);
            return this.PartialView("EditIphoneBuild", model);
        }

        [HttpPost]
        [Transaction]
        public ActionResult EditIphoneBuild(IphoneAppViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.PartialView("EditIphoneBuild", model);
            }

            var dbWorkingBuild = this.versioniPhoneAppRepository.Where(v => v.IsWorkingBild).OrderByDescending(v=>v.Version).ToList();
            var dbDownLoadBuild = this.versioniPhoneAppRepository.FirstOrDefault(v => v.IsDownLoaded);
            var id = Convert.ToInt64(model.Id);
            var dbBuild = this.versioniPhoneAppRepository.FirstOrDefault(v => v.Id == id);

            if (!model.IsWorkingBild && dbDownLoadBuild != null && dbDownLoadBuild.Id.ToString() == model.Id)
            {
                foreach (var t in dbWorkingBuild.Where(t => t.Id.ToString() != model.Id))
                {
                    if(this.RenameToNotWorkingBuild(dbDownLoadBuild))
                    {
                        return Json("not found", JsonRequestBehavior.AllowGet);
                    }

                    if(this.RenameToWorkingBuild(t))
                    {
                        this.RenameToWorkingBuild(dbDownLoadBuild);
                        return Json("not found", JsonRequestBehavior.AllowGet);
                    }
                           
                    if (dbBuild != null)
                    {
                        dbBuild.IsWorkingBild = model.IsWorkingBild;
                        this.versioniPhoneAppRepository.Save(dbBuild);
                    }
                    break;
                }
            }
            else
            {
                if (dbDownLoadBuild == null || dbDownLoadBuild.Id.ToString() == model.Id)
                {
                    return Json("success", JsonRequestBehavior.AllowGet);
                }

                var  modelVersionBuidl = Convert.ToInt64(model.Version.Replace(".", string.Empty));
                var dbDownLoadVersionBuidl = Convert.ToInt64(dbDownLoadBuild.Version.Replace(".", string.Empty));
                    
                if (dbBuild == null)
                {
                    return Json("success", JsonRequestBehavior.AllowGet);
                }

                dbBuild.IsWorkingBild = model.IsWorkingBild;

                if (modelVersionBuidl > dbDownLoadVersionBuidl)
                {
                    if(this.RenameToNotWorkingBuild(dbDownLoadBuild))
                    {
                        return Json("not found", JsonRequestBehavior.AllowGet);
                    }

                    if(this.RenameToWorkingBuild(dbBuild))
                    {
                        this.RenameToWorkingBuild(dbDownLoadBuild);
                        return Json("not found", JsonRequestBehavior.AllowGet);
                    }
                }
                this.versioniPhoneAppRepository.Save(dbBuild);
            }
              
            //this.iPhoneAppGridMaster.EditEntity(model);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        private bool RenameToNotWorkingBuild(VersioniPhoneApp notWorkingBuild)
        {
            try
            {
                var oldfileName = Path.GetFileNameWithoutExtension(notWorkingBuild.LocalStoragePath);
                var versionBuild = notWorkingBuild.Version;
                versionBuild = versionBuild.Replace(".", string.Empty);
                var newfilename = string.Format("{0}{1}.ipa", oldfileName, versionBuild);
                var newFilePath = Server.MapPath("~/Content/Builds/" + newfilename);
                var fileBuild = Server.MapPath("~/Content/Builds/" + BuildeFileName);

                if (oldfileName != null)
                {
                    if (!System.IO.File.Exists(fileBuild))
                    {
                        return true;
                    }
                    
                    System.IO.File.Move(fileBuild, newFilePath);
                    notWorkingBuild.LocalStoragePath = "~/Content/Builds/" + newfilename;
                    notWorkingBuild.IsDownLoaded = false;
                    this.versioniPhoneAppRepository.Save(notWorkingBuild);
                }
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.Write(exp.Message);
            }

            return false;
        }
        
        private bool RenameToWorkingBuild(VersioniPhoneApp workingBuild)
        {
            try
            {
                var oldfileName = Path.GetFileNameWithoutExtension(workingBuild.LocalStoragePath);
                var newFilePath = Server.MapPath("~/Content/Builds/" + BuildeFileName);
                var fileMove = Server.MapPath(workingBuild.LocalStoragePath);

                if (oldfileName != null)
                {
                    if (!System.IO.File.Exists(fileMove))
                    {
                        return true;
                    }

                    System.IO.File.Move(fileMove, newFilePath);
                    workingBuild.LocalStoragePath = "~/Content/Builds/" + BuildeFileName;
                    workingBuild.IsDownLoaded = true;
                    this.versioniPhoneAppRepository.Save(workingBuild);
                }
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.Write(exp.Message);
            }

            return false;
        }

        private void EmptyDirectory( DirectoryInfo directory)
        {
            try
            {
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                {
                    subDirectory.Delete(true);
                }
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.Write(exp.Message); ;
            }
        }

        public ActionResult GetPushNotificationDialog()
        {
            var lastBuildVersion = this.versioniPhoneAppRepository.OrderByDescending(vb => vb.Version).FirstOrDefault();
            
            var msg = lastBuildVersion != null ? "Last version build: " + lastBuildVersion.Version : "No loaded build";
            
            return this.PartialView("~/Areas/SuperAdmin/Views/ManageDownload/GetpPushNotificationDialog.cshtml", new SendPushNotificationViewModel { Message = msg });
        }

        [Transaction]
        [HttpPost]
        public JsonResult SendPushNotification(SendPushNotificationViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Json(false, JsonRequestBehavior.AllowGet);
            }


            var empLicense = employees.Where(x => x.Licenses.Any(y => y.Status == LicenseStatuses.Active)
                                                  && x.Status == Statuses.Active && x.Id != 1 && x.Id != 2).ToList();
            empLicense.ForEach(e => this.push.Send(e, string.Format(model.Message), null, null, true));
            var message = string.Empty;

            return this.Json(message, JsonRequestBehavior.AllowGet);
        }
    }
}
