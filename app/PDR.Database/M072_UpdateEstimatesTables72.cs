using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M072_UpdateEstimatesTables72 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Estimates"]
                 .AddNullableColumn("EstHourlyRate", DbType.Double)
                 .AddNullableColumn("EstDiscount", DbType.Double)
                 .AddNullableColumn("EstLaborTax", DbType.Double)
                 .AddNullableColumn("EstMaxCorProtect", DbType.Double)
                 .AddNullableColumn("EstAluminiumPer", DbType.Double)
                 .AddNullableColumn("EstOversizedRoofPer", DbType.Double)
                 .AddNullableColumn("EstDoubleMetalPer", DbType.Double)
                 .AddNullableColumn("EstLimitForBodyPart", DbType.Double)
                 .AddNullableColumn("EstOversizedDents", DbType.Double)
                 .AddNullableColumn("EstMaxPercent", DbType.Double)
                 .AddNullableColumn("EstCorProtectPart", DbType.Double);
        }
    }
}
