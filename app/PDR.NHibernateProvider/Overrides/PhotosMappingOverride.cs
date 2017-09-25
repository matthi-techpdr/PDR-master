using NHibernate.Mapping.ByCode;

using PDR.Domain.Model;

namespace PDR.NHibernateProvider.Overrides
{
    using PDR.Domain.Model.Photos;

    /// <summary>
	/// Photos mapping override
	/// </summary>
	public class PhotosMappingOverride : IOverride
	{
		public void Override(ModelMapper mapper)
		{
            mapper.Class<CarPhoto>(x => x.Property(m => m.PhotoFull, l => l.Length(int.MaxValue)));
            mapper.Class<CarPhoto>(x => x.Property(m => m.PhotoThumbnail, l => l.Length(int.MaxValue)));
            mapper.Class<CarImage>(x => x.Property(m => m.PhotoThumbnail, l => l.Length(int.MaxValue)));
            mapper.Class<CarImage>(x => x.Property(m => m.PhotoFull, l => l.Length(int.MaxValue)));
            mapper.Class<RepairOrder>(cm => cm.ManyToOne(x => x.RoCustomerSignature, m => { m.Cascade(Cascade.All); m.Unique(true); }));
		}
	}
}
