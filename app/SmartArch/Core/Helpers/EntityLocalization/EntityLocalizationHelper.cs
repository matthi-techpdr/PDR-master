using System;
using System.Linq.Expressions;

using Microsoft.Practices.ServiceLocation;

namespace SmartArch.Core.Helpers.EntityLocalization
{
    /// <summary>
    /// Represents helper for localizing entity attributes.
    /// </summary>
    public static class EntityLocalizationHelper
    {
        /// <summary>
        /// The helper's engine.
        /// </summary>
        private static IEntityLocalizationEngine engine;

        /// <summary>
        /// Gets a value indicating whether this instance has default engine.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has default engine; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDefaultEngine { get; private set; }

        /// <summary>
        /// Initializes static members of the <see cref="EntityLocalizationHelper"/> class.
        /// </summary>
        static EntityLocalizationHelper()
        {
            SetDefaultEngine();
        }

        /// <summary>
        /// Gets or sets the engine.
        /// </summary>
        /// <value>The helper's engine.</value>
        public static IEntityLocalizationEngine Engine
        {
            get
            {
                return engine ?? (engine = ServiceLocator.Current.GetInstance<IEntityLocalizationEngine>());
            }

            set
            {
                engine = value;
                IsDefaultEngine = false;
            }
        }

        /// <summary>
        /// Sets the default engine.
        /// </summary>
        public static void SetDefaultEngine()
        {
            engine = null;
            IsDefaultEngine = true;
        }

        /// <summary>
        /// Gets the localized name of the attribute.
        /// </summary>
        /// <param name="type">The same type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The localized name of the attribute.</returns>
        public static string GetAttributeName(Type type, string propertyName)
        {
            var name = Engine.GetAttributeName(type, propertyName);

            return name;
        }

        /// <summary>
        /// Gets the localized name of the attribute.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>
        /// The localized name of the attribute.
        /// </returns>
        public static string GetAttributeName<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            var name = Engine.GetAttributeName(property);

            return name;
        }

        /// <summary>
        /// Gets the localized name of the class.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The localized name.</returns>
        public static string GetClassName(Type entityType)
        {
            var name = Engine.GetClassName(entityType);            

            return name;
        }
    }
}