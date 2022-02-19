using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleViet.Data.Migrations
{
    public partial class LittleVietChange_Status_Type_Int_To_Enum_And_Add_ProductPrice_Column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Serving");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Order");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Product",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Product");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Serving",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ProductType",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Order",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
