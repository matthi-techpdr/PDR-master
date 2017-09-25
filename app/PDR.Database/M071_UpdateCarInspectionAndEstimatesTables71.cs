using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M071_UpdateCarInspectionAndEstimatesTables71 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["CarInspections"]
                .AddNullableColumn("AdditionalPercentsAmount", DbType.Double)
                .AddNullableColumn("AluminiumAmount", DbType.Double)
                .AddNullableColumn("DoubleMetallAmount", DbType.Double)
                .AddNullableColumn("OversizedRoofAmount", DbType.Double);

            db.Tables["Estimates"]
                 .AddNullableColumn("LaborSum", DbType.Double)
                 .AddNullableColumn("Subtotal", DbType.Double)
                .AddNullableColumn("DiscountSum", DbType.Double)
                .AddNullableColumn("TaxSum", DbType.Double)
                .AddNullableColumn("TotalAmount", DbType.Double);
        }
    }
}
