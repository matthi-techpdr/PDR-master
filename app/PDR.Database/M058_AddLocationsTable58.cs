using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M058_AddLocationsTable58 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("Locations")
                .WithNotNullableColumn("Id", DbType.Int64)
                .WithNullableColumn("LicenseFk", DbType.Int64)
                .WithNullableColumn("Date", DbType.DateTime)
                .WithNullableColumn("Lat", DbType.Double)
                .WithNullableColumn("Lng", DbType.Double)
                .WithNullableColumn("CompanyFk", DbType.Int64);

            db.Tables["Locations"].AddForeignKeyTo("Companies").Through("CompanyFk", "Id");
            db.Tables["Locations"].AddForeignKeyTo("Licenses").Through("LicenseFk", "Id");
        }
    }
}
