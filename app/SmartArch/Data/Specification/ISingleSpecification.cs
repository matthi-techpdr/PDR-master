using System.Linq;

namespace SmartArch.Data.Specification
{
    public interface ISpecification<T> 
    {
        IQueryable<T> SatisfiedBy(IQueryable<T> candidates);
    }    
}