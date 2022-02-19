using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleViet.Data.Migrations
{
    public partial class Added_CouponType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CouponTypeId",
                table: "Coupon",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CouponType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    StripeProductId = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_CouponTypeId",
                table: "Coupon",
                column: "CouponTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupon_CouponType_CouponTypeId",
                table: "Coupon",
                column: "CouponTypeId",
                principalTable: "CouponType",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupon_CouponType_CouponTypeId",
                table: "Coupon");

            migrationBuilder.DropTable(
                name: "CouponType");

            migrationBuilder.DropIndex(
                name: "IX_Coupon_CouponTypeId",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "CouponTypeId",
                table: "Coupon");
        }
    }
}
