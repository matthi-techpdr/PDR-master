using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M053_UpdateCustomersToMatrixesTable53 : IMigration
    {
        public void Up(IDatabase db)
        {
           // db.Tables["Customers_Matrices"].Rename("PriceMatrices_WholesaleCustomers");
        }
    }
}