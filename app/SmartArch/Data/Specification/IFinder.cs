using System.Linq;

namespace SmartArch.Data.Specification
{
    public interface IFinder<TEntity> 
    {
        IQueryable<TEntity> All(ISpecification<TEntity> specification);

        TEntity One(ISingleSpecification<TEntity> specification);
    }
}
