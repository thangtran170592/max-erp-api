using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Barcodes_ContentHash",
                table: "Barcodes");

            migrationBuilder.DropIndex(
                name: "IX_Barcodes_OwnerType_OwnerId_IsPrimary",
                table: "Barcodes");

            migrationBuilder.DropIndex(
                name: "IX_Barcodes_OwnerType_OwnerId_Symbology_ContentHash",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "MetaJson",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Barcodes");

            migrationBuilder.RenameColumn(
                name: "Symbology",
                table: "Barcodes",
                newName: "Size");

            migrationBuilder.RenameColumn(
                name: "OwnerType",
                table: "Barcodes",
                newName: "BarcodeType");

            migrationBuilder.RenameColumn(
                name: "ContentHash",
                table: "Barcodes",
                newName: "Data");

            migrationBuilder.AddColumn<int>(
                name: "BarcodeForType",
                table: "Barcodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Barcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Barcodes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_BarcodeForType_UserId",
                table: "Barcodes",
                columns: new[] { "BarcodeForType", "UserId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Barcodes_BarcodeForType_UserId",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "BarcodeForType",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Barcodes");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "Barcodes",
                newName: "Symbology");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Barcodes",
                newName: "ContentHash");

            migrationBuilder.RenameColumn(
                name: "BarcodeType",
                table: "Barcodes",
                newName: "OwnerType");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "Barcodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "Barcodes",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaJson",
                table: "Barcodes",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Barcodes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_ContentHash",
                table: "Barcodes",
                column: "ContentHash");

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_OwnerType_OwnerId_IsPrimary",
                table: "Barcodes",
                columns: new[] { "OwnerType", "OwnerId", "IsPrimary" },
                unique: true,
                filter: "[IsPrimary] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_OwnerType_OwnerId_Symbology_ContentHash",
                table: "Barcodes",
                columns: new[] { "OwnerType", "OwnerId", "Symbology", "ContentHash" },
                unique: true);
        }
    }
}
