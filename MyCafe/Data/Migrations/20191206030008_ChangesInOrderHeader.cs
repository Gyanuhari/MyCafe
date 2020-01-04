using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MyCafe.Data.Migrations
{
    public partial class ChangesInOrderHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "OrderHeaders",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OrderTotal",
                table: "OrderHeaders",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "OrderTotal",
                table: "OrderHeaders");
        }
    }
}
