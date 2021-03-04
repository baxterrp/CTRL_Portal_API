using CTRL.Portal.API.APIConstants;
using FluentMigrator;
using FluentMigrator.SqlServer;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(1)]
    public class Version1 : Migration
    {
        public override void Down()
        {
            Delete.Table(ApiNames.UserAccountsTable);
            Delete.Table(ApiNames.AccountsTable);
        }

        public override void Up()
        {
            Create.Table(ApiNames.AccountsTable)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey()
                .WithColumn("Name").AsString().NotNullable();

            Create.Table(ApiNames.UserAccountsTable)
                .WithColumn("Id").AsInt32().PrimaryKey().Identity(1, 1)
                .WithColumn("AccountId").AsString().NotNullable().ForeignKey()
                .WithColumn("UserName").AsString().NotNullable();

            Create.ForeignKey()
                .FromTable(ApiNames.UserAccountsTable).ForeignColumn("AccountId")
                .ToTable("Accounts").PrimaryColumn("Id");
        }
    }
}
