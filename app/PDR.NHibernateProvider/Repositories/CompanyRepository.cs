using System.Linq;
using NHibernate;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

namespace PDR.NHibernateProvider.Repositories
{
    public class CompanyRepository<T> : NativeRepository<T>, ICompanyRepository<T>
        where T : Entity, ICompanyEntity
    {
        private readonly ICurrentWebStorage<Employee> userWebStorage;

        public CompanyRepository(ISession session, ICurrentWebStorage<Employee> userWebStorage)
            : base(session)
        {
            this.userWebStorage = userWebStorage;
        }

        protected override IQueryable<T> RepositoryQuery
        {
            get
            {
                var company = this.userWebStorage.Get().Company;
                return base.RepositoryQuery.Where(e => e.Company == company);
            }
        }
    }
}
