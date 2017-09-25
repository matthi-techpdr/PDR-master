using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NixJqGridFramework.Server
{
    public class Sort
    {
        /// <summary>
        /// Sorts the columns.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sidx">The sidx.</param>
        /// <param name="sord">The sord.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static List<T> SortColumns<T>(string sidx, string sord, List<T> data)
        {
            if (sord == "asc")
            {
                return sortBy(data, sidx + " asc") as List<T>;
            }
            else
            {
                return sortBy(data, sidx + " desc") as List<T>;
            }
        }

        /// <summary>
        /// Sorts the by.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="sortExpression">The sort expression.</param>
        /// <returns></returns>
        private static IEnumerable<T> sortBy<T>(IEnumerable<T> list, string sortExpression)
        {
            sortExpression += "";
            string[] parts = sortExpression.Split(' ');
            bool descending = false;
            string property = "";

            if (parts.Length > 0 && parts[0] != "")
            {
                property = parts[0];

                if (parts.Length > 1)
                {
                    @descending = parts[1].ToLower().Contains("esc");
                }

                PropertyInfo prop = typeof(T).GetProperty(property);

                if (prop == null)
                {
                    throw new Exception("No property '" + property + "' in + " + typeof(T).Name + "'");
                }

                if (@descending)
                    list = list.OrderByDescending(x => prop.GetValue(x, null)).ToList();
                else
                    list = list.OrderBy(x => prop.GetValue(x, null)).ToList();
            }

            return list;
        }
    }
}
