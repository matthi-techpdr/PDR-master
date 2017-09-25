using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M074_CreateReportsTable74 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("Reports")
                .WithNullableColumn("Id", DbType.Int64)
                .WithNullableColumn("Class", DbType.String).OfSize(255)
                .WithNullableColumn("StartDate", DbType.String).OfSize(255)
                .WithNullableColumn("EndDate", DbType.String).OfSize(255)
                .WithNullableColumn("TeamId", DbType.Int64)
                .WithNullableColumn("CustomerId", DbType.Int64)
                .WithNullableColumn("Commission", DbType.Boolean)
                .WithNullableColumn("Title", DbType.String).OfSize(255)
                .WithNullableColumn("EmployeeFk", DbType.Int64)
                .WithNullableColumn("CompanyFk", DbType.Int64);
        }
    }
}
