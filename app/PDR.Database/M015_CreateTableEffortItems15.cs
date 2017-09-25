using System.Data;

using MigSharp;

namespace PDR.Database
{
	[MigrationExport]
    public class M015_CreateTableEffortItems15 : IMigration
	{
		public void Up(IDatabase db)
		{
		    db.CreateTable("EffortItems").WithPrimaryKeyColumn("Id", DbType.Int64);
            db.Tables["EffortItems"].AddNullableColumn("CompanyFk", DbType.Int64);
		}
	}
}
