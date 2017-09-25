using System.Collections.Generic;

using PDR.Domain.Model.Base;

using SmartArch.Data;

namespace PDR.Domain.Contracts.Repositories
{
    public class CompanyMemoryRepository<TEntity> : MemoryRepository<TEntity>, ICompanyRepository<TEntity>
        where TEntity : Entity, ICompanyEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository&lt;TEntity&gt;"/> class.
        /// </summary>
        public CompanyMemoryRepository()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public CompanyMemoryRepository(IEnumerable<TEntity> entities) : base(entities)
        {
        }

    }
}