using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M094_CreateTableVersioniPhoneApp94 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("VersioniPhoneApps")
                 .WithPrimaryKeyColumn("Id", DbType.Int64)
                 .WithNullableColumn("Version", DbType.String).OfSize(255)
                 .WithNullableColumn("LocalStoragePath", DbType.String)
                 .WithNullableColumn("IsWorkingBild", DbType.Boolean)
                 .WithNullableColumn("DateUpload", DbType.DateTime)
                 .WithNullableColumn("IsDownLoaded", DbType.Boolean);
        }
    }
}