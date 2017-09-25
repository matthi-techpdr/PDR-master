using MigSharp;

namespace PDR.Database
{
    /// <summary>
    /// Fake migration from Max Dumchikov (duplicate column 'Phone2').
    /// </summary>
    [MigrationExport]
    public class M028_AddColumnToCustomersTable28 : IMigration
    {
        public void Up(IDatabase db)
        {
            // duplicate column 'Phone2'
            // db.Tables["Customers"].AddNullableColumn("Phone2", DbType.String).OfSize(255);
        }
    }
}
