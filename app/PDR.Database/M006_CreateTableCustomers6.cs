using System.Data;

using MigSharp;

namespace PDR.Database
{
	/// <summary>
	/// Create Customer table
	/// </summary>
	[MigrationExport]
	public class M006_CreateTableCustomers6 : IMigration
	{
		public void Up(IDatabase db)
		{
			db.CreateTable("Customers")
				.WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("Class", DbType.String).OfSize(255)
                .WithNullableColumn("FirstName", DbType.String).OfSize(255)
                .WithNullableColumn("LastName", DbType.String).OfSize(255)
                .WithNullableColumn("Address1", DbType.String).OfSize(255)
                .WithNullableColumn("Address2", DbType.String).OfSize(255)
                .WithNullableColumn("Phone", DbType.String).OfSize(255)
                .WithNullableColumn("Phone2", DbType.String).OfSize(255)
                .WithNullableColumn("Fax", DbType.String).OfSize(255)
				.WithNullableColumn("State", DbType.Int16)
                .WithNullableColumn("Zip", DbType.String).OfSize(255)
                .WithNullableColumn("CompanyFk", DbType.Int64)
                .WithNullableColumn("Email", DbType.String).OfSize(255)
                .WithNullableColumn("City", DbType.String).OfSize(255);
		}
	}
}
