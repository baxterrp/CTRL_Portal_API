using CTRL.Portal.Migrations.Metadata;
using FluentMigrator;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(7)]
    public class Version7 : Migration
    {
        public override void Down()
        {
            Delete.Table(Names.SubscriptionsTable);
        }

        public override void Up()
        {
            Create.Table(Names.SubscriptionsTable)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey()
                .WithColumn("AccountId").AsString().NotNullable().ForeignKey()
                .WithColumn("Name").AsString();

            Create.ForeignKey()
                .FromTable(Names.SubscriptionsTable).ForeignColumn("AccountId")
                .ToTable("Accounts").PrimaryColumn("Id");
        }
    }
}
