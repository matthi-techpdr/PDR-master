using System.Data;

using MigSharp;

namespace PDR.Database
{
	[MigrationExport]
	public class M023_UpdateCustomerTable23 : IMigration
	{
		public void Up(IDatabase db)
		{
			db.Tables["Customers"]
                .AddNullableColumn("CreatingDate", DbType.DateTime)
                .AddNullableColumn("Status", DbType.Int32);
		}
	}
}
