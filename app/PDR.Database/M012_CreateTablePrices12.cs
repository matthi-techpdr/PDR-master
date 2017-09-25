using System.Data;
using MigSharp;

namespace PDR.Database
{
	/// <summary>
	/// Create pricematrix item table
	/// </summary>
	[MigrationExport]
	public class M012_CreateTablePrices12 : IMigration
	{
		public void Up(IDatabase db)
		{
			db.CreateTable("Prices")
				.WithPrimaryKeyColumn("Id", DbType.Int64)
				.WithNullableColumn("PartOfBody", DbType.Int16)
				.WithNullableColumn("AverageSize", DbType.Int16)
				.WithNullableColumn("TotalDents", DbType.Int16)
				.WithNullableColumn("Cost", DbType.Double)
				.WithNullableColumn("EstimateFk", DbType.Int64)
				.WithNullableColumn("MatrixFk", DbType.Int64)
                .WithNullableColumn("CompanyFk", DbType.Int64)
				.WithNullableColumn("Class", DbType.String).OfSize(255);
		}
	}
}
