using System.Data;

using MigSharp;

namespace PDR.Database
{
	[MigrationExport]
	public class M004_CreateTableCars4 : IMigration
	{
		public void Up(IDatabase db)
		{
			db.CreateTable("Cars")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNotNullableColumn("VIN", DbType.String).OfSize(255)
                .WithNotNullableColumn("Model", DbType.String).OfSize(255)
                .WithNotNullableColumn("Make", DbType.String).OfSize(255)
                .WithNotNullableColumn("Year", DbType.Int32)
                .WithNullableColumn("Trim", DbType.String).OfSize(255)
                .WithNotNullableColumn("Mileage", DbType.Int32)
                .WithNullableColumn("Color", DbType.String).OfSize(255)
                .WithNullableColumn("LicensePlate", DbType.String).OfSize(255)
                .WithNullableColumn("State", DbType.Int16)
                .WithNullableColumn("CustRO", DbType.String).OfSize(255)
                .WithNullableColumn("CompanyFk", DbType.Int64)
                .WithNullableColumn("Stock", DbType.String).OfSize(255);
		}
	}
}
