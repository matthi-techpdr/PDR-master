using System.Linq;
using System.Web;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;

namespace PDR.Domain.Services.VersionStorage
{
    public class VersionStorage<T> : IVersionStorage<T> where T : VersioniPhoneApp
    {
        private readonly INativeRepository<T> repository;

        public VersionStorage(INativeRepository<T> repository)
        {
            this.repository = repository;
        }

        public bool IsWorking()
        {
            var version = HttpContext.Current.Request.Cookies["VersionApp"];
            var vers = this.repository.SingleOrDefault(x => x.Version == version.Value);
            return vers != null && vers.IsWorkingBild;
        }
    }
}