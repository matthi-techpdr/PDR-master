using System;
﻿using System.Collections.Generic;
using System.Linq;
﻿using System.Reflection;

﻿using Fasterflect;

using PDR.Domain.Model;
using PDR.Domain.Model.Base;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;

using PDR.Domain.Model.Photos;
using PDR.NHibernateProvider.Overrides;

using NetInflector = Inflector.Net.Inflector;

namespace PDR.NHibernateProvider
{
    /// <summary>
    /// Applies global common conventions to the mapped entities. 
    /// For clarity configurations set here can be overriden in 
    /// an entity's specific mapping file.  For example; The Id 
    /// convention here is set to Id but if the Id column 
    /// was mapped in the entity's mapping file then the entity's 
    /// mapping file configuration will take precedence.
    /// </summary>
    internal static class Conventions
    {
    	private const string FOREIGN_KEY_COLUMN_POSTFIX = "Fk";
        
        private static IEnumerable<Type> hierarchyEntities;

        public static void WithConventions(this ConventionModelMapper mapper, Configuration configuration)
        {
            Type baseEntityType = typeof(Entity);

            mapper.IsEntity(IsEntity);
            mapper.IsRootEntity((type, declared) => baseEntityType == type.BaseType || typeof(CompanyEntity) == type.BaseType);
            mapper.IsTablePerClassHierarchy(IsHierarchyRootEntity);
            mapper.IsManyToMany(IsManyToMany);
            mapper.IsOneToOne(IsOneToOne);

            mapper.BeforeMapClass += (modelInspector, type, classCustomizer) =>
            {
                classCustomizer.Id(c => c.Column("Id"));
                classCustomizer.Id(c => c.Generator(Generators.HighLow));
                
                classCustomizer.Table(NetInflector.Pluralize(type.Name.ToString()));
            };

            //mapper.BeforeMapMapKey += (modelInspector, propertyPath, map) =>
            //{
            //    map.Column(propertyPath.LocalMember.Name + FOREIGN_KEY_COLUMN_POSTFIX);
            //};

            mapper.BeforeMapSet += BeforeMappingCollectionConvention;
            mapper.BeforeMapBag += BeforeMappingCollectionConvention;
            mapper.BeforeMapList += BeforeMappingCollectionConvention;
            mapper.BeforeMapIdBag += BeforeMappingCollectionConvention;
            mapper.BeforeMapMap += BeforeMappingCollectionConvention;

            mapper.BeforeMapManyToOne += (modelInspector, propertyPath, map) =>
            {
                map.Column(propertyPath.LocalMember.Name + FOREIGN_KEY_COLUMN_POSTFIX);
            };

			mapper.BeforeMapSubclass += (modelInspector, type, map) =>
			{
				map.DiscriminatorValue(type.Name);
			};

            AddConventionOverrides(mapper);

            //HbmMapping mapping = mapper.CompileMappingFor(typeof(Entity).Assembly.GetExportedTypes().Where(x => IsEntity(x)));
            var types = typeof(Entity).Assembly.GetExportedTypes().Where(t => IsEntity(t)).PartialOrderBy(x => x, new EntityTypeComparer());
            HbmMapping mapping = mapper.CompileMappingFor(types);
            configuration.AddDeserializedMapping(mapping, "PDRMappings");
            mapping.WriteAllXmlMapping("pdr.nh.mappings.xml");
        }

        /// <summary>
        /// Determine if type implements Entity
        /// </summary>
        public static bool IsEntity(Type type, bool declared = false)
        {
            return typeof(Entity).IsAssignableFrom(type) && typeof(Entity) != type && !type.IsInterface && !type.IsGenericType;
        }

        /// <summary>
        /// Looks through this assembly for any IOverride classes.  If found, it creates an instance
        /// of each and invokes the Override(mapper) method, accordingly.
        /// </summary>
        private static void AddConventionOverrides(ConventionModelMapper mapper)
        {
            var overrideType = typeof(IOverride);
            List<Type> types = typeof(IOverride).Assembly.GetTypes()
                .Where(t => overrideType.IsAssignableFrom(t) && t != typeof(IOverride))
                .ToList();

            types.ForEach(t =>
            {
                var conventionOverride = (IOverride)Activator.CreateInstance(t);
                conventionOverride.Override(mapper);
            });
        }

		private static bool IsHierarchyRootEntity(Type type, bool declared = false)
		{
			if (hierarchyEntities == null)
			{
			    var alltypes = Assembly.GetAssembly(typeof(Entity)).GetTypes().ToList();
                var rootEntities = alltypes.Where(t => (t.BaseType == typeof(Entity) || t.BaseType == typeof(CompanyEntity)) &&
                                   alltypes.Any(x => x.BaseType == t)).Except(new [] { typeof(CompanyEntity) }).ToList();
				var nestedEntities = alltypes.Where(t => rootEntities.Any(rt => rt.IsAssignableFrom(t)));
				hierarchyEntities = rootEntities.Union(nestedEntities);
			}

			return hierarchyEntities.Contains(type);
		}

        public static bool IsManyToMany(MemberInfo memberInfo, bool declared = false)
        {
            var isManyToMany = false;
            var memberType = memberInfo.Type();
            if (memberType.IsGenericType)
            {
                var entityType = memberInfo.DeclaringType;
                var relatedEntityType = memberType.GetGenericArguments()[0];
                var relatedEntityProperties = relatedEntityType.Properties();
                var targetPropertiesCount = relatedEntityProperties
                    .Select(x => x.Type())
                    .Count(x => x.IsGenericType && x.GetGenericArguments()[0] == entityType);

                isManyToMany = targetPropertiesCount > 0;
            }

            return isManyToMany;
        }

        private static void BeforeMappingCollectionConvention(IModelInspector inspector, PropertyPath member, ICollectionPropertiesMapper customizer)
        {
            if (!inspector.IsManyToMany(member.LocalMember))
            {
                customizer.Key(k => k.Column(DetermineKeyColumnName(inspector, member)));
                customizer.Cascade(Cascade.Persist);
                customizer.Inverse(true);
            }
        }

        private static string DetermineKeyColumnName(IModelInspector inspector, PropertyPath member)
        {
            var otherSideProperty = member.OneToManyOtherSideProperty();
            if (inspector.IsOneToMany(member.LocalMember) && otherSideProperty != null)
            {
                return otherSideProperty.Name + FOREIGN_KEY_COLUMN_POSTFIX;
            }

            return member.Owner().Name + FOREIGN_KEY_COLUMN_POSTFIX;
        }

        public static bool IsOneToOne(MemberInfo memberInfo, bool declared = false)
        {
            var isOneToOne = false;
            var memberType = memberInfo.Type();
            if (Conventions.IsEntity(memberType))
            {
                var entityType = memberInfo.DeclaringType;
                var relatedEntityType = memberType;
                
                isOneToOne = GetRelationSecondSide(entityType, relatedEntityType) != null;
            }

            return isOneToOne;
        }

        public static MemberInfo GetRelationSecondSide(Type entityType, Type relationType)
        {
            if (entityType == typeof(Company) || relationType == typeof(Company))
            {
                return null;
            }

            var relatedEntityProperties = relationType.Properties();
            var targetProperty = relatedEntityProperties
                .SingleOrDefault(x => x.Type() == entityType);

            return targetProperty;
        }
        public static IEnumerable<TSource> PartialOrderBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return PartialOrderBy(source, keySelector, null);
        }

        public static IEnumerable<TSource> PartialOrderBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (comparer == null) comparer = (IComparer<TKey>)Comparer<TKey>.Default;

            return PartialOrderByIterator(source, keySelector, comparer);
        }

        private static IEnumerable<TSource> PartialOrderByIterator<TSource, TKey>(
            IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer)
        {
            var values = source.ToArray();
            var keys = values.Select(keySelector).ToArray();
            int count = values.Length;
            var notYieldedIndexes = System.Linq.Enumerable.Range(0, count).ToArray();
            int valuesToGo = count;

            while (valuesToGo > 0)
            {
                //Start with first value not yielded yet
                int minIndex = notYieldedIndexes.First(i => i >= 0);

                //Find minimum value amongst the values not yielded yet
                for (int i = 0; i < count; i++)
                    if (notYieldedIndexes[i] >= 0)
                        if (comparer.Compare(keys[i], keys[minIndex]) < 0)
                        {
                            minIndex = i;
                        }

                //Yield minimum value and mark it as yielded
                yield return values[minIndex];
                notYieldedIndexes[minIndex] = -1;
                valuesToGo--;
            }
        }
    }
    public class EntityTypeComparer : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            if (x == y)
                return 0;
            //if (x.IsAssignableFrom(y) || (!x.IsAssignableTo<Entity>() && y.IsAssignableTo<Entity>()))
            if (x.IsAssignableFrom(y))
                return -1;
            //if (y.IsAssignableFrom(x) || (!y.IsAssignableTo<Entity>() && x.IsAssignableTo<Entity>()))
            if (y.IsAssignableFrom(x))
                return 1;
            return 0;
        }
    }
}
