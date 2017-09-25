using System;
using System.Linq;
using SmartArch.Core.Domain.Base;

namespace SmartArch.Data.Specification
{
    public class Finder<TEntity> : IFinder<TEntity> where TEntity : BaseEntity
    {
        private readonly IQueryable<TEntity> candidates;

        public Finder(IQueryable<TEntity> candidates)
        {
            if (candidates == null)
            {
                throw new ArgumentNullException("Can't create instance because constructor argument candidates is null");
            }

            this.candidates = candidates;
        }

        public IQueryable<TEntity> All(ISpecification<TEntity> specification)
        {

            if (specification == null)
            {
                throw new ArgumentNullException("Unable gets all entities by specification which is null");
            }

            IQueryable<TEntity> result = specification.SatisfiedBy(this.candidates);
            return result;
        }

        public TEntity One(ISingleSpecification<TEntity> specification)
        {
            if (specification == null)
            {
                throw new ArgumentNullException("Unable gets all entities by specification which is null");
            }

            TEntity result = specification.SatisfiedBy(this.candidates);
            return result;
        }
    }
}
