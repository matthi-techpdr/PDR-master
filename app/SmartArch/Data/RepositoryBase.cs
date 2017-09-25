using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SmartArch.Data
{
    /// <summary>
    /// Represents base functionality of entities repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
    {
        /// <summary>
        /// Gets the repository query.
        /// </summary>
        /// <value>The repository query.</value>
        protected abstract IQueryable<TEntity> RepositoryQuery { get; }

        #region Implementation of IRepository<TEntity>

        /// <summary>
        /// Saves the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public abstract void Save(TEntity entity);

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public abstract void Remove(TEntity entity);

        /*
        /// <summary>
        /// Gets the entities finder.
        /// </summary>
        /// <value>The entities finder.</value>
        public virtual IFinder<TEntity> Find
        {
            get
            {
                return new Finder<TEntity>(this);
            }
        }
        */

        #endregion
        
        #region Implementation of IEnumerable<TEntity>

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator GetEnumerator()
        {
            return this.RepositoryQuery.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return this.RepositoryQuery.GetEnumerator();
        }

        #endregion

        #region Implementation of IQueryable<TEntity>

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.</returns>
        public Expression Expression
        {
            get
            {
                return this.RepositoryQuery.Expression;
            }
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.</returns>
        public Type ElementType
        {
            get
            {
                return this.RepositoryQuery.ElementType;
            }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.</returns>
        public IQueryProvider Provider
        {
            get
            {
                return this.RepositoryQuery.Provider;
            }
        }

        #endregion
    }
}