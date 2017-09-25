using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NixJqGridFramework.Server
{
    using NixJqGridFramework.Entities.Enums;

    public class Search
    {
        public static IEnumerable<T> SearchItem<T>(string searchField, string searchOper, string searchString, IEnumerable<T> list)
        {
            var searched = new object();

            searched = ContextClass<T>.GetDataItems(searchField, searchString, searchOper, list);

            return searched as IEnumerable<T>;
        }
    }

    public interface IOper<T>
    {
        List<T> GetItems(string searchField, string searchString, IEnumerable<T> list);
    }

    public class EqOper<T> : IOper<T>
    {
        public List<T> GetItems(string searchField, string searchString, IEnumerable<T> list)
        {
            return list.Where(x => typeof(T).GetProperty(searchField).GetValue(x, null).ToString() == searchString).ToList();
        }
    }

    public class NeOper<T> : IOper<T>
    {
        public List<T> GetItems(string searchField, string searchString, IEnumerable<T> list)
        {
            return list.Where(x => typeof(T).GetProperty(searchField).GetValue(x, null).ToString() != searchString).ToList();
        }
    }

    public class BwOper<T> : IOper<T>
    {
        public List<T> GetItems(string searchField, string searchString, IEnumerable<T> list)
        {
            return list.Where(x => typeof(T).GetProperty(searchField).GetValue(x, null).ToString().StartsWith(searchString)).ToList();
        }
    }

    public class BnOper<T> : IOper<T>
    {
        public List<T> GetItems(string searchField, string searchString, IEnumerable<T> list)
        {
            return list.Where(x => !typeof(T).GetProperty(searchField).GetValue(x, null).ToString().StartsWith(searchString)).ToList();
        }
    }

    public class EwOper<T> : IOper<T>
    {
        public List<T> GetItems(string searchField, string searchString, IEnumerable<T> list)
        {
            return list.Where(x => typeof(T).GetProperty(searchField).GetValue(x, null).ToString().EndsWith(searchString)).ToList();
        }
    }

    public class EnOper<T> : IOper<T>
    {
        public List<T> GetItems(string searchField, string searchString, IEnumerable<T> list)
        {
            return list.Where(x => !typeof(T).GetProperty(searchField).GetValue(x, null).ToString().EndsWith(searchString)).ToList();
        }
    }

    public class CnOper<T> : IOper<T>
    {
        public List<T> GetItems(string searchField, string searchString, IEnumerable<T> list)
        {
            return list.Where(x => typeof(T).GetProperty(searchField).GetValue(x, null).ToString().Contains(searchString)).ToList();
        }
    }

    public class NcOper<T> : IOper<T>
    {
        public List<T> GetItems(string searchField, string searchString, IEnumerable<T> list)
        {
            return list.Where(x => !typeof(T).GetProperty(searchField).GetValue(x, null).ToString().Contains(searchString)).ToList();
        }
    }

    public class InOper<T> : IOper<T>
    {
        public List<T> GetItems(string searchField, string searchString, IEnumerable<T> list)
        {
            return null;
        }
    }

    public class NiOper<T> : IOper<T>
    {
        public List<T> GetItems(string searchField, string searchString, IEnumerable<T> list)
        {
            return null;
        }
    }

    public class NuOper<T> : IOper<T>
    {
        public List<T> GetItems(string searchField, string searchString, IEnumerable<T> list)
        {
            return list.Where(x => typeof(T).GetProperty(searchField).GetValue(x, null) == null).ToList();
        }
    }

    public class NnOper<T> : IOper<T>
    {
        public List<T> GetItems(string searchField, string searchString, IEnumerable<T> list)
        {
            return list.Where(x => typeof(T).GetProperty(searchField).GetValue(x, null) != null).ToList();
        }
    }

    public class ContextClass<T>
    {
        private static Dictionary<string, IOper<T>> strategy = new Dictionary<string, IOper<T>>();

        static ContextClass()
        {
            strategy.Add(Operators.eq.ToString(), new EqOper<T>());
            strategy.Add(Operators.ne.ToString(), new NeOper<T>());
            strategy.Add(Operators.bw.ToString(), new BwOper<T>());
            strategy.Add(Operators.bn.ToString(), new BnOper<T>());
            strategy.Add(Operators.ew.ToString(), new EwOper<T>());
            strategy.Add(Operators.en.ToString(), new EnOper<T>());
            strategy.Add(Operators.cn.ToString(), new CnOper<T>());
            strategy.Add(Operators.nc.ToString(), new NcOper<T>());
            strategy.Add(Operators.In.ToString().ToLower(), new InOper<T>());
            strategy.Add(Operators.ni.ToString(), new NiOper<T>());
            strategy.Add(Operators.nu.ToString(), new NuOper<T>());
            strategy.Add(Operators.nn.ToString(), new NnOper<T>());
        }

        public static List<T> GetDataItems(string searchField, string searchString, string oper, IEnumerable<T> list)
        {
            return strategy[oper].GetItems(searchField, searchString, list);
        }
    }
}
