using System.Collections.Generic;
using System.Linq;

namespace SmartArch.Data
{
    /// <summary>
    /// Represents repository in memory.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class MemoryRepository<TEntity> : RepositoryBase<TEntity>
    {
        /// <summary>
        /// The repository source.
        /// </summary>
        private readonly ICollection<TEntity> source;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository&lt;TEntity&gt;"/> class.
        /// </summary>
        public MemoryRepository() : this(new List<TEntity>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public MemoryRepository(IEnumerable<TEntity> entities)
        {
            this.source = new List<TEntity>(entities);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryRepository&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public MemoryRepository(params TEntity[] entities) : this(entities.ToList())
        {
        }

        /// <summary>
        /// Gets the repository query.
        /// </summary>
        /// <value>The repository query.</value>
        protected override IQueryable<TEntity> RepositoryQuery
        {
            get
            {
                return this.source.AsQueryable();
            }
        }

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void Save(TEntity entity)
        {
            if (!this.source.Contains(entity))
            {
                this.source.Add(entity);
            }
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void Remove(TEntity entity)
        {
            this.source.Remove(entity);
        }
    }
}