using System.Data;

using MigSharp;

namespace PDR.Database
{
	[MigrationExport]
    public class M025_UpdateRelationBitweenEstimateAndEnsurance25 : IMigration
	{
		public void Up(IDatabase db)
		{
            //db.Tables["Estimates"].ForeignKeys["FK_Estimates_Insurances"].Drop();
            //db.Tables["Estimates"].Columns["InsuranceFk"].Drop();

		    db.Tables["Insurances"].AddNullableColumn("EstimateFk", DbType.Int64);
            db.Tables["Insurances"].AddForeignKeyTo("Estimates", "FK_Insurance_Estimate").Through("EstimateFk", "Id");
		}
	}
}
