using System.Linq;

namespace SmartArch.Data.Specification
{
    public interface ISingleSpecification<T> 
    {
        T SatisfiedBy(IQueryable<T> candidates);
    }    
}