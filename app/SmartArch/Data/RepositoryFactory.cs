using Microsoft.Practices.ServiceLocation;

using SmartArch.Core.Domain.Base;

namespace SmartArch.Data
{
    /// <summary>
    /// Represents repositories factory which used <c>IoC</c> container.
    /// </summary>
    public class RepositoryFactory : IRepositoryFactory
    {
        /// <summary>
        /// Creates the entities repository by specified entity type.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <returns>The entities repository.</returns>
        public IRepository<T> Create<T>() where T : BaseEntity
        {
            var repository = ServiceLocator.Current.GetInstance<IRepository<T>>();

            return repository;
        }
    }
}