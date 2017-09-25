using System.Data;
using MigSharp;

namespace PDR.Database
{
    [MigrationExport]
    public class M060_CreateCarPartsPriceTable60 : IMigration
    {
        public void Up(IDatabase db)
        {
            db.CreateTable("CarModels")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("Make", DbType.String).OfSize(255)
                .WithNullableColumn("Model", DbType.String).OfSize(255)
                .WithNullableColumn("YearFrom", DbType.Int32)
                .WithNullableColumn("CompanyFk", DbType.Int64);

            db.CreateTable("CarSectionsPrices")
                .WithPrimaryKeyColumn("Id", DbType.Int64)
                .WithNullableColumn("Name", DbType.Int32)
                .WithNullableColumn("NewSectionPrice", DbType.Double)
                .WithNullableColumn("CarModelFk", DbType.Int64)
                .WithNullableColumn("CompanyFk", DbType.Int64);

            db.Tables["EffortItems"]
                .AddNullableColumn("Name", DbType.String).OfSize(255)
				.AddNullableColumn("HoursR_R", DbType.Double)
                .AddNullableColumn("HoursR_I", DbType.Double)
                .AddNullableColumn("CarSectionsPricesFk", DbType.Int64);
        }
    }
}
