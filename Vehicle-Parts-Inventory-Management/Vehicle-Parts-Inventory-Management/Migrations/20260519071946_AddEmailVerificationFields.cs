using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Parts_Inventory_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerificationTokenExpiresUtc",
                table: "Staff",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationTokenHash",
                table: "Staff",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "Staff",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerificationTokenExpiresUtc",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationTokenHash",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "Customers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailVerificationTokenExpiresUtc",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "EmailVerificationTokenHash",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "Staff");

            migrationBuilder.DropColumn(
                name: "EmailVerificationTokenExpiresUtc",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailVerificationTokenHash",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "Customers");
        }
    }
}
