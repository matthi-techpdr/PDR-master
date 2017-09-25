using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

using NHibernate;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Photos;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.Webstorage;

using PDR.Web.Core.NLog.FileLoggers;
using PDR.Web.WebAPI.Authorization;
using PDR.Web.WebAPI.WebApiRepoExtensions;

using SmartArch.Data;
using SmartArch.Web.Attributes;

namespace PDR.Web.WebAPI.HelpControllers
{
    [ApiAuthorize]
    public class PhotoUploaderController : Controller
    {
        private readonly ICompanyRepository<Estimate> estimates;

        private readonly Employee currentEmployee;

        private readonly ICompanyRepository<Photo> photos;

        private readonly ICompanyRepository<RepairOrder> repairOrders;

        private readonly ILogger logger;

        public PhotoUploaderController(
            ICompanyRepository<Estimate> estimates,
            ICompanyRepository<RepairOrder> repairOrders, 
            ICurrentWebStorage<Employee> storage,
            ICompanyRepository<Photo> photos,
            ILogger logger)
        {
            this.estimates = estimates;
            this.repairOrders = repairOrders;
            this.photos = photos;
            this.logger = logger;
            this.currentEmployee = storage.Get();
        }

        [HttpPost]
        [Transaction]
        public ActionResult UploadEstimatePhoto(long id)
        {
            var estimate = this.estimates.GetIfExist(id);
            var oldPhotos = estimate.Photos.Select(x => x.Id).ToList();
            var carPhotos = this.GetPhotos();
            foreach (var carPhoto in carPhotos)
            {
                var estimatePhoto = (CarPhoto)carPhoto;
                estimatePhoto.Estimate = estimate;
                estimatePhoto.Company = this.currentEmployee.Company;
                estimate.Photos.Add(estimatePhoto);
            }

            this.estimates.Save(estimate);

            var session = ServiceLocator.Current.GetInstance<ISession>();
            using (var transaction = session.BeginTransaction())
            {
                transaction.Commit();
                session.Flush();
            }

            var newPhotos = estimate.Photos.Select(x => x.Id).ToList().Except(oldPhotos).ToList();
            if (!newPhotos.Any())
            {
                return new HttpStatusCodeResult(500, "Not recieved photos.");
            }

            EstimateLogger.Edit(estimate);
            return this.Json(new { NewPhotos = newPhotos.Select(x => new { Id = x }) });
        }

        [HttpPost]
        [Transaction]
        public JsonResult UploadRepairOrderPhoto(long id)
        {
            var ro = this.repairOrders.GetIfExist(id);
            var oldPhotos = ro.AdditionalPhotos.Select(x => x.Id).ToList();
            var carPhotos = this.GetPhotos(true);
            foreach (var carPhoto in carPhotos)
            {
                var roPhoto = (AdditionalCarPhoto)carPhoto;
                roPhoto.RepairOrder = ro;
                roPhoto.Company = this.currentEmployee.Company;
                ro.AdditionalPhotos.Add(roPhoto);
            }

            this.repairOrders.Save(ro);

            var session = ServiceLocator.Current.GetInstance<ISession>();
            using (var transaction = session.BeginTransaction())
            {
                transaction.Commit();
                session.Flush();
            }
            
            var newPhotos = ro.AdditionalPhotos.Select(x => x.Id).ToList().Except(oldPhotos);
            CommonLogger.EditRepairOrder(ro);
            return this.Json(new { NewPhotos = newPhotos.Select(x => new { Id = x }) });
        }

        [HttpPost]
        [Transaction]
        public ActionResult CustomerApproval(long id)
        {
            var estimate = this.estimates.GetIfExist(id);

            var files = Request.Files;
            if (files.Count > 0)
            {
                var signature = files[0];
                if (signature == null)
                {
                    return new HttpStatusCodeResult(500, "File does not exist.");
                }

                var memoryStream = new MemoryStream();
                estimate.Signature = true;
                signature.InputStream.CopyTo(memoryStream);
                estimate.CustomerSignature = new CustomerSignature(memoryStream.ToArray(), signature.ContentType, true);
            }

            estimate.EstimateStatus = EstimateStatus.Approved;
            this.estimates.Save(estimate);
            this.logger.Log(estimate, EstimateLogActions.Approve);

            return new HttpStatusCodeResult(200);
        }

        private IEnumerable<Photo> GetPhotos(bool forRo = false)
        {
            var photoList = new List<Photo>();
            var files = Request.Files;
            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file != null)
                {
                    var memStream = new MemoryStream();
                    file.InputStream.CopyTo(memStream);
                    
                    ////================ Remove block after fix HTPDR-1153==============
                    //var f = Image.FromStream(memStream);
                    //var filepath = Server.MapPath("/content/images/upload/temp/");
                    //f.Save(filepath + "img.png", ImageFormat.Png);
                    ////==========================================================
                    
                    Photo photo;
                    if (forRo)
                    {
                        photo = new AdditionalCarPhoto(memStream.ToArray(), file.ContentType, true);
                    }
                    else
                    {
                        photo = new CarPhoto(memStream.ToArray(), file.ContentType, true);
                    }

                    photoList.Add(photo);
                }
            }

            return photoList;
        }

        public ActionResult DownloadPhoto(long id)
        {
            var photo = this.photos.Get(id);
            if (photo == null)
            {
                return new HttpStatusCodeResult(404, "Photo not found");
            }

            return this.File(photo.PhotoFull, photo.ContentType);
        }


        [HttpPost]
        [Transaction]
        public ActionResult DeletePhoto(long id)
        {
            var photo = this.photos.Get(id);
            if (photo == null)
            {
                return new HttpStatusCodeResult(404, "Photo not found");
            }

            this.photos.Remove(photo);
            return new HttpStatusCodeResult(200, "Photo successfully deleted.");
        }
    }
}
