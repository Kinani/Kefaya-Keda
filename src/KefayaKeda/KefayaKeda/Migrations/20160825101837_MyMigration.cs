using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KefayaKeda.Migrations
{
    public partial class MyMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KKProfiles",
                columns: table => new
                {
                    KKProfileId = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastEdit = table.Column<DateTime>(nullable: false),
                    TimeAllowance = table.Column<TimeSpan>(nullable: false),
                    TimeEllapsed = table.Column<TimeSpan>(nullable: false),
                    WhatAction = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KKProfiles", x => x.KKProfileId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KKProfiles");
        }
    }
}
