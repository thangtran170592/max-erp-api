using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_Departments_DepartmentId",
                table: "ApplicationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_Positions_PositionId",
                table: "ApplicationUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetValue",
                table: "ApprovalFeatures",
                type: "uniqueidentifier",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_Departments_DepartmentId",
                table: "ApplicationUsers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_Positions_PositionId",
                table: "ApplicationUsers",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_Departments_DepartmentId",
                table: "ApplicationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_Positions_PositionId",
                table: "ApplicationUsers");

            migrationBuilder.AlterColumn<string>(
                name: "TargetValue",
                table: "ApprovalFeatures",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldMaxLength: 255);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_Departments_DepartmentId",
                table: "ApplicationUsers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_Positions_PositionId",
                table: "ApplicationUsers",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id");
        }
    }
}
