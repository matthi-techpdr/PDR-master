using System;
using System.Web;
using System.Web.Security;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Base;
using SmartArch.Data;

namespace PDR.Domain.Services.Webstorage
{
    public class CurrentWebStorage<T> : ICurrentWebStorage<T>
        where T : Entity
    {
        private readonly INativeRepository<T> repository;

        private T entity;

        public CurrentWebStorage(INativeRepository<T> repository)
        {
            this.repository = repository;
        }

        public T Get()
        {
            var identity = HttpContext.Current.User.Identity as FormsIdentity;
            if (this.entity == null && identity != null && identity.Ticket != null)
            {
                FormsAuthenticationTicket ticket = identity.Ticket;
                if (!FormsAuthentication.CookiesSupported)
                {
                    ticket = FormsAuthentication.Decrypt(ticket.Name);
                }

                if (ticket != null)
                {
                    var id = ParseUserData(ticket.UserData);
                    this.entity = this.repository.Get(id);
                }
            }
            return this.entity;
        }

        public void Set(T obj, HttpResponseBase response)
        {
            var authCookie = this.GetAuthCookie(obj);
            response.Cookies.Add(authCookie);
            this.entity = null;
        }

        public HttpCookie GetAuthCookie(T obj, bool forIphone = false, string version = "")
        {
            var entityId = string.Format("{0}Id={1}", typeof(T).Name, obj.Id);
            var dynam = (dynamic)obj;
            var duration = forIphone ? 360 : 120;
            var ticket = new FormsAuthenticationTicket(
                        1,
                        dynam.Login,
                        DateTime.Now,
                        DateTime.Now.AddMinutes(duration),
                        false,
                        entityId);

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket)) { Expires = ticket.Expiration };
            //if(forIphone)
            //{
            //    HttpContext.Current.Response.Cookies.Add(new HttpCookie("VersionApp", version) { Expires = ticket.Expiration });
            //}
            return authCookie;
        }

        private static long ParseUserData(string userData)
        {
            return long.Parse(userData.Split('=')[1]);
        }
    }
}