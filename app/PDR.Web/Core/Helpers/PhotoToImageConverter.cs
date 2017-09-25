using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using PDR.Domain.Model.Photos;
using PDR.Domain.Services.TempImageStorage;

namespace PDR.Web.Core.Helpers
{
    public static class PhotoToImageConverter
    {
         public static ImageInfo ToImage<TPhoto>(this TPhoto photo) where TPhoto : Photo
         {
             var fullsizeUrl = RouteTable.Routes.GetVirtualPathForArea(HttpContext.Current.Request.RequestContext, new RouteValueDictionary(new { area = "Default", controller = "Photos", action = "FullSize", id = photo.Id })).VirtualPath; // this.Url.RouteUrl(new { area = "Default", controller = "Photos", action = "FullSize", id = photo.Id });
             var thumbnailUrl = RouteTable.Routes.GetVirtualPathForArea(HttpContext.Current.Request.RequestContext, new RouteValueDictionary(new { area = "Default", controller = "Photos", action = "Thumbnail", id = photo.Id })).VirtualPath; // this.Url.RouteUrl(new { area = "Default", controller = "Photos", action = "Thumbnail", id = photo.Id });
             var image = new ImageInfo { Id = photo.Id.ToString(), ContentType = photo.ContentType, FullSizeUrl = fullsizeUrl, ThumbnailUrl = thumbnailUrl };

             return image;
         }

         public static List<ImageInfo> ToImages<TPhoto>(this IEnumerable<TPhoto> photos) where TPhoto : Photo
         {
             var images = photos.Select(photo => photo.ToImage()).ToList();

             return images;
         }
    }
}