using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleViet.Data.Migrations
{
    public partial class Remove_OrderDetails_Price : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderDetail");

            migrationBuilder.AddColumn<string>(
                name: "LastStripeSessionId",
                table: "Order",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastStripeSessionId",
                table: "Order");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "OrderDetail",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
