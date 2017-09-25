using System.Linq;

using NHibernate;
using NHibernate.Linq;

namespace SmartArch.Data.NH
{
    /// <summary>
    /// Represents repository using <c>NHibernate</c> storing model.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class Repository<TEntity> : RepositoryBase<TEntity>
    {
        /// <summary>
        /// The <c>NHibernate</c> session.
        /// </summary>
        private readonly ISession session;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository&lt;TEntity&gt;"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public Repository(ISession session)
        {
            // Check.Require<ArgumentNullException>(session != null, "Can't create Repository<TEntity> instance because session is null");
            
            this.session = session;
        }

        /// <summary>
        /// Gets the repository query.
        /// </summary>
        /// <value>The repository query.</value>
        protected override IQueryable<TEntity> RepositoryQuery
        {
            get
            {
                return this.session.Query<TEntity>();
            }
        }

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void Save(TEntity entity)
        {
            this.session.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void Remove(TEntity entity)
        {
            this.session.Delete(entity);
        }
    }
}