using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleViet.Data.Migrations
{
    public partial class Added_CouponTypeId_Non_Nullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupon_CouponType_CouponTypeId",
                table: "Coupon");

            migrationBuilder.AlterColumn<Guid>(
                name: "CouponTypeId",
                table: "Coupon",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Coupon_CouponType_CouponTypeId",
                table: "Coupon",
                column: "CouponTypeId",
                principalTable: "CouponType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupon_CouponType_CouponTypeId",
                table: "Coupon");

            migrationBuilder.AlterColumn<Guid>(
                name: "CouponTypeId",
                table: "Coupon",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupon_CouponType_CouponTypeId",
                table: "Coupon",
                column: "CouponTypeId",
                principalTable: "CouponType",
                principalColumn: "Id");
        }
    }
}
