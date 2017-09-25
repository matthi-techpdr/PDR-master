using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NHibernate.Linq;

using SmartArch.Data.Fetching;

namespace SmartArch.Data.NH.Fetching
{
    /// <summary>
    /// Represents NHibernate fetch provider.
    /// </summary>
    public class NhFetchingProvider : IFetchingProvider
    {
        /// <summary>
        /// Fetches the specified query.
        /// </summary>
        /// <typeparam name="TOriginating">The type of the originating.</typeparam>
        /// <typeparam name="TRelated">The type of the related.</typeparam>
        /// <param name="query">The query of entities.</param>
        /// <param name="relatedObjectSelector">The related object selector.</param>
        /// <returns>The fetching request.</returns>
        public IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
        {
            var fetch = EagerFetchingExtensionMethods.Fetch(query, relatedObjectSelector);
            return new FetchRequest<TOriginating, TRelated>(fetch);
        }

        /// <summary>
        /// Fetches the many.
        /// </summary>
        /// <typeparam name="TOriginating">The type of the originating.</typeparam>
        /// <typeparam name="TRelated">The type of the related.</typeparam>
        /// <param name="query">The query of entities.</param>
        /// <param name="relatedObjectSelector">The related object selector.</param>
        /// <returns>The fetching request.</returns>
        public IFetchRequest<TOriginating, TRelated> FetchMany<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            var fecth = EagerFetchingExtensionMethods.FetchMany(query, relatedObjectSelector);
            return new FetchRequest<TOriginating, TRelated>(fecth);
        }

        /// <summary>
        /// Thens the fetch.
        /// </summary>
        /// <typeparam name="TQueried">The type of the queried.</typeparam>
        /// <typeparam name="TFetch">The type of the fetch.</typeparam>
        /// <typeparam name="TRelated">The type of the related.</typeparam>
        /// <param name="query">The query of entities.</param>
        /// <param name="relatedObjectSelector">The related object selector.</param>
        /// <returns>The fetching request.</returns>
        public IFetchRequest<TQueried, TRelated> ThenFetch<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector)
        {
            var impl = query as FetchRequest<TQueried, TFetch>;
            var fetch = EagerFetchingExtensionMethods.ThenFetch(impl.NhibernateFetchRequest, relatedObjectSelector);
            return new FetchRequest<TQueried, TRelated>(fetch);
        }

        /// <summary>
        /// Thens the fetch many.
        /// </summary>
        /// <typeparam name="TQueried">The type of the queried.</typeparam>
        /// <typeparam name="TFetch">The type of the fetch.</typeparam>
        /// <typeparam name="TRelated">The type of the related.</typeparam>
        /// <param name="query">The query of entities.</param>
        /// <param name="relatedObjectSelector">The related object selector.</param>
        /// <returns>The fetching request.</returns>
        public IFetchRequest<TQueried, TRelated> ThenFetchMany<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            var impl = query as FetchRequest<TQueried, TFetch>;
            var fetch = EagerFetchingExtensionMethods.ThenFetchMany(impl.NhibernateFetchRequest, relatedObjectSelector);
            return new FetchRequest<TQueried, TRelated>(fetch);
        }
    }
}