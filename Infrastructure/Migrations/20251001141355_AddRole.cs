using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationRole_Claims_AspNetRoles_RoleId",
                table: "ApplicationRole_Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationRoles_AspNetRoles_Id",
                table: "ApplicationRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUser_Roles_AspNetRoles_RoleId",
                table: "ApplicationUser_Roles");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "ApplicationRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ApplicationRoles",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "ApplicationRoles",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "ApplicationRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationRole_Claims_ApplicationRoles_RoleId",
                table: "ApplicationRole_Claims",
                column: "RoleId",
                principalTable: "ApplicationRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_Roles_ApplicationRoles_RoleId",
                table: "ApplicationUser_Roles",
                column: "RoleId",
                principalTable: "ApplicationRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationRole_Claims_ApplicationRoles_RoleId",
                table: "ApplicationRole_Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUser_Roles_ApplicationRoles_RoleId",
                table: "ApplicationUser_Roles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "ApplicationRoles");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "ApplicationRoles");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ApplicationRoles");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "ApplicationRoles");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationRole_Claims_AspNetRoles_RoleId",
                table: "ApplicationRole_Claims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationRoles_AspNetRoles_Id",
                table: "ApplicationRoles",
                column: "Id",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUser_Roles_AspNetRoles_RoleId",
                table: "ApplicationUser_Roles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
