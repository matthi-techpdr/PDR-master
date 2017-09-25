using System.Data;
using MigSharp;

namespace PDR.Database
{
	[MigrationExport]
    public class M018_CreateTablePhotos18 : IMigration
	{
		public void Up(IDatabase db)
		{
			db.CreateTable("Photos")
				.WithPrimaryKeyColumn("Id", DbType.Int64)
				.WithNullableColumn("Name", DbType.String).OfSize(255)
				.WithNullableColumn("PhotoFull", DbType.Binary)
				.WithNullableColumn("PhotoThumbnail", DbType.Binary)
				.WithNullableColumn("EstimateFk", DbType.Int64)
                .WithNullableColumn("CompanyFk", DbType.Int64);
		}
	}
}
