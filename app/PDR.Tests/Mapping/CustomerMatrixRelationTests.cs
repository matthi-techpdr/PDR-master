using NHibernate;
using NHibernate.Dialect;

using NUnit.Framework;

using PDR.Domain.Model;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Matrixes;
using PDR.Init.IoC;
using PDR.NHibernateProvider;
using PDR.Tests.Domain.Helpers;

namespace PDR.Tests.Mapping
{
    public class CustomerMatrixRelationTests
    {
        private ISessionFactory sessionFactory;

        private ISession session;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            DependencyResolverInitializer.Initialize();
            CeDatabaseCreator.Create();
            var configuration = NHibernateInitializer<MsSqlCeDialect>.Initialize(null, true, CeDatabaseCreator.ConnectionString);
            this.sessionFactory = configuration.BuildSessionFactory();
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            this.sessionFactory.Close();
        }

        [SetUp]
        public void SetUp()
        {
            this.session = this.sessionFactory.OpenSession();
        }

        [TearDown]
        public void TearDown()
        {
            this.session.Close();
        }

        [Test]
        public void Relation_CustomerAndMatrix_CanSaveRelation()
        {
            this.session.BeginTransaction();
            var company = new Company { Name = "Company" };
            var customer = new RetailCustomer { Company = company, FirstName = "First", LastName = "Last" };
            var matrix = new DefaultMatrix { Company = company };
           
            this.session.Save(company);
            this.session.Save(customer);
            this.session.Save(matrix);
            this.session.Transaction.Commit();
        }
    }
}