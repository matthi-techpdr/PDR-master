using NHibernate.Mapping.ByCode;

using PDR.Domain.Model;

namespace PDR.NHibernateProvider.Overrides
{
    public class CompanyMapping : IOverride
    {
        public void Override(ModelMapper mapper)
        {
            mapper.Class<Company>(cm => cm.ManyToOne(x => x.Logo, m => { m.Cascade(Cascade.All); m.Unique(true); }));
            mapper.Class<Company>(cm => cm.ManyToOne(x => x.DefaultMatrix, m => { m.Cascade(Cascade.All); m.Unique(true); }));
        }
    }
}