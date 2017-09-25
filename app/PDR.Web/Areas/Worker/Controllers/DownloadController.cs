using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Web.Areas.Worker.Models;
using PDR.Web.Core.Authorization;

namespace PDR.Web.Areas.Worker.Controllers
{
    [PDRAuthorize(Roles = "Worker")]
    public class DownloadController : Controller
    {
        private readonly INativeRepository<VersioniPhoneApp> repository; 

        public DownloadController(INativeRepository<VersioniPhoneApp> repository)
        {
            this.repository = repository;
        }

        public ActionResult Index()
        {
            var aut = System.Web.HttpContext.Current.Request.Url.Authority;
            var dic = (Dictionary<Guid, long>) HttpContext.Application["Authorize"];
            
            ClearToken(dic);

            var guid = Guid.NewGuid();
            dic.Add(guid, DateTime.Now.Ticks);
            var downLoadUrl = string.Format("itms-services://?action=download-manifest&url=https://{0}/Worker/Download/Manifest%3Ftoken%3D{1}", aut, guid);

            var model = new DownloadBuildViewModel
                            {
                                UrlDownload = downLoadUrl,
                                TokenLogOut = guid.ToString(),
                                NoWorkingBuilds = this.repository.Any(x => x.IsWorkingBild && x.IsDownLoaded)
                            };

            if (Request.UserAgent == null)
            {
                return View(model);
            }

            if (Request.UserAgent.ToLower().Contains("iphone"))
            {
                return View("IndexMobile", model);
            }

            if (Request.UserAgent.ToLower().Contains("android")
                && Request.UserAgent.ToLower().Contains("mobile")
                && !Request.UserAgent.ToLower().Contains("firefox")
                && !Request.UserAgent.ToLower().Contains("tablet"))
            {
                return View("IndexMobile", model);
            }

            return View(model);
        }

        public ActionResult Manifest(string token)
        {
            const string path = "~/Content/Builds/PDRManage.plist";

            var plistFile = Server.MapPath(path);

            var xelement = XElement.Load(plistFile);

            var urlManifestTag = xelement.Elements().First().Elements().First().ElementsAfterSelf().First().Elements().First().Elements()
                    .First().ElementsAfterSelf().First().Elements().First().Elements().First(e => e.Value == "url").ElementsAfterSelf().First();

            var aut = System.Web.HttpContext.Current.Request.Url.Authority;
            var downLoadUrlFromManifest = string.Format("https://{0}/Worker/Download/Build?token={1}", aut, token);
            urlManifestTag.Value = downLoadUrlFromManifest;
            xelement.Save(plistFile);

            var file = System.IO.File.ReadAllBytes(plistFile);

            return File(file, "text/plain");
        }

        public ActionResult Build(string token)
        {
            var file = System.IO.File.ReadAllBytes(Server.MapPath("~/Content/Builds/PDRManage.ipa"));

            return File(file, "application/octet-stream");
        }

        private static void ClearToken(Dictionary<Guid, long> dic)
        {
            var keys = new List<Guid>(dic.Keys);

            foreach (var key in keys)
            {
                var ticks = dic[key];
                if (DateTime.Now.Ticks - ticks > new TimeSpan(2, 0, 0).Ticks)
                {
                    dic.Remove(key);
                }
            }
        }
    }
}
