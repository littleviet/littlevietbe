using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleViet.Data.Migrations
{
    public partial class Added_New_Coupon_Fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Coupon");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Coupon",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrentQuantity",
                table: "Coupon",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "InitialQuantity",
                table: "Coupon",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_AccountId",
                table: "Coupon",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupon_Account_AccountId",
                table: "Coupon",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupon_Account_AccountId",
                table: "Coupon");

            migrationBuilder.DropIndex(
                name: "IX_Coupon_AccountId",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "CurrentQuantity",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "InitialQuantity",
                table: "Coupon");

            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "Coupon",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
