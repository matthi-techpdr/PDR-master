using System.Web;

namespace PDR.Domain.Services.Webstorage
{
    public interface ICurrentWebStorage<T>
    {
        T Get();

        void Set(T obj, HttpResponseBase response);

        HttpCookie GetAuthCookie(T obj, bool forIphone = false, string version = "");
    }
}
