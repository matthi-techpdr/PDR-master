using NHibernate;

using PDR.Domain.Contracts.Repositories;

namespace PDR.NHibernateProvider.Repositories
{
    public class NativeRepository<T> : SmartArch.Data.NH.Repository<T>, INativeRepository<T>
    {
        public NativeRepository(ISession session) : base(session)
        {
        }
    }
}