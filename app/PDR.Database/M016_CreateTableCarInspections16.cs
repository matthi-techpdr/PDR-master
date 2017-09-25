using System.Data;

using MigSharp;

namespace PDR.Database
{
	[MigrationExport]
    public class M016_CreateTableCarInspections16 : IMigration
	{
		public void Up(IDatabase db)
		{
			db.CreateTable("CarInspections")
				.WithPrimaryKeyColumn("Id", DbType.Int64)
				.WithNullableColumn("Name", DbType.Int16)
				.WithNullableColumn("DentsAmount", DbType.Int16)
				.WithNullableColumn("AverageSize", DbType.Int16)
				.WithNullableColumn("Aluminium", DbType.Boolean)
				.WithNullableColumn("DoubleMetal", DbType.Boolean)
				.WithNullableColumn("CorrosionProtection", DbType.Boolean)
                .WithNullableColumn("CompanyFk", DbType.Int64)
                .WithNullableColumn("EstimateFk", DbType.Int64);

 			db.Tables["CustomLines"].AddForeignKeyTo("CarInspections").Through("CarInspectionFk", "Id");
		}
	}
}
