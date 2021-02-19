using FluentMigrator;
using FluentMigrator.SqlServer;

namespace CTRL.Portal.API.CustomMigrations
{
    [Migration(6)]
    public class Version : Migration
    {
        public override void Down()
        {
            Create.Table("ResetCodes");
        }

        public override void Up()
        {
            Delete.Table("ResetCodes");
        }
    }
}
