using System;
using System.Linq;
using System.Linq.Expressions;

namespace SmartArch.Data.Specification
{
    public class QuerySpecification<T> : IQuerySpecification<T>
    {
        private readonly Expression<Func<T, bool>> matchingCriteria;

        public QuerySpecification(Expression<Func<T, bool>> matchingCriteria)
        {
            this.matchingCriteria = matchingCriteria;
        }

        public IQueryable<T> SatisfiedBy(IQueryable<T> candidates)
        {
            return candidates.Where(this.matchingCriteria);
        }
    }
}