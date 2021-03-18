using CTRL.Portal.Migrations.Metadata;
using FluentMigrator;

namespace CTRL.Portal.Migrations.Custom
{
    [Migration(4)]
    public class Version4 : Migration
    {
        public override void Down()
        {
            Delete.Table(Names.CodesTable);
        }

        public override void Up()
        {
            Create.Table(Names.CodesTable)
                .WithColumn("Id").AsString().NotNullable().PrimaryKey()
                .WithColumn("Email").AsString().NotNullable()
                .WithColumn("Expiration").AsDateTime().NotNullable()
                .WithColumn("Code").AsString(6);
        }
    }
}
