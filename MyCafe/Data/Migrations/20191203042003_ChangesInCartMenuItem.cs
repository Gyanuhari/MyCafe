using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MyCafe.Data.Migrations
{
    public partial class ChangesInCartMenuItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartMenuItems_MenuItems_MenuItemId",
                table: "CartMenuItems");

            migrationBuilder.DropIndex(
                name: "IX_CartMenuItems_MenuItemId",
                table: "CartMenuItems");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "CartMenuItems",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "CartMenuItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartMenuItems_MenuItemId",
                table: "CartMenuItems",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartMenuItems_MenuItems_MenuItemId",
                table: "CartMenuItems",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
