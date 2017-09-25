using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    class M108_AddColumnCanCreateEstimatesToUsersTable108: IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Users"].AddNullableColumn("CanCreateEstimates", DbType.Boolean);
        }
    }
}
