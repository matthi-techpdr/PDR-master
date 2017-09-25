using NHibernate.Mapping.ByCode;

using PDR.Domain.Model;
using PDR.Domain.Model.Photos;
using PDR.Domain.Model.Users;

namespace PDR.NHibernateProvider.Overrides
{
    public class EstimateMapping : IOverride
    {
        public void Override(ModelMapper mapper)
        {
            mapper.Class<Estimate>(cm => cm.ManyToOne(x => x.Car, m => { m.Cascade(Cascade.All); m.Unique(true); }));
            mapper.Class<Estimate>(cm => cm.ManyToOne(x => x.Customer, m => { m.Cascade(Cascade.All); m.Unique(true); }));
            mapper.Class<Estimate>(cm => cm.ManyToOne(x => x.Insurance, m => { m.Cascade(Cascade.All); m.Unique(true); }));
            mapper.Class<Estimate>(cm => cm.ManyToOne(x => x.CustomerSignature, m => { m.Cascade(Cascade.All); m.Unique(true); }));

            mapper.Class<Estimate>(cm => cm.ManyToOne(x => x.CarImage, m => { m.Cascade(Cascade.All); m.Unique(true); m.Column("CarImageFk"); }));
            mapper.Class<Estimate>(x => x.Set(e => e.Photos, k => k.Key(c => c.Column("EstimateFk")), m => m.OneToMany(a => a.Class(typeof(CarPhoto)))));

            mapper.Class<Estimate>(r => r.ManyToOne(x => x.Employee, m => { m.Cascade(Cascade.All); m.Column("EmployeeFk"); }));
            mapper.Class<Employee>(x => x.Set(e => e.Estimates, k => k.Key(c => c.Column("EmployeeFk")), m => m.OneToMany(a => a.Class(typeof(Estimate)))));

            mapper.Class<Estimate>(x => x.Set(e => e.CarInspections,
                k =>
            {
                k.Key(c => c.Column("EstimateFk"));
                k.Cascade(Cascade.All);
            },
            m => m.OneToMany(a => a.Class(typeof(CarInspection)))));

            mapper.Class<Estimate>(mapping => mapping.Bag(entity => entity.PreviousEmployees,
                bag =>
                {
                    bag.Key(key => key.Column("EstimateFk"));
                    bag.Table("PreEmployees_PreEstimates");
                    bag.Cascade(Cascade.None);
                },
                collectionRelation => collectionRelation.ManyToMany(m => m.Column("EmployeeFk"))));

            mapper.Class<Employee>(mapping => mapping.Bag(entity => entity.PreviousEstimates,
                bag =>
                {
                    bag.Key(key => key.Column("EmployeeFk"));
                    bag.Table("PreEmployees_PreEstimates");
                    bag.Cascade(Cascade.All);
                },
                collectionRelation => collectionRelation.ManyToMany(m => m.Column("EstimateFk"))));
        }
    }
}
