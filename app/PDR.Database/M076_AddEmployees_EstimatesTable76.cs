using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M076_AddEmployees_EstimatesTable76 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("PreEmployees_PreEstimates")
                .WithNullableColumn("EmployeeFk", DbType.Int64)
                .WithNullableColumn("EstimateFk", DbType.Int64);
        }
    }
}
