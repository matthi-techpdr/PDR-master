using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M092_UpdateCompanyTable92 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Companies"].AddNullableColumn("DefaultVehicleFk", DbType.Int64);
        }
    }
}