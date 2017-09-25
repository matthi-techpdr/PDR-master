using System.Web.Mvc;

using PDR.Domain.Services.TempImageStorage;
using PDR.Web.Areas.Default.Controllers.Base;

namespace PDR.Web.Areas.Default.Controllers
{
    public class ImagesController : PDRController
    {
        private readonly ITempImageManager tempImageManager;

        public ImagesController(ITempImageManager tempImageManager)
        {
            this.tempImageManager = tempImageManager;
        }

        public JsonResult UploadToTemp()
        {
            var files = this.Request.Files;
            var uploadedFiles = new object[files.Count];
            for (int i = 0; i < files.Count; i++)
            {
                var imgInfo = this.tempImageManager.Save(files.Get(i));
                uploadedFiles[i] = new { id = imgInfo.Id, fullSizeUrl = imgInfo.FullSizeUrl, thumbnailUrl = imgInfo.ThumbnailUrl };
            }

            return new JsonResult { Data = new { files = uploadedFiles }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult RemoveTemp(string id)
        {
            this.tempImageManager.Remove(id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
