using System.Collections.Generic;

using PDR.Domain.Services.TempImageStorage;

namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    public interface IPhotoContainer
    {
        List<ImageInfo> UploadPhotos { get; set; }

        List<ImageInfo> StoredPhotos { get; set; } 
    }
}