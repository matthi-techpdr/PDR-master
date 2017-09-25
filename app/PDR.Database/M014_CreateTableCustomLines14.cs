using System.Data;

using MigSharp;

namespace PDR.Database
{
	[MigrationExport]
    public class M014_CreateTableCustomLines14 : IMigration
	{
		public void Up(IDatabase db)
		{
			db.CreateTable("CustomLines")
				.WithPrimaryKeyColumn("Id", DbType.Int64)
				.WithNullableColumn("Class", DbType.String).OfSize(255)
				.WithNullableColumn("Name", DbType.String).OfSize(255)
				.WithNullableColumn("Cost", DbType.Double)
				.WithNullableColumn("EstimateFk", DbType.Int64)
                .WithNullableColumn("CompanyFk", DbType.Int64)
				.WithNullableColumn("CarInspectionFk", DbType.Int64);
		}
	}
}
