using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M031_AddEstimateReportsTables31 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("EstimateReports")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("Date", DbType.DateTime)
                .WithNullableColumn("Title", DbType.String).OfSize(255)
                .WithNullableColumn("CompanyFk", DbType.Int64);

            db.CreateTable("EstimateReportItems")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("CreationDate", DbType.DateTime)
                .WithNullableColumn("CustomerName", DbType.String).OfSize(255)
                .WithNullableColumn("CalculatedSum", DbType.Double)
                .WithNullableColumn("UpdatedSum", DbType.Double)
                .WithNullableColumn("Status", DbType.Int64)
                .WithNullableColumn("EstimateReportFk", DbType.Int64);

            db.Tables["EstimateReportItems"].AddForeignKeyTo("EstimateReports", "Fk_estRepIt_estRep").Through("EstimateReportFk", "Id");
            db.Tables["EstimateReports"].AddForeignKeyTo("Companies", "Fk_estRep_com").Through("CompanyFk", "Id");

        }
    }
}