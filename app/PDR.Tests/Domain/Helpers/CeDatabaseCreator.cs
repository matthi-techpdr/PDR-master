using System.Data.SqlServerCe;
using System.IO;
using System.Reflection;

using MigSharp;

using PDR.Database;

namespace PDR.Tests.Domain.Helpers
{
    public static class CeDatabaseCreator
    {
        private static readonly string DbPath;

        static CeDatabaseCreator()
        {
            var currLocationPath = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).FullName;
            var currDir = new DirectoryInfo(Path.Combine(currLocationPath, "Databases"));
            if (!currDir.Exists)
            {
                currDir.Create();
            }

            DbPath = Path.Combine(currDir.FullName, "PDRce.sdf");
        }

        public static string ConnectionString
        {
            get
            {
                return string.Format(@"Data Source={0}", DbPath);
            }
        }

        public static void Create()
        {
            if (File.Exists(DbPath))
            {
                File.Delete(DbPath);
            }

            var engine = new SqlCeEngine(ConnectionString);
            engine.CreateDatabase();
        }

        public static void ExecuteMigrations()
        {
            var migrator = new Migrator(ConnectionString, ProviderNames.SqlServerCe4);
            migrator.MigrateAll(Assembly.GetAssembly(typeof(M001_CreateTableHiLo1)));
        }
    }
}
