using PDR.Domain.Model.Base;

using SmartArch.Core.Domain.Base;

namespace PDR.Domain.Contracts.Repositories
{
    public interface IRepositoryFactory
    {
        INativeRepository<T> CreateNative<T>() where T : BaseEntity;

        ICompanyRepository<T> CreateForCompany<T>() where T : Entity, ICompanyEntity;
    }
}