using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LittleViet.Data.Migrations
{
    public partial class LittleVietChange_ESNAME_CSName_Column_To_EsName_CsName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ESName",
                table: "ProductType",
                newName: "EsName");

            migrationBuilder.RenameColumn(
                name: "CAName",
                table: "ProductType",
                newName: "CaName");

            migrationBuilder.RenameColumn(
                name: "ESName",
                table: "Product",
                newName: "EsName");

            migrationBuilder.RenameColumn(
                name: "CAName",
                table: "Product",
                newName: "CaName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EsName",
                table: "ProductType",
                newName: "ESName");

            migrationBuilder.RenameColumn(
                name: "CaName",
                table: "ProductType",
                newName: "CAName");

            migrationBuilder.RenameColumn(
                name: "EsName",
                table: "Product",
                newName: "ESName");

            migrationBuilder.RenameColumn(
                name: "CaName",
                table: "Product",
                newName: "CAName");
        }
    }
}
