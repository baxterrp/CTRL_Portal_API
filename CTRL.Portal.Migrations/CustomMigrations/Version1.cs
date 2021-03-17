using CTRL.Portal.Migrations.Metadata;
using FluentMigrator;
using FluentMigrator.SqlServer;

namespace CTRL.Portal.Migrations.Custom
{
    [Migration(1)]
    public class Version1 : Migration
    {
        public override void Down()
        {
            Delete.Table(Names.UserAccountsTable);
            Delete.Table(Names.AccountsTable);
        }

        public override void Up()
        {
            Create.Table(Names.AccountsTable)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey()
                .WithColumn("Name").AsString().NotNullable();

            Create.Table(Names.UserAccountsTable)
                .WithColumn("Id").AsInt32().PrimaryKey().Identity(1, 1)
                .WithColumn("AccountId").AsString().NotNullable().ForeignKey()
                .WithColumn("UserName").AsString().NotNullable();

            Create.ForeignKey()
                .FromTable(Names.UserAccountsTable).ForeignColumn("AccountId")
                .ToTable("Accounts").PrimaryColumn("Id");
        }
    }
}
