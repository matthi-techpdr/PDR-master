using System;
using System.Drawing;

using PDR.Domain.Model.Attributes;

namespace PDR.Domain.Model.Enums
{
    public static class EnumExtenssions
    {
        public static Image GetImage(this PartOfBody value, PartColor color, bool truck = false)
        {
            Type type = value.GetType();

            var fi = type.GetField(value.ToString());
            var attrs = truck ? 
                fi.GetCustomAttributes(typeof(TruckPartImageAttribute), false) as BasePartImageAttribute[] :
                fi.GetCustomAttributes(typeof(CarPartImageAttribute), false) as BasePartImageAttribute[];
            string url = null;
            if (attrs != null && attrs.Length == 1)
            {
                var carPartAttribute = attrs[0];
                switch (color)
                {
                    case PartColor.Grey:
                        url = carPartAttribute.Grey;
                        break;
                    case PartColor.Green:
                        url = carPartAttribute.Green;
                        break;
                    case PartColor.Red:
                        url = carPartAttribute.Red;
                        break;
                }
            }
            var result = url != null ? Image.FromFile(url) : null;
            return result;
        }

        public static string GetTotalAmountName(this TotalDents value)
        {
            try
            {
                Type type = value.GetType();

                var fi = type.GetField(value.ToString());
                var attrs = fi.GetCustomAttributes(typeof(TotalDentsStringAttribute), false) as TotalDentsStringAttribute[];

                if (attrs != null && attrs.Length == 1)
                {
                    return attrs[0].Name;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static string GetAverageSizeName(this AverageSize value)
        {
            try
            {
                Type type = value.GetType();

                var fi = type.GetField(value.ToString());
                var attrs = fi.GetCustomAttributes(typeof(AverageSizeStringAttribute), false) as AverageSizeStringAttribute[];

                if (attrs != null && attrs.Length == 1)
                {
                    return attrs[0].Name;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}