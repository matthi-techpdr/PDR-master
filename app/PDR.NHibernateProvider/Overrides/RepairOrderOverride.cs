using NHibernate.Mapping.ByCode;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;

namespace PDR.NHibernateProvider.Overrides
{
    public class RepairOrderOverride : IOverride
    {
        public void Override(ModelMapper mapper)
        {
            mapper.Class<RepairOrder>(
                x => x.Set(e => e.TeamEmployeePercents, 
                k =>
                {
                    k.Key(c => c.Column("RepairOrderFk"));
                    k.Cascade(Cascade.All | Cascade.DeleteOrphans);
                },
                m => m.OneToMany(a => a.Class(typeof(TeamEmployeePercent)))));

            mapper.Class<TeamEmployee>(
                x => x.Set(e => e.TeamEmployeePercents,
                    k =>
                    {
                        k.Key(c => c.Column("TeamEmployeeFk"));
                        k.Cascade(Cascade.All | Cascade.DeleteOrphans);
                    },
                    m => m.OneToMany(a => a.Class(typeof(TeamEmployeePercent)))));
        }
    }
}