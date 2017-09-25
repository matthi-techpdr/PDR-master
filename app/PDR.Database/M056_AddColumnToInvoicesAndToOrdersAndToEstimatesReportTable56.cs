using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M056_AddColumnToInvoicesAndToOrdersAndToEstimatesReportTable56 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["EstimateReportItems"].AddNullableColumn("EstimateID", DbType.Int64);
            db.Tables["RepairOrderReportItems"].AddNullableColumn("RepairOrderID", DbType.Int64);
            db.Tables["InvoiceReportItems"].AddNullableColumn("InvoiceID", DbType.Int64);
        }
    }
}
