﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace LibraryData.Migrations
{
    public partial class Book_Entity_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "LibraryAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeweyIndex",
                table: "LibraryAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ISBN",
                table: "LibraryAssets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "LibraryAssets");

            migrationBuilder.DropColumn(
                name: "DeweyIndex",
                table: "LibraryAssets");

            migrationBuilder.DropColumn(
                name: "ISBN",
                table: "LibraryAssets");
        }
    }
}
