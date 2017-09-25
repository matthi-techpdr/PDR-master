using System.Data;

using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M052_UpdateMatrixTable52 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Matrices"].AddNullableColumn("Class", DbType.String).OfSize(255);
        } 
    }
}