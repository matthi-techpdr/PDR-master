using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Fasterflect;
using NHibernate.Mapping.ByCode;
using PDR.Domain.Model.Base;

namespace PDR.NHibernateProvider.Overrides
{
	/// <summary>
	/// customers override mapping
	/// </summary>
	public class ManyToManyMappingOverride : IOverride
	{
        private readonly List<KeyValuePair<Type, Type>> manyToManyRelations = new List<KeyValuePair<Type, Type>>();

        public void Override(ModelMapper mapper)
		{
		    var types = typeof(Entity).Assembly.GetExportedTypes()
                .Where(x => Conventions.IsEntity(x))
                .OrderBy(x => x.Name);
            foreach (var type in types)
            {
                var manyToManyProperties = type.Properties().Where(x => Conventions.IsManyToMany(x));
                foreach (var property in manyToManyProperties)
                {
                    this.MapManyToMany(mapper, property);
                }
            }
		}

        private static string GetManyToManyTableName<TLeft, TRight>()
        {
            var tableName = string.Format("{0}_{1}", Inflector.Net.Inflector.Pluralize(typeof(TLeft).Name), Inflector.Net.Inflector.Pluralize(typeof(TRight).Name));

            return tableName;
        }

        private void MapManyToMany(ModelMapper mapper, MemberInfo member)
        {
            var type = this.GetType();
            var method = type.Methods(Flags.InstancePrivate, "MapManyToMany")
                             .Single(m => m.IsGenericMethod)
                             .MakeGenericMethod(member.ReflectedType, member.Type().DetermineCollectionElementType());
            method.Invoke(this, new object[] { mapper, member.Name });
        }

        // This method used in none generic version!!! DON'T REMOVE this method! 
        // ReSharper disable UnusedMember.Local
        private void MapManyToMany<TOwner, TItem>(ConventionModelMapper mapper, string propName) where TOwner : class
        {
            if (this.manyToManyRelations.Contains(new KeyValuePair<Type, Type>(typeof(TOwner), typeof(TItem))))
            {
                return;
            }

            var expression = Expression.Parameter(typeof(TOwner), "x");
            var property = Expression.Property(expression, typeof(TOwner), propName);
            var lambda = Expression.Lambda<Func<TOwner, IEnumerable<TItem>>>(property, "x", new[] { expression });

            bool isSecondSideOfRelation = this.manyToManyRelations.Contains(new KeyValuePair<Type, Type>(typeof(TItem), typeof(TOwner)));
            var tableName = isSecondSideOfRelation ?
                GetManyToManyTableName<TItem, TOwner>() :
                GetManyToManyTableName<TOwner, TItem>();
            mapper.Class<TOwner>(c => c.Set(lambda,
                    cm =>
                    {
                        cm.Cascade(Cascade.All);
                        cm.Table(tableName);
                        cm.Key(km => km.Column(typeof(TOwner).Name + "Fk"));
                        if (!isSecondSideOfRelation)
                        {
                            cm.Inverse(true);
                        }
                    },
                    em => em.ManyToMany(m => m.Column(typeof(TItem).Name + "Fk"))));
            this.manyToManyRelations.Add(new KeyValuePair<Type, Type>(typeof(TOwner), typeof(TItem)));
        }
        // ReSharper restore UnusedMember.Local
	}
}
