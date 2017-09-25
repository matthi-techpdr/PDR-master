using System;
using System.Linq.Expressions;

namespace SmartArch.Core.Helpers.EntityLocalization
{
    /// <summary>
    /// Represents interface of entity localization engine.
    /// </summary>
    public interface IEntityLocalizationEngine
    {
        /// <summary>
        /// Gets the localized name of the entity attribute.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The entity attribute name.</returns>
        string GetAttributeName(Type entityType, string propertyName);

        /// <summary>
        /// Gets the localized name of the class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The localized name.</returns>
        string GetClassName(Type entityType);

        /// <summary>
        /// Gets the name of the entity attribute.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>
        /// The entity attribute name.
        /// </returns>
        string GetAttributeName<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property);
    }
}