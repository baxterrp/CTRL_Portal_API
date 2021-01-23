using FluentMigrator;
using FluentMigrator.SqlServer;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(1)]
    public class Version1 : Migration
    {
        public override void Down()
        {
            Delete.Table("UserAccounts");
            Delete.Table("accounts");
        }

        public override void Up()
        {
            Create.Table("Accounts")
                .WithColumn("Id").AsString().NotNullable().PrimaryKey()
                .WithColumn("Name").AsString().NotNullable();

            Create.Table("UserAccounts")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity(1, 1)
                .WithColumn("AccountId").AsString().NotNullable().ForeignKey()
                .WithColumn("UserName").AsString().NotNullable();

            Create.ForeignKey()
                .FromTable("UserAccounts").ForeignColumn("AccountId")
                .ToTable("Accounts").PrimaryColumn("Id");
        }
    }
}
