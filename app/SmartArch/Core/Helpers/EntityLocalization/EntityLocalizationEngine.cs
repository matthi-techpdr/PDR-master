using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;

using StringNamedFormat;

namespace SmartArch.Core.Helpers.EntityLocalization
{
    /// <summary>
    /// Represents engine of localization entity attributes.
    /// </summary>
    public class EntityLocalizationEngine : IEntityLocalizationEngine
    {
        private readonly Type entityNameResType;

        public EntityLocalizationEngine(Type entityNameResType)
        {
            this.entityNameResType = entityNameResType;
        }

        /// <summary>
        /// Gets the name of the entity attribute.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The entity attribute name.</returns>
        public string GetAttributeName(Type entityType, string propertyName)
        {
            var resAssemblyTypes = Assembly.GetAssembly(this.entityNameResType).GetTypes();
            var resType = GetResType(entityType, resAssemblyTypes);
            string attributeName = null;
            if (resType != null)
            {
                var resourceManager = new ResourceManager(resType);
                attributeName = resourceManager.GetString(propertyName);
                if (attributeName == null)
                {
                    throw new NotSupportedException("Not support localized name of attribute {attr} into class {type}. See resurce file named '{file}'".NamedFormat(new { attr = propertyName, type = entityType, file = resType.Name }));
                }
            }

            return attributeName;
        }

        /// <summary>
        /// Gets the name of the entity attribute.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>
        /// The entity attribute name.
        /// </returns>
        public string GetAttributeName<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> property)
        {
            return this.GetAttributeName(typeof(TEntity), Reflector.Property(property).Name);
        }

        /// <summary>
        /// Gets the name of the entity attribute.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The entity attribute name.</returns>
        public string GetClassName(Type entityType)
        {
            var resType = this.entityNameResType;
            var resourceManager = new ResourceManager(resType);
            var className = resourceManager.GetString(entityType.Name);
            if (className == null)
            {
                throw new NotSupportedException("Not support localized name of class {type}. See resurce file named 'EntityNamesRes'".NamedFormat(new { type = entityType }));
            }

            return className;
        }

        /// <summary>
        /// Gets the name of the resource class.
        /// </summary>
        /// <param name="type">The type of localized entity.</param>
        /// <returns>The name of the resource class.</returns>
        private static string GetResClassName(Type type)
        {
            var resName = type.Name + "Res";

            return resName;
        }

        /// <summary>
        /// Gets the type of the resource class with localized attributes.
        /// </summary>
        /// <param name="type">The same type.</param>
        /// <param name="candidates">The resource candidates.</param>
        /// <returns>The type of the resource class with localized attributes.</returns>
        private static Type GetResType(Type type, IEnumerable<Type> candidates)
        {
            var listOfCandidates = candidates.ToList();
            Type currentType = listOfCandidates.FirstOrDefault(t => t.Name == GetResClassName(type));
            if (currentType == null && type.BaseType != typeof(object))
            {
                currentType = GetResType(type.BaseType, listOfCandidates);
            }

            return currentType;
        }
    }
}