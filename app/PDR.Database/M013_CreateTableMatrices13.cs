using System.Data;

using MigSharp;

namespace PDR.Database
{
	/// <summary>
	/// Create pricematrix
	/// </summary>
	[MigrationExport]
	public class M013_CreateTableMatrices13 : IMigration
	{
		public void Up(IDatabase db)
		{
			db.CreateTable("Matrices")
				.WithPrimaryKeyColumn("Id", DbType.Int64)
				.WithNullableColumn("Name", DbType.String).OfSize(255)
				.WithNullableColumn("Description", DbType.String).OfSize(255)
				.WithNullableColumn("AluminiumPanel", DbType.Int16)
				.WithNullableColumn("DoubleLayeredPanels", DbType.Int16)
				.WithNullableColumn("OversizedRoof", DbType.Int16)
				.WithNullableColumn("Maximum", DbType.Int16)
				.WithNullableColumn("CorrosionProtectionPart", DbType.Double)
                .WithNullableColumn("MaxCorrosionProtection", DbType.Double)
                .WithNullableColumn("CompanyFk", DbType.Int64);
				
            db.Tables["Prices"].AddForeignKeyTo("Matrices").Through("MatrixFk", "Id");
		}
	}
}
