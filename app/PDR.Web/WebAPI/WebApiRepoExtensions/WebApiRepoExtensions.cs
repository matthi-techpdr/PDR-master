using System.Net;
using System.Net.Http;
using System.Web.Http;

using SmartArch.Core.Domain.Base;
using SmartArch.Data;

namespace PDR.Web.WebAPI.WebApiRepoExtensions
{
    public static class WebApiRepoExtenssions
    {
        public static T GetIfExist<T>(this IRepository<T> entities, long id) where T : BaseEntity
        {
            var entitiy = entities.Get(id);
            if (entitiy == null)
            {
                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.NotFound);
                message.Content = new StringContent(string.Format("{0} with the same id is not found.", typeof(T).Name));
                throw new HttpResponseException(message);
            }

            return entitiy;
        }
    }
}