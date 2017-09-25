using System;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using NHibernate.Dialect;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model.Base;
using PDR.Domain.Services.Account;
using PDR.Domain.Services.Grid;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.Logging;
using PDR.Domain.Services.Mailing;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.Sheduler;
using PDR.Domain.Services.TempImageStorage;
using PDR.Domain.Services.VersionStorage;
using PDR.Domain.Services.Webstorage;
using PDR.Domain.Services.XLS;

using PDR.Init.FluentValidation;
using PDR.NHibernateProvider;
using PDR.NHibernateProvider.Repositories;

using SmartArch.Core.Domain.Base;
using SmartArch.Data;
using SmartArch.Data.Fetching;
using SmartArch.Data.NH;
using SmartArch.Data.NH.Fetching;
using SmartArch.Data.NH.Proxy;
using SmartArch.Data.Proxy;

using StructureMap;

namespace PDR.Init.IoC
{
    using System.Reflection;

    public class DependencyResolverInitializer
    {
        public static void Initialize(Action<ConfigurationExpression> customConfig = null)
        {
            var container = new Container(x => 
            {
                x.For<ISessionFactory>().Singleton().Use(() => {var v = NHibernateInitializer<MsSql2008Dialect>.Initialize().BuildSessionFactory(); return v; });
                x.For<ISession>().HttpContextScoped().Use(a => a.GetInstance<ISessionFactory>().OpenSession());
                x.For(typeof(IAuthorizationProvider)).Use(typeof(AuthorizationProvider));
                x.For(typeof(ICurrentWebStorage<>)).HttpContextScoped().Use(typeof(CurrentWebStorage<>));
                x.For(typeof(IVersionStorage<>)).HttpContextScoped().Use(typeof(VersionStorage<>));
                x.For(typeof(IGridMaster<,,>)).HttpContextScoped().Use(typeof(GridMaster<,,>));
                x.For(typeof(IGridMasterForStoredProcedure<,,>)).HttpContextScoped().Use(typeof(GridMasterNew<,,>));
                x.For(typeof(ISuperadminGridMaster<,,>)).HttpContextScoped().Use(typeof(SuperadminGridMaster<,,>));
                x.For(typeof(IRepository<>)).Use(typeof(Repository<>));
                x.For(typeof(INativeRepository<>)).Use(typeof(NativeRepository<>));
                x.For(typeof(ICompanyRepository<>)).Use(typeof(CompanyRepository<>));
                x.For(typeof(IProxyAnalyzer)).Use(typeof(ProxyAnalyzer));
                x.For(typeof(ITransactionManager)).HttpContextScoped().Use(typeof(TransactionManager));

                x.For(typeof(IFetchingProvider)).HttpContextScoped().Use(typeof(NhFetchingProvider));
                x.For(typeof(IFetchRequest<,>)).HttpContextScoped().Use(typeof(FetchRequest<,>));

                x.For(typeof(ITempImageManager)).HttpContextScoped().Use(typeof(TempImageManager));

                x.For(typeof(IPdfConverter)).HttpContextScoped().Use(typeof(PdfConverter));
                x.For(typeof(ILogger)).HttpContextScoped().Use(typeof(Logger));
                x.For(typeof(ISheduler)).HttpContextScoped().Use(typeof(Sheduler));
                x.For(typeof(IMailService)).HttpContextScoped().Use(typeof(MailService));
                x.For(typeof(IXLSGenerator)).HttpContextScoped().Use(typeof(XLSGenerator));
                x.For(typeof(ReassignHelper)).HttpContextScoped().Use(typeof(ReassignHelper));

                x.For(typeof(Domain.Contracts.Repositories.IRepositoryFactory)).HttpContextScoped().Use(typeof(PDR.NHibernateProvider.Repositories.RepositoryFactory));
                x.Scan(scanner =>
                    {
                        scanner.AssemblyContainingType<TransactionManager>();
                        scanner.AssemblyContainingType<BaseEntity>();
                        scanner.WithDefaultConventions();
                    });
            });

            if (customConfig != null)
            {
                container.Configure(customConfig); 
            }
            

            ObjectFactory.Configure(cfg => cfg.AddRegistry(new RegistryValidators()));
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(container));
            ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(container));
        }

        public static void ReleaseAndDisposeAllHttpScopedObjects()
        {
            // Make sure to dispose of NHibernate session if created on this web request
            ObjectFactory.ReleaseAndDisposeAllHttpScopedObjects();
        }
    }
}