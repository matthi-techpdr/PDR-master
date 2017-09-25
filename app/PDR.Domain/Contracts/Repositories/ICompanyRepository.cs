using PDR.Domain.Model.Base;

namespace PDR.Domain.Contracts.Repositories
{
    public interface ICompanyRepository<T> : INativeRepository<T>
        where T : Entity, ICompanyEntity
    {
    }
}
