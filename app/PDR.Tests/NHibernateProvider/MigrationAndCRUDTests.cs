using System;
using System.Collections;
using System.Linq;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NUnit.Framework;
using PDR.Domain.Model;
using PDR.Domain.Model.Base;
using PDR.Domain.Services;
using PDR.NHibernateProvider;
using PDR.Tests.Domain;
using PDR.Tests.Domain.Helpers;

namespace PDR.Tests.NHibernateProvider
{
    [TestFixture]
    public class MigrationAndCRUDTests
    {
        /// <summary>
        /// All entities types.
        /// </summary>
        private readonly InstanceCreator instanceCreator = new InstanceCreator();

        /// <summary>
        /// Nhibernate config.
        /// </summary>
        private Configuration configuration;

        /// <summary>
        /// NHibernate session.
        /// </summary>
        private ISession session;

        [TestFixtureSetUp]
        public virtual void SetUp()
        {
            CeDatabaseCreator.Create();

            this.configuration = NHibernateInitializer<MsSqlCe40Dialect>.Initialize(test: true, connectionString: CeDatabaseCreator.ConnectionString);
            var factory = this.configuration.BuildSessionFactory();
            this.session = factory.OpenSession();

            CeDatabaseCreator.ExecuteMigrations();
        }

        [Test]
        public void OneEntityCreationIsCorrect()
        {
            var testInstance = this.instanceCreator.GetInstances().Where(s => s.GetType() == typeof(Estimate)).SingleOrDefault();
            if (testInstance != null)
            {
                using (var transaction = this.session.BeginTransaction())
                {
                    this.session.SaveOrUpdate(testInstance);
                    transaction.Commit();
                }
            }
        }


        [Test]
        public void EntitiesCreationIsCorrect()
        {
            var testInstances = this.instanceCreator.GetInstances().ToList();
            foreach (var inst in testInstances)
            {
               var count = this.session.CreateCriteria(inst.GetType().Name).List().Count;
                    using (var transaction = this.session.BeginTransaction())
                    {
                        this.session.SaveOrUpdate(inst);
                        transaction.Commit();
                    }

                var newCount = this.session.CreateCriteria(inst.GetType().Name).List().Count;
                Assert.AreEqual(newCount, count + 1);
            }
        }

        [Test]
        public void EntitiesGetterIsCorrect()
        {
            foreach (var type in InstanceCreator.EntitiesTypes)
            {
                var list = this.session.CreateCriteria(type.Name).List();
                Assert.That(list, Is.AssignableTo(typeof(IList)));
                Assert.That(list, Is.All.InstanceOf(type));
            }
        }

        [Test]
        public void EntitiesDeletingIsCorrect()
        {
            foreach (var type in InstanceCreator.EntitiesTypes)
            {
                var testEntity = (Entity)this.session.CreateCriteria(type.Name).List()[0];
                if (testEntity != null)
                {
                    var testEntityId = testEntity.Id;
                    using (var transaction = this.session.BeginTransaction())
                    {
                        this.session.Delete(testEntity);
                        transaction.Commit();
                    }

                    Assert.IsNull(this.session.Get(type.Name, testEntityId));
                }
            }
        }
    }
}
