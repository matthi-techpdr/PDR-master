using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

using SmartArch.Web.Helpers;

namespace PDR.Domain.Services.TempImageStorage
{
    public class TempImageManager : ITempImageManager
    {
        /// <summary>
        /// The virtual path to image temp folder.
        /// </summary>
        private const string TEMP_PATH = "/content/images/upload/temp/";

        private const string NO_FOUND_IMG = "aaaknfjaslhfgkuahgfdhahjj-NO-FOUND.jpeg";

        private string StoringFolderPath
        {
            get
            {
                return HttpContext.Current.Server.MapPath(TEMP_PATH);
            }
        }

        /// <summary>
        /// Saves the image to temp storage.
        /// </summary>
        /// <param name="image">The image posted by http.</param>
        /// <returns>The uploaded image info in format: id - guid which associate with image, fullSizeUrl - url to fullsize image, thumbnailUrl - url to thumbnail of image.</returns>
        public ImageInfo Save(HttpPostedFileBase image)
        {
            var guid = Guid.NewGuid().ToString();
            var storingFullSizeImageName = guid + '-' + image.FileName;
            var storingFolderPath = this.StoringFolderPath;
            var storingFullSizeImagePath = Path.Combine(storingFolderPath, storingFullSizeImageName);
            image.SaveAs(storingFullSizeImagePath);

            var storingThumbnailImageName = guid + "-Thumbnail-" + image.FileName;
            var storingThumbnailImagePath = Path.Combine(storingFolderPath, storingThumbnailImageName);
            using (Image storedImage = Image.FromFile(storingFullSizeImagePath))
            {
                var thumbnailImage = storedImage.GetThumbnailImage(60, 60, () => false, IntPtr.Zero);
                thumbnailImage.Save(storingThumbnailImagePath);
                thumbnailImage.Dispose();
            }

            return new ImageInfo { Id = guid, ContentType = image.ContentType, FullSizeUrl = TEMP_PATH + storingFullSizeImageName, ThumbnailUrl = TEMP_PATH + storingThumbnailImageName };
        }

        public ImageInfo Get(string guid)
        {
            var storingDir = new DirectoryInfo(this.StoringFolderPath);
            var files = storingDir.GetFiles(guid + "*");
            if (files.Count() == 2)
            {
                string thumbnail = NO_FOUND_IMG;
                string fullsize = NO_FOUND_IMG;
                string contentType = null;
                foreach (var file in files)
                {
                    if (contentType == null)
                    {
                        contentType = MimeType.Get(file.Name);
                    }

                    if (file.Name.Contains("-Thumbnail-"))
                    {
                        thumbnail = file.Name;
                    }
                    else
                    {
                        fullsize = file.Name;
                    }
                }

                return new ImageInfo { Id = guid, ContentType = contentType, ThumbnailUrl = Path.Combine(TEMP_PATH, thumbnail), FullSizeUrl = Path.Combine(TEMP_PATH, fullsize) };
            }

            return null;
        }

        public void Remove(string guid)
        {
            var storingDir = new DirectoryInfo(this.StoringFolderPath);
            var files = storingDir.GetFiles(guid + "*");
            foreach (var fileInfo in files)
            {
                fileInfo.Delete();
            }
        }
    }    
}