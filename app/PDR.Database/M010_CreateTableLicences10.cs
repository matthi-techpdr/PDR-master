namespace PDR.Database
{
    using System.Data;

    using MigSharp;

    [MigrationExport]
    public class M010_CreateTableLicences10 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("Licenses")
                .WithPrimaryKeyColumn("Id", DbType.Int64)                
                .WithNullableColumn("DeviceId", DbType.String).OfSize(255)
                .WithNullableColumn("DeviceName", DbType.String).OfSize(255)
                .WithNullableColumn("DeviceType", DbType.Int16)
                .WithNullableColumn("PhoneNumber", DbType.String).OfSize(255)
                .WithNullableColumn("GpsReportFrequency", DbType.Int16)
                .WithNullableColumn("CreationDate", DbType.DateTime)
                .WithNullableColumn("EmployeeFk", DbType.Int64)
                .WithNullableColumn("CompanyFk", DbType.Int64)
                .WithNullableColumn("Status", DbType.Int16);
        }
    }
}
