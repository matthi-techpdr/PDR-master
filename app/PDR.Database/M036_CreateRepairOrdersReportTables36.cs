using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M036_CreateRepairOrdersReportTables36 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("RepairOrderReports")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("Date", DbType.DateTime)
                .WithNullableColumn("Title", DbType.String).OfSize(255)
                .WithNullableColumn("CompanyFk", DbType.Int64)
                .WithNullableColumn("EmployeeFk", DbType.Int64);

            db.CreateTable("RepairOrderReportItems")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("CreationDate", DbType.DateTime)
                .WithNullableColumn("CustomerName", DbType.String).OfSize(255)
                .WithNullableColumn("TotalSum", DbType.Double)
                .WithNullableColumn("Status", DbType.Int64)
                .WithNullableColumn("RepairOrderReportFk", DbType.Int64)
                .WithNullableColumn("CompanyFk", DbType.Int64);

            db.Tables["RepairOrderReportItems"].AddForeignKeyTo("RepairOrderReports", "Fk_roRepIt_roRep").Through("RepairOrderReportFk", "Id");
            db.Tables["RepairOrderReports"].AddForeignKeyTo("Companies", "Fk_roRep_com").Through("CompanyFk", "Id");
            db.Tables["RepairOrderReports"].AddForeignKeyTo("Users", "Fk_roRep_emp").Through("EmployeeFk", "Id");
        } 
    }
}