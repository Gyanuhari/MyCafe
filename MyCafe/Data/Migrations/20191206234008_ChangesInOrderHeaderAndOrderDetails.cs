using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MyCafe.Data.Migrations
{
    public partial class ChangesInOrderHeaderAndOrderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_CartMenuItems_CartMenuId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Coupons_CouponId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_CartMenuId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "MenuCount",
                table: "OrderHeaders");

            migrationBuilder.RenameColumn(
                name: "CouponId",
                table: "OrderDetails",
                newName: "MenuItemId");

            migrationBuilder.RenameColumn(
                name: "CartMenuId",
                table: "OrderDetails",
                newName: "MenuCount");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_CouponId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_MenuItemId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OrderHeaders",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PickupTime",
                table: "OrderHeaders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "OrderHeaders",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_UserId",
                table: "OrderHeaders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_MenuItems_MenuItemId",
                table: "OrderDetails",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_AspNetUsers_UserId",
                table: "OrderHeaders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_MenuItems_MenuItemId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_AspNetUsers_UserId",
                table: "OrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_UserId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "PickupTime",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OrderHeaders");

            migrationBuilder.RenameColumn(
                name: "MenuItemId",
                table: "OrderDetails",
                newName: "CouponId");

            migrationBuilder.RenameColumn(
                name: "MenuCount",
                table: "OrderDetails",
                newName: "CartMenuId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_MenuItemId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_CouponId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OrderHeaders",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "MenuCount",
                table: "OrderHeaders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_CartMenuId",
                table: "OrderDetails",
                column: "CartMenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_CartMenuItems_CartMenuId",
                table: "OrderDetails",
                column: "CartMenuId",
                principalTable: "CartMenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Coupons_CouponId",
                table: "OrderDetails",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
