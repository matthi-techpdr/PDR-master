using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NHibernate.Linq;

using SmartArch.Data.Fetching;

namespace SmartArch.Data.NH.Fetching
{
    /// <summary>
    /// Represents fetch request.
    /// </summary>
    /// <typeparam name="TQueried">The type of the queried.</typeparam>
    /// <typeparam name="TFetch">The type of the fetch.</typeparam>
    public class FetchRequest<TQueried, TFetch> : IFetchRequest<TQueried, TFetch>
    {
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The request enumerator.</returns>
        public IEnumerator<TQueried> GetEnumerator()
        {
            return this.NhibernateFetchRequest.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.NhibernateFetchRequest.GetEnumerator();
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <returns>A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.</returns>
        public Type ElementType
        {
            get
            {
                return this.NhibernateFetchRequest.ElementType;
            }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <returns>The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.</returns>
        public Expression Expression
        {
            get
            {
                return this.NhibernateFetchRequest.Expression;
            }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <returns>The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.</returns>
        public IQueryProvider Provider
        {
            get
            {
                return this.NhibernateFetchRequest.Provider;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FetchRequest&lt;TQueried, TFetch&gt;"/> class.
        /// </summary>
        /// <param name="fetchRequest">The nh fetch request.</param>
        public FetchRequest(INhFetchRequest<TQueried, TFetch> fetchRequest)
        {
            this.NhibernateFetchRequest = fetchRequest;
        }

        /// <summary>
        /// Gets the nh fetch request.
        /// </summary>
        public INhFetchRequest<TQueried, TFetch> NhibernateFetchRequest { get; private set; }
    }
}