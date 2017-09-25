using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NHibernate.Mapping.ByCode;

namespace PDR.NHibernateProvider
{
    public static class PropertyPathExtensions
    {
        private const string MANY_TO_MANY_INTERMEDIATE_TABLE_INFIX = "_";

        public static Type Owner(this PropertyPath member)
        {
            return member.GetRootMember().DeclaringType;
        }

        public static Type CollectionElementType(this PropertyPath member)
        {
            return member.LocalMember.GetPropertyOrFieldType().DetermineCollectionElementOrDictionaryValueType();
        }

        public static MemberInfo OneToManyOtherSideProperty(this PropertyPath member)
        {
            return member.CollectionElementType().GetFirstPropertyOfType(member.Owner());
        }

        public static string ManyToManyIntermediateTableName(this PropertyPath member)
        {
            return string.Join(MANY_TO_MANY_INTERMEDIATE_TABLE_INFIX, member.ManyToManySidesNames().OrderBy(x => x));
        }

        private static IEnumerable<string> ManyToManySidesNames(this PropertyPath member)
        {
            yield return member.Owner().Name;
            yield return member.CollectionElementType().Name;
        }
    }
}