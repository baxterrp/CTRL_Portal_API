using CTRL.Portal.API.APIConstants;
using FluentMigrator;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(7)]
    public class Version7 : Migration
    {
        public override void Down()
        {
            Delete.Table(ApiNames.SubscriptionsTable);
        }

        public override void Up()
        {
            Create.Table(ApiNames.SubscriptionsTable)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey()
                .WithColumn("AccountId").AsString().NotNullable().ForeignKey()
                .WithColumn("Name").AsString();

            Create.ForeignKey()
                .FromTable(ApiNames.SubscriptionsTable).ForeignColumn("AccountId")
                .ToTable("Accounts").PrimaryColumn("Id");
        }
    }
}
