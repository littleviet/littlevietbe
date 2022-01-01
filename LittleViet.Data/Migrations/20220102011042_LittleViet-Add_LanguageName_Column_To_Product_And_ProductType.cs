using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleViet.Data.Migrations
{
    public partial class LittleVietAdd_LanguageName_Column_To_Product_And_ProductType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "Payment",
                table: "Order",
                newName: "PaymentType");

            migrationBuilder.AddColumn<string>(
                name: "CAName",
                table: "ProductType",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductType",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ENName",
                table: "ProductType",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ProductType",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CAName",
                table: "Product",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ESName",
                table: "Product",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductTypeId",
                table: "Product",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CAName",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "ENName",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProductType");

            migrationBuilder.DropColumn(
                name: "CAName",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ESName",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ProductTypeId",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "PaymentType",
                table: "Order",
                newName: "Payment");

            migrationBuilder.AddColumn<int>(
                name: "ProductType",
                table: "Product",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
