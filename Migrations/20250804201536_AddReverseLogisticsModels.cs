using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace irevlogix_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddReverseLogisticsModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualPickupDate",
                table: "Shipments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientContactId",
                table: "Shipments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DispositionCost",
                table: "Shipments",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DispositionNotes",
                table: "Shipments",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LogisticsCost",
                table: "Shipments",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginAddress",
                table: "Shipments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OriginatorClientId",
                table: "Shipments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledPickupDate",
                table: "Shipments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TransportationCost",
                table: "Shipments",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssetCategoryId",
                table: "ShipmentItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DispositionCost",
                table: "ShipmentItems",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DispositionMethod",
                table: "ShipmentItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAssetRecoverable",
                table: "ShipmentItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaterialTypeId",
                table: "ShipmentItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProcessingLotId",
                table: "ShipmentItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasure",
                table: "ShipmentItems",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssetCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ParentCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RequiresDataDestruction = table.Column<bool>(type: "boolean", nullable: false),
                    IsRecoverable = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientContacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    JobTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsPrimaryContact = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientContacts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RecyclingMethod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingLots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LotNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IncomingMaterialNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ContaminationPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    ActualReceivedWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    WeightUnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SourceShipmentId = table.Column<int>(type: "integer", nullable: true),
                    ProcessingMethod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProcessingCost = table.Column<decimal>(type: "numeric", nullable: true),
                    ProcessingNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessingLots_Shipments_SourceShipmentId",
                        column: x => x.SourceShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ClientContactId",
                table: "Shipments",
                column: "ClientContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_OriginatorClientId",
                table: "Shipments",
                column: "OriginatorClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentItems_AssetCategoryId",
                table: "ShipmentItems",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentItems_MaterialTypeId",
                table: "ShipmentItems",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentItems_ProcessingLotId",
                table: "ShipmentItems",
                column: "ProcessingLotId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_Name_ClientId",
                table: "AssetCategories",
                columns: new[] { "Name", "ClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientContacts_ClientId",
                table: "ClientContacts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientContacts_Email_ClientId",
                table: "ClientContacts",
                columns: new[] { "Email", "ClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTypes_Name_ClientId",
                table: "MaterialTypes",
                columns: new[] { "Name", "ClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingLots_LotNumber_ClientId",
                table: "ProcessingLots",
                columns: new[] { "LotNumber", "ClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingLots_SourceShipmentId",
                table: "ProcessingLots",
                column: "SourceShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentItems_AssetCategories_AssetCategoryId",
                table: "ShipmentItems",
                column: "AssetCategoryId",
                principalTable: "AssetCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentItems_MaterialTypes_MaterialTypeId",
                table: "ShipmentItems",
                column: "MaterialTypeId",
                principalTable: "MaterialTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentItems_ProcessingLots_ProcessingLotId",
                table: "ShipmentItems",
                column: "ProcessingLotId",
                principalTable: "ProcessingLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_ClientContacts_ClientContactId",
                table: "Shipments",
                column: "ClientContactId",
                principalTable: "ClientContacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Clients_OriginatorClientId",
                table: "Shipments",
                column: "OriginatorClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentItems_AssetCategories_AssetCategoryId",
                table: "ShipmentItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentItems_MaterialTypes_MaterialTypeId",
                table: "ShipmentItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentItems_ProcessingLots_ProcessingLotId",
                table: "ShipmentItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_ClientContacts_ClientContactId",
                table: "Shipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Clients_OriginatorClientId",
                table: "Shipments");

            migrationBuilder.DropTable(
                name: "AssetCategories");

            migrationBuilder.DropTable(
                name: "ClientContacts");

            migrationBuilder.DropTable(
                name: "MaterialTypes");

            migrationBuilder.DropTable(
                name: "ProcessingLots");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_ClientContactId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_OriginatorClientId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentItems_AssetCategoryId",
                table: "ShipmentItems");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentItems_MaterialTypeId",
                table: "ShipmentItems");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentItems_ProcessingLotId",
                table: "ShipmentItems");

            migrationBuilder.DropColumn(
                name: "ActualPickupDate",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ClientContactId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DispositionCost",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DispositionNotes",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "LogisticsCost",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "OriginAddress",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "OriginatorClientId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ScheduledPickupDate",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "TransportationCost",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "AssetCategoryId",
                table: "ShipmentItems");

            migrationBuilder.DropColumn(
                name: "DispositionCost",
                table: "ShipmentItems");

            migrationBuilder.DropColumn(
                name: "DispositionMethod",
                table: "ShipmentItems");

            migrationBuilder.DropColumn(
                name: "IsAssetRecoverable",
                table: "ShipmentItems");

            migrationBuilder.DropColumn(
                name: "MaterialTypeId",
                table: "ShipmentItems");

            migrationBuilder.DropColumn(
                name: "ProcessingLotId",
                table: "ShipmentItems");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "ShipmentItems");
        }
    }
}
