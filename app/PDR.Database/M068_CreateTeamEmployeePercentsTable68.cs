using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M068_CreateTeamEmployeePercentsTable68 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("TeamEmployeePercents")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("TeamEmployeeFk", DbType.Int64)
                .WithNullableColumn("EmployeePart", DbType.Double)
                .WithNullableColumn("RepairOrderFk", DbType.Int64)
                .WithNullableColumn("CompanyFk", DbType.Int64);

            db.Tables["TeamEmployeePercents"].AddForeignKeyTo("RepairOrders", "Fk_teamEmpPer_RO").Through("RepairOrderFk", "Id");
        }
    }
}
