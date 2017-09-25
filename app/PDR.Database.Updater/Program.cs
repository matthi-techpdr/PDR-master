using System.Configuration;
using System.Reflection;

using MigSharp;

namespace PDR.Database.Updater
{
    public class Program
    {
        public static void Main()
        {
            var options = new MigrationOptions();
            options.SupportedProviders.Remove(ProviderNames.SQLite);
            //var migrator = new Migrator(ConfigurationManager.ConnectionStrings["dev"].ConnectionString, ProviderNames.SqlServer2008, options);

            var migrator = new Migrator(@"Password=pdr_pas215;Persist Security Info=True;User ID=pdruser;Initial Catalog=PDR_Test;Data Source=.\SQLEXPRESS", ProviderNames.SqlServer2008, options);

            migrator.MigrateAll(Assembly.GetAssembly(typeof(M001_CreateTableHiLo1)));
        }
    }
}