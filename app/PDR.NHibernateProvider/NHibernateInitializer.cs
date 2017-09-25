using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using PDR.Domain.Model.Base;

namespace PDR.NHibernateProvider
{
    public static class NHibernateInitializer<T> where T : Dialect
    {
        private static Configuration configuration;

        public static Configuration Initialize(string connectionStringName = "PDRConnectionString", bool test = false, string connectionString = null)
        {
            if (configuration == null)
            {
                configuration = new Configuration();
                configuration.Proxy(p => p.ProxyFactoryFactory<DefaultProxyFactoryFactory>()).DataBaseIntegration(
                    db =>
                        {
                            db.ConnectionStringName = !test ? connectionStringName : null;
                            db.Dialect<T>();
                            if (test)
                            {
                                db.ConnectionString = connectionString;
                            }
                        }).AddAssembly(typeof(Entity).Assembly);

                var mapper = new ConventionModelMapper();
                mapper.WithConventions(configuration);
            }
            return configuration;
        }
    }
}