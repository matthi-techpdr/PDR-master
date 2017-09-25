using System.Data;

using MigSharp;

namespace PDR.Database
{
	[MigrationExport]
    public class M017_CreateTableChosenEffortItems17 : IMigration
	{
		public void Up(IDatabase db)
		{
			db.CreateTable("ChosenEffortItems")
				.WithPrimaryKeyColumn("Id", DbType.Int64)
				.WithNullableColumn("Choosed", DbType.Boolean)
                .WithNullableColumn("EffortItemFk", DbType.Int64)
				.WithNullableColumn("CarInspectionFK", DbType.Int64)
                .WithNullableColumn("CompanyFk", DbType.Int64);

            db.Tables["ChosenEffortItems"].AddForeignKeyTo("CarInspections", "Fk_CEI_CI").Through("CarInspectionFk", "Id");
            db.Tables["ChosenEffortItems"].AddForeignKeyTo("EffortItems", "FkCEI_EI").Through("EffortItemFk", "Id");
		}
	}
}
