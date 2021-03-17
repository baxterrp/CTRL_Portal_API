using CTRL.Portal.Migrations.Metadata;
using FluentMigrator;
using System;

namespace CTRL.Portal.Migrations.Custom
{
    [Migration(5)]
    public class Version5 : Migration
    {
        public override void Down()
        {
            Delete.Table(Names.ModulesTable);
        }

        public override void Up()
        {
            Create.Table(Names.ModulesTable)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey()
                .WithColumn("Name").AsString().NotNullable();

            Insert.IntoTable("Modules").Row(
                new
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Names.InventoryModule
                });
        }
    }
}
