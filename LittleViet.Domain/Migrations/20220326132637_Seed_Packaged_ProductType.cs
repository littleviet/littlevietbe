using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleViet.Data.Migrations
{
    public partial class Seed_Packaged_ProductType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductType",
                columns: new[] { "Id", "CaName", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "Description", "EsName", "IsDeleted", "Name", "UpdatedBy", "UpdatedDate" },
                values: new object[] { new Guid("96c9d97c-1207-46a6-bb25-8773b622d205"), "Productes envasats", null, new DateTime(2022, 3, 26, 13, 26, 36, 814, DateTimeKind.Utc).AddTicks(4455), null, null, "Packaged products", "Productos Empaquetados", false, "Packaged Products", null, new DateTime(2022, 3, 26, 13, 26, 36, 814, DateTimeKind.Utc).AddTicks(4458) });

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_CouponCode",
                table: "Coupon",
                column: "CouponCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Coupon_CouponCode",
                table: "Coupon");

            migrationBuilder.DeleteData(
                table: "ProductType",
                keyColumn: "Id",
                keyValue: new Guid("96c9d97c-1207-46a6-bb25-8773b622d205"));
        }
    }
}
