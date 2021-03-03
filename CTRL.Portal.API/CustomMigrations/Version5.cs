using CTRL.Portal.API.APIConstants;
using FluentMigrator;
using System;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(5)]
    public class Version5 : Migration
    {
        public override void Down()
        {
            Delete.Table(ApiNames.ModulesTable);
        }

        public override void Up()
        {
            Create.Table(ApiNames.ModulesTable)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey()
                .WithColumn("Name").AsString().NotNullable();

            Insert.IntoTable("Modules").Row(
                new
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = ApiNames.InventoryModule
                });
        }
    }
}
