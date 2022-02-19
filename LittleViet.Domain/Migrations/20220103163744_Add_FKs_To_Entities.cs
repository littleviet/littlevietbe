using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleViet.Data.Migrations
{
    public partial class Add_FKs_To_Entities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Serving_ProductId",
                table: "Serving",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_AccountId",
                table: "Reservation",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderId",
                table: "OrderDetail",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ServingId",
                table: "OrderDetail",
                column: "ServingId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Order_OrderId",
                table: "OrderDetail",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Serving_ServingId",
                table: "OrderDetail",
                column: "ServingId",
                principalTable: "Serving",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImage_Product_ProductId",
                table: "ProductImage",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Account_AccountId",
                table: "Reservation",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Serving_Product_ProductId",
                table: "Serving",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Order_OrderId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Serving_ServingId",
                table: "OrderDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductImage_Product_ProductId",
                table: "ProductImage");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Account_AccountId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Serving_Product_ProductId",
                table: "Serving");

            migrationBuilder.DropIndex(
                name: "IX_Serving_ProductId",
                table: "Serving");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_AccountId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_OrderId",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_ServingId",
                table: "OrderDetail");
        }
    }
}
