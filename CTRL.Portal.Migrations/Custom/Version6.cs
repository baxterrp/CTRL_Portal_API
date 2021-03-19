﻿using FluentMigrator;

namespace CTRL.Portal.Migrations.Custom
{
    [Migration(6)]
    public class Version6 : Migration
    {
        public override void Down()
        {
            Delete.Table("AccountCode");
        }

        public override void Up()
        {
            Create.Table("AccountCodes")
                .WithColumn("Id").AsString().NotNullable().PrimaryKey()
                .WithColumn("AccountId").AsString().NotNullable().ForeignKey()
                .WithColumn("CodeId").AsString().NotNullable().ForeignKey()
                .WithColumn("Accepted").AsBoolean().NotNullable().WithDefaultValue("false");

            Create.ForeignKey()
                .FromTable("AccountCodes").ForeignColumn("AccountId")
                .ToTable("Accounts").PrimaryColumn("Id");

            Create.ForeignKey()
                .FromTable("AccountCodes").ForeignColumn("CodeId")
                .ToTable("Codes").PrimaryColumns("Id");
        }
    }
}

