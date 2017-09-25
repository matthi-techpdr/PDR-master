using System.Data;
using MigSharp;

namespace PDR.Database
{
	/// <summary>
	/// Create Estimate table
	/// </summary>
	[MigrationExport]
    public class M019_CreateTableEstimates19 : IMigration
	{
		public void Up(IDatabase db)
		{
            db.CreateTable("Estimates")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("CreationDate", DbType.DateTime)
                .WithNullableColumn("PriorDamages", DbType.String)
                .WithNullableColumn("EstimateStatus", DbType.Int16)
                .WithNullableColumn("Signature", DbType.Boolean)
                .WithNullableColumn("Archived", DbType.Boolean)
                .WithNullableColumn("CompanyFk", DbType.Int64)
                .WithNullableColumn("EmployeeFk", DbType.Int64)
                .WithNullableColumn("InsuranceFk", DbType.Int64)
                .WithNullableColumn("CarFk", DbType.Int64)
                .WithNullableColumn("CustomerFk", DbType.Int64)
                .WithNullableColumn("MatrixFk", DbType.Int64);

            db.Tables["Estimates"].AddForeignKeyTo("Users").Through("EmployeeFk", "Id");
            db.Tables["Estimates"].AddForeignKeyTo("Insurances").Through("InsuranceFk", "Id");
            db.Tables["Estimates"].AddForeignKeyTo("Cars").Through("CarFk", "Id");
            db.Tables["Estimates"].AddForeignKeyTo("Matrices").Through("MatrixFk", "Id");
            db.Tables["Estimates"].AddForeignKeyTo("Customers").Through("CustomerFk", "Id");

			db.Tables["Photos"].AddForeignKeyTo("Estimates").Through("EstimateFk", "Id");
			db.Tables["CarInspections"].AddForeignKeyTo("Estimates").Through("EstimateFk", "Id");			
		}
	}
}
