using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ApprovalDocuments_ApprovalDocumentId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_ApprovalDocuments_ApprovalDocumentId",
                table: "Warehouses");

            migrationBuilder.DropTable(
                name: "ProductHistories");

            migrationBuilder.DropTable(
                name: "WarehouseHistories");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_ApprovalDocumentId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Products_ApprovalDocumentId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Id_ApprovalDocumentId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalDocuments_ApprovalFeatureId_DataId",
                table: "ApprovalDocuments");

            migrationBuilder.DropColumn(
                name: "ApprovalDocumentId",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "ApprovalDocumentId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ApprovalHistories",
                newName: "ApprovalStatus");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ApprovalDocuments",
                newName: "ApprovalStatus");

            migrationBuilder.RenameColumn(
                name: "DataId",
                table: "ApprovalDocuments",
                newName: "LatestApprovalHistoryId");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "ApprovalDocuments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LatestApprovalFeatureId",
                table: "ApprovalConfigs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalDocuments_ApprovalFeatureId_DocumentId",
                table: "ApprovalDocuments",
                columns: new[] { "ApprovalFeatureId", "DocumentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApprovalDocuments_ApprovalFeatureId_DocumentId",
                table: "ApprovalDocuments");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "ApprovalDocuments");

            migrationBuilder.DropColumn(
                name: "LatestApprovalFeatureId",
                table: "ApprovalConfigs");

            migrationBuilder.RenameColumn(
                name: "ApprovalStatus",
                table: "ApprovalHistories",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "LatestApprovalHistoryId",
                table: "ApprovalDocuments",
                newName: "DataId");

            migrationBuilder.RenameColumn(
                name: "ApprovalStatus",
                table: "ApprovalDocuments",
                newName: "Status");

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovalDocumentId",
                table: "Warehouses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovalDocumentId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Height = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReasonRejection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmittedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Uid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Width = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductHistories_PackageUnits_PackageUnitId",
                        column: x => x.PackageUnitId,
                        principalTable: "PackageUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductHistories_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductHistories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReasonRejection = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmittedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Uid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseHistories_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_ApprovalDocumentId",
                table: "Warehouses",
                column: "ApprovalDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ApprovalDocumentId",
                table: "Products",
                column: "ApprovalDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Id_ApprovalDocumentId",
                table: "Products",
                columns: new[] { "Id", "ApprovalDocumentId" },
                unique: true,
                filter: "[ApprovalDocumentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalDocuments_ApprovalFeatureId_DataId",
                table: "ApprovalDocuments",
                columns: new[] { "ApprovalFeatureId", "DataId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductHistories_CategoryId",
                table: "ProductHistories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductHistories_PackageUnitId",
                table: "ProductHistories",
                column: "PackageUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductHistories_ProductId",
                table: "ProductHistories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseHistories_WarehouseId",
                table: "WarehouseHistories",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ApprovalDocuments_ApprovalDocumentId",
                table: "Products",
                column: "ApprovalDocumentId",
                principalTable: "ApprovalDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_ApprovalDocuments_ApprovalDocumentId",
                table: "Warehouses",
                column: "ApprovalDocumentId",
                principalTable: "ApprovalDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
