using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M065_AddColumnToMatrixTable65 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.Tables["Matrices"].AddNullableColumn("OversizedDents", DbType.Double);
        }
    }
}
