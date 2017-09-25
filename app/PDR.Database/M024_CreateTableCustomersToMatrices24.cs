using System.Data;

using MigSharp;

namespace PDR.Database
{
	[MigrationExport]
    public class M024_CreateTableCustomersToMatrices24 : IMigration
	{
		public void Up(IDatabase db)
		{
            db.CreateTable("PriceMatrices_WholesaleCustome")
                .WithNullableColumn("WholesaleCustomerFk", DbType.Int64)
                .WithNullableColumn("PriceMatrixFk", DbType.Int64);

            db.Tables["PriceMatrices_WholesaleCustome"].AddForeignKeyTo("Customers", "Fk_Cust_Matr").Through("WholesaleCustomerFk", "Id");
            db.Tables["PriceMatrices_WholesaleCustome"].AddForeignKeyTo("Matrices", "Fk_Matr_Cust").Through("PriceMatrixFk", "Id");
		}
	}
}
