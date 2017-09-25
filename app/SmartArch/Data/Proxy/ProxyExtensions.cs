using System;

using Microsoft.Practices.ServiceLocation;

using SmartArch.Core.Helpers.EntityLocalization;

namespace SmartArch.Data.Proxy
{
    public static class ProxyExtensions
    {
        /// <summary>
        /// The helper's engine.
        /// </summary>
        private static IProxyAnalyzer engine;

        /// <summary>
        /// Gets a value indicating whether this instance has default engine.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has default engine; otherwise, <c>false</c>.
        /// </value>
        public static bool IsDefaultEngine { get; private set; }

        /// <summary>
        /// Initializes static members of the <see cref="EntityLocalizationHelper"/> class.
        /// </summary>
        static ProxyExtensions()
        {
            SetDefaultEngine();
        }

        /// <summary>
        /// Gets or sets the engine.
        /// </summary>
        /// <value>The helper's engine.</value>
        public static IProxyAnalyzer Engine
        {
            get
            {
                return engine ?? (engine = ServiceLocator.Current.GetInstance<IProxyAnalyzer>());
            }

            set
            {
                engine = value;
                IsDefaultEngine = false;
            }
        }

        /// <summary>
        /// Sets the default engine.
        /// </summary>
        public static void SetDefaultEngine()
        {
            engine = null;
            IsDefaultEngine = true;
        }

        /// <summary>
        /// Returns the real type of the given proxy. If the object is not a proxy, it's normal type is returned.
        /// </summary>
        public static Type GetPersistType(this object proxy)
        {
            Type persistenceType = Engine.GetPersistType(proxy);

            return persistenceType;
        }

        public static bool IsPersist<T>(this object proxy)
        {
            Type persistenceType = Engine.GetPersistType(proxy);
            bool isPersist = persistenceType == typeof(T);

            return isPersist;
        }

        public static T ToPersist<T>(this object proxy) where T : class
        {
            var persistObj = Engine.GetPersist(proxy) as T;

            return persistObj;
        }
    }
}