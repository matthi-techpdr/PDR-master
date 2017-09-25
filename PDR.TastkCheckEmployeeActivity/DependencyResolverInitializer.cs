using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.NHibernateProvider.Repositories;
using SmartArch.Core.Domain.Base;
using SmartArch.Data;
using SmartArch.Data.NH;
using SmartArch.Data.NH.Proxy;
using SmartArch.Data.Proxy;
using PDR.Init.IoC;
using System.Reflection;
using PDR.Init.FluentValidation;
using StructureMap;
using NHibernate;
using NHibernate.Dialect;
using PDR.Domain.Model.Base;
using PDR.NHibernateProvider;

namespace PDR.TaskCheckEmployeeActivity
{
    public class DependencyResolverInit
    {
        public static void Initialize()
        {
            var container = new Container(x => 
            {
                x.For<ISessionFactory>().Singleton().Use(() =>
                {var v = NHibernateInitializer<MsSql2008Dialect>.Initialize().BuildSessionFactory(); return v;});
                x.For<ISession>().Use(a => a.GetInstance<ISessionFactory>().OpenSession());
                x.For(typeof(ICurrentStorage<>)).Use(typeof(CurrentStorage<>));
                x.For(typeof(IRepository<>)).Use(typeof(Repository<>));
                x.For(typeof(INativeRepository<>)).Use(typeof(NativeRepository<>));
                x.For(typeof(IProxyAnalyzer)).Use(typeof(ProxyAnalyzer));
                x.For(typeof(ITransactionManager)).Use(typeof(TransactionManager));
                x.For(typeof(Domain.Contracts.Repositories.IRepositoryFactory)).Use(typeof(NHibernateProvider.Repositories.RepositoryFactory));

                x.Scan(scanner =>
                    {
                        scanner.AssemblyContainingType<TransactionManager>();
                        scanner.AssemblyContainingType<BaseEntity>();
                        scanner.WithDefaultConventions();
                    });
            });

            ObjectFactory.Configure(cfg => cfg.AddRegistry(new RegistryValidators(Assembly.Load("PDR.Domain"))));
        
            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(container));
        }
    }
}