using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleViet.Domain.Migrations
{
    public partial class PriceToDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Serving",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.UpdateData(
                table: "ProductType",
                keyColumn: "Id",
                keyValue: new Guid("96c9d97c-1207-46a6-bb25-8773b622d205"),
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2022, 7, 31, 11, 48, 17, 272, DateTimeKind.Utc).AddTicks(5221), new DateTime(2022, 7, 31, 11, 48, 17, 272, DateTimeKind.Utc).AddTicks(5223) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Serving",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.UpdateData(
                table: "ProductType",
                keyColumn: "Id",
                keyValue: new Guid("96c9d97c-1207-46a6-bb25-8773b622d205"),
                columns: new[] { "CreatedDate", "UpdatedDate" },
                values: new object[] { new DateTime(2022, 7, 30, 11, 48, 26, 633, DateTimeKind.Utc).AddTicks(3340), new DateTime(2022, 7, 30, 11, 48, 26, 633, DateTimeKind.Utc).AddTicks(3342) });
        }
    }
}
