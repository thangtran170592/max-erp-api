using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uid",
                table: "PackageUnits");

            migrationBuilder.AddColumn<Guid>(
                name: "PackageId",
                table: "PackageUnits",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PackageUnits_PackageId",
                table: "PackageUnits",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageUnits_Packages_PackageId",
                table: "PackageUnits",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageUnits_Packages_PackageId",
                table: "PackageUnits");

            migrationBuilder.DropIndex(
                name: "IX_PackageUnits_PackageId",
                table: "PackageUnits");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "PackageUnits");

            migrationBuilder.AddColumn<string>(
                name: "Uid",
                table: "PackageUnits",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
