using System;

namespace PDR.Domain.Model.Enums.Extensions
{
    using System.Reflection;

    using PDR.Domain.Model.Enums.Attributes;

    public static class EnumExtensions
    {
        public static String GetDescription(this Enum item)
        {
            MemberInfo[] info = item.GetType().GetMember(item.ToString());
            if (info == null || info.Length == 0)
            {
                throw new Exception("Enum name can't be accessed as there is no member info");
            }

            object[] attributes = info[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes == null || attributes.Length == 0)
            {
                throw new Exception("Enum name is not defined");
            }
            return ((DescriptionAttribute)attributes[0]).Description;
        }
    }
}
