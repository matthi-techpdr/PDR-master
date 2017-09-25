using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Photos;
using PDR.Web.Areas.Default.Controllers.Base;

namespace PDR.Web.Areas.Default.Controllers
{
    public class PhotosController : PDRController
    {
        private readonly ICompanyRepository<Photo> photos;

        public PhotosController(ICompanyRepository<Photo> photos)
        {
            this.photos = photos;
        }

        public FileContentResult FullSize(long id)
        {
            return this.ImageContent(id, photo => photo.PhotoFull);
        }

        public FileContentResult Thumbnail(long id)
        {
            return this.ImageContent(id, photo => photo.PhotoThumbnail);
        }

        private FileContentResult ImageContent(long photoId, Func<Photo, byte[]> imageAccessor)
        {
            var photo = this.photos.SingleOrDefault(x => x.Id == photoId);
            if (photo == null)
            {
                throw new HttpException(404, "Not found requested photo");
            }

            return new FileContentResult(imageAccessor.Invoke(photo), photo.ContentType);
        }
    }
}
