using System.Data;

using MigSharp;

namespace PDR.Database
{
	[MigrationExport]
    public class M021_UpdateTableCuctomers21 : IMigration
	{
		public void Up(IDatabase db)
		{
			db.Tables["Customers"]
				.AddNullableColumn("ContactName", DbType.String)
				.AddNullableColumn("ContactTitle", DbType.String)
				.AddNullableColumn("Password", DbType.String)
				.AddNullableColumn("Discount", DbType.Int32)
				.AddNullableColumn("LaborRate", DbType.Double)
				.AddNullableColumn("PartRate", DbType.Int32)
				.AddNullableColumn("HourlyRate", DbType.Double)
				.AddNullableColumn("Insurance", DbType.Boolean)
				.AddNullableColumn("EstimateSignature", DbType.Boolean)
				.AddNullableColumn("OrderSignature", DbType.Boolean)
				.AddNullableColumn("WorkByThemselve", DbType.Boolean)
                .AddNullableColumn("Comment", DbType.String).OfSize(1100);
		}
	}
}
