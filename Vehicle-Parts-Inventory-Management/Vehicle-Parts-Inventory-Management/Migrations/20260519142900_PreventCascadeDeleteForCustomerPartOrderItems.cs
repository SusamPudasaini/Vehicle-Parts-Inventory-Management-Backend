using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Parts_Inventory_Management.Migrations
{
    /// <inheritdoc />
    public partial class PreventCascadeDeleteForCustomerPartOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPartOrderItems_Parts_PartId",
                table: "CustomerPartOrderItems");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPartOrderItems_Parts_PartId",
                table: "CustomerPartOrderItems",
                column: "PartId",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPartOrderItems_Parts_PartId",
                table: "CustomerPartOrderItems");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPartOrderItems_Parts_PartId",
                table: "CustomerPartOrderItems",
                column: "PartId",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
