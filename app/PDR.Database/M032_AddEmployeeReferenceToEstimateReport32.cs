using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M032_AddEmployeeReferenceToEstimateReport32 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["EstimateReports"].AddNullableColumn("EmployeeFk", DbType.Int64);
            db.Tables["EstimateReports"].AddForeignKeyTo("Users", "Fk_estRep_emp").Through("EmployeeFk", "Id");
        }
    }
}