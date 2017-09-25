using NHibernate.Mapping.ByCode;

namespace PDR.NHibernateProvider.Overrides
{
    internal interface IOverride
    {
        void Override(ModelMapper mapper);
    }
}
