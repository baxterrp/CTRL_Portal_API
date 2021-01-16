using FluentMigrator;
using FluentMigrator.SqlServer;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(01152021804)]
    public class Version2 : Migration
    {
        public override void Down()
        {
            Delete.Table("UserSettings");
        }

        public override void Up()
        {
            Create.Table("UserSettings")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity(1, 1)
                .WithColumn("Theme").AsString().Nullable()
                .WithColumn("UserName").AsString().NotNullable();
        }
    }
}
