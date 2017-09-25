using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Fasterflect;

using NHibernate.Mapping.ByCode;

using PDR.Domain.Model.Base;
using PDR.Domain.Model.Photos;

namespace PDR.NHibernateProvider.Overrides
{
	/// <summary>
	/// customers override mapping
	/// </summary>
	public class OneToOneMappingOverride : IOverride
	{
        private static MethodInfo mapMethod;

        private readonly IDictionary<Type, Type> oneToOneRelations = new Dictionary<Type, Type>();
        
        public void Override(ModelMapper mapper)
		{
		    var types = typeof(Entity).Assembly.GetExportedTypes().Where(x => Conventions.IsEntity(x));
            foreach (var type in types)
            {
                var manyToManyProperties = type.Properties().Where(x => Conventions.IsOneToOne(x));
                foreach (var property in manyToManyProperties)
                {
                    this.MapOneToOne(mapper, property);
                }
            }
		}

        private static Expression<Func<T, TProperty>> GetPropertyExpression<T, TProperty>(string propertyName)
        {
            var expression = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(expression, typeof(T), propertyName);
            var lambda = Expression.Lambda<Func<T, TProperty>>(property, "x", new[] { expression });

            return lambda;
        }

        private void MapOneToOne(ModelMapper mapper, MemberInfo member)
        {
            if (mapMethod == null)
            {
                var type = this.GetType();
                mapMethod =
                    type.Methods(Flags.InstancePrivate, "MapOneToOne").Single(m => m.IsGenericMethod);
            }

            mapMethod
                .MakeGenericMethod(member.ReflectedType, member.Type())
                .Invoke(this, new object[] { mapper, member.Name });
        }

        // This method used in none generic version!!! DON'T REMOVE this method! 
        // ReSharper disable UnusedMember.Local
        private void MapOneToOne<TOwner, TItem>(ConventionModelMapper mapper, string propertyName) where TOwner : class where TItem : class
        {            
            if (this.oneToOneRelations.Contains(new KeyValuePair<Type, Type>(typeof(TOwner), typeof(TItem))))
            {
                return;
            }

            var lambda = GetPropertyExpression<TOwner, TItem>(propertyName);
            bool isSecondSideOfRelation = this.oneToOneRelations.Contains(new KeyValuePair<Type, Type>(typeof(TItem), typeof(TOwner)));
            if (isSecondSideOfRelation)
            {
                mapper.Class<TOwner>(c => c.ManyToOne(lambda, cm => { cm.Cascade(Cascade.Persist); cm.Unique(true); }));
            }
            else
            {
                var secondSideMember = Conventions.GetRelationSecondSide(typeof(TOwner), typeof(TItem));
                mapper.Class<TOwner>(c => c.OneToOne(lambda,
                    cm =>
                    {
                        cm.Constrained(false);
                        cm.PropertyReference(secondSideMember);
                        cm.Cascade(Cascade.Persist);
                    }));
            }

            if (typeof(TItem) != typeof(CarImage))
            {
                this.oneToOneRelations.Add(typeof(TOwner), typeof(TItem));
            }
            else
            {
                this.oneToOneRelations.Add(typeof(TItem), typeof(TOwner));
            }
        }
        // ReSharper restore UnusedMember.Local        
	}
}
