using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleViet.Data.Migrations
{
    public partial class Add_Stripe_Properties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "OrderDetail");

            migrationBuilder.AddColumn<string>(
                name: "StripePriceId",
                table: "Serving",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeProductId",
                table: "Product",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Quantity",
                table: "OrderDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "OrderStatus",
                table: "Order",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripePriceId",
                table: "Serving");

            migrationBuilder.DropColumn(
                name: "StripeProductId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "Order");

            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "OrderDetail",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
