using SmartArch.Core.Domain.Base;

namespace SmartArch.Data
{
    /// <summary>
    /// Represents factory for creating entities repositories.
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Creates the entities repository by specified entity type.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <returns>The entities repository.</returns>
        IRepository<T> Create<T>() where T : BaseEntity;
    }
}