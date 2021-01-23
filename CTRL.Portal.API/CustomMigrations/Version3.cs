using FluentMigrator;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(3)]
    public class Version3 : Migration
    {
        public override void Down()
        {
            Delete.Column("DefaultAccount")
                .FromTable("UserSettings");
        }

        public override void Up()
        {
            Alter.Table("UserSettings")
                .AddColumn("DefaultAccount").AsString().Nullable();
        }
    }
}
