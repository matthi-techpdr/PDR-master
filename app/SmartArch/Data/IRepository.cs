using System.Linq;

namespace SmartArch.Data
{
    /// <summary>
    /// Represents interface for marked entities repository.
    /// </summary>
    public interface IRepository
    {
    }

    /// <summary>
    /// Represents entities repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepository<TEntity> : IQueryable<TEntity>, IRepository
    {
        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity instance.</param>
        void Save(TEntity entity);

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity instance.</param>
        void Remove(TEntity entity);

        /*
        /// <summary>
        /// Gets the entities finder.
        /// </summary>
        /// <value>The entities finder.</value>
        IFinder<TEntity> Find { get; }
        */
    }
}