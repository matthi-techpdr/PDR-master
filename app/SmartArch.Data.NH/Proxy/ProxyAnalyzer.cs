using System;

using NHibernate.Proxy;

using SmartArch.Data.Proxy;

namespace SmartArch.Data.NH.Proxy
{
    public class ProxyAnalyzer : IProxyAnalyzer
    {
        public object GetPersist(object proxy)
        {
            if (proxy is INHibernateProxy)
            {
                var lazyInitialiser = ((INHibernateProxy)proxy).HibernateLazyInitializer;
                var type = lazyInitialiser.PersistentClass;
                if (type.IsAbstract || type.GetNestedTypes().Length > 0)
                {
                    var persistObj = lazyInitialiser.GetImplementation();

                    return persistObj;
                }
            }

            return proxy;
        }

        /// <summary>
        /// Returns the real type of the given proxy. If the object is not a proxy, it's normal type is returned.
        /// </summary>
        public Type GetPersistType(object proxy)
        {
            var persist = this.GetPersist(proxy);

            return persist.GetType();
        }
    }
}