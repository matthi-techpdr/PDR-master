using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Base;

using SmartArch.Core.Domain.Base;

using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.NHibernateProvider.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public INativeRepository<T> CreateNative<T>() where T : BaseEntity
        {
            var repository = ServiceLocator.Current.GetInstance<INativeRepository<T>>();

            return repository;
        }

        public ICompanyRepository<T> CreateForCompany<T>() where T : Entity, ICompanyEntity
        {
            var repository = ServiceLocator.Current.GetInstance<ICompanyRepository<T>>();

            return repository;
        }
    }
}