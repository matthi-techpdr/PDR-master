using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M054_RenameCustomersToMatrixesColumn54 : IMigration
    {
        public void Up(IDatabase db)
        {
            //db.Tables["PriceMatrices_WholesaleCustomers"].Columns["MatrixFk"].Rename("PriceMatrixFk");
            //db.Tables["PriceMatrices_WholesaleCustomers"].Columns["CustomerFk"].Rename("WholesaleCustomerFk");
        }
    }
}