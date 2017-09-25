using System.Web;

namespace PDR.Domain.Services.TempImageStorage
{
    public interface ITempImageManager
    {
        ImageInfo Save(HttpPostedFileBase image);

        ImageInfo Get(string guid);

        void Remove(string guid);
    }
}