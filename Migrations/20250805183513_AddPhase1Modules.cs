using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace irevlogix_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPhase1Modules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientContacts_Clients_ClientId",
                table: "ClientContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcessingLots_Shipments_SourceShipmentId",
                table: "ProcessingLots");

            migrationBuilder.DropIndex(
                name: "IX_ProcessingLots_LotNumber_ClientId",
                table: "ProcessingLots");

            migrationBuilder.DropIndex(
                name: "IX_ClientContacts_ClientId",
                table: "ClientContacts");

            migrationBuilder.DropColumn(
                name: "LotNumber",
                table: "ProcessingLots");

            migrationBuilder.DropColumn(
                name: "WeightUnit",
                table: "ProcessingLots");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "MaterialTypes");

            migrationBuilder.DropColumn(
                name: "RecyclingMethod",
                table: "MaterialTypes");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "MaterialTypes");

            migrationBuilder.RenameColumn(
                name: "SourceShipmentId",
                table: "ProcessingLots",
                newName: "OperatorUserId");

            migrationBuilder.RenameColumn(
                name: "ProcessingNotes",
                table: "ProcessingLots",
                newName: "QualityControlNotes");

            migrationBuilder.RenameColumn(
                name: "ProcessingMethod",
                table: "ProcessingLots",
                newName: "CertificationStatus");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "ProcessingLots",
                newName: "CompletionDate");

            migrationBuilder.RenameColumn(
                name: "ActualReceivedWeight",
                table: "ProcessingLots",
                newName: "TotalProcessedWeight");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessingLots_SourceShipmentId",
                table: "ProcessingLots",
                newName: "IX_ProcessingLots_OperatorUserId");

            migrationBuilder.AddColumn<int>(
                name: "ClientContactId1",
                table: "Shipments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualRevenue",
                table: "ProcessingLots",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificationNumber",
                table: "ProcessingLots",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProcessingLots",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedRevenue",
                table: "ProcessingLots",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IncomingMaterialCost",
                table: "ProcessingLots",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LotID",
                table: "ProcessingLots",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalIncomingWeight",
                table: "ProcessingLots",
                type: "numeric",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MaterialTypes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultPricePerUnit",
                table: "MaterialTypes",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "MaterialTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ClientContacts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ClientContacts",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ClientId",
                table: "ClientContacts",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "DefaultDisposition",
                table: "AssetCategories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresDataSanitization",
                table: "AssetCategories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ApplicationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SettingKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SettingValue = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsEncrypted = table.Column<bool>(type: "boolean", nullable: false),
                    IsReadOnly = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetTrackingStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StatusName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    ColorCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTrackingStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeBaseArticles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    AuthorUserId = table.Column<int>(type: "integer", nullable: true),
                    PublishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Summary = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeBaseArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeBaseArticles_Users_AuthorUserId",
                        column: x => x.AuthorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcessedMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcessingLotId = table.Column<int>(type: "integer", nullable: false),
                    MaterialTypeId = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitOfMeasure = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProcessedWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    WeightUnit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    QualityGrade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DestinationVendor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ExpectedSalesPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    ActualSalesPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    SaleDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CertificationNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsHazardous = table.Column<bool>(type: "boolean", nullable: false),
                    HazardousClassification = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MaterialTypeId1 = table.Column<int>(type: "integer", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessedMaterials_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProcessedMaterials_MaterialTypes_MaterialTypeId1",
                        column: x => x.MaterialTypeId1,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProcessedMaterials_ProcessingLots_ProcessingLotId",
                        column: x => x.ProcessingLotId,
                        principalTable: "ProcessingLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcessingLotId = table.Column<int>(type: "integer", nullable: false),
                    StepName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ResponsibleUserId = table.Column<int>(type: "integer", nullable: true),
                    LaborHours = table.Column<decimal>(type: "numeric", nullable: true),
                    MachineHours = table.Column<decimal>(type: "numeric", nullable: true),
                    EnergyConsumption = table.Column<decimal>(type: "numeric", nullable: true),
                    ProcessingCostPerUnit = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalStepCost = table.Column<decimal>(type: "numeric", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Equipment = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    InputWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    OutputWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    WasteWeight = table.Column<decimal>(type: "numeric", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessingSteps_ProcessingLots_ProcessingLotId",
                        column: x => x.ProcessingLotId,
                        principalTable: "ProcessingLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessingSteps_Users_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssetID = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AssetCategoryId = table.Column<int>(type: "integer", nullable: true),
                    SourceShipmentId = table.Column<int>(type: "integer", nullable: true),
                    Manufacturer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SerialNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OriginalPurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OriginalCost = table.Column<decimal>(type: "numeric", nullable: true),
                    Condition = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EstimatedValue = table.Column<decimal>(type: "numeric", nullable: true),
                    ActualSalePrice = table.Column<decimal>(type: "numeric", nullable: true),
                    CostOfRecovery = table.Column<decimal>(type: "numeric", nullable: true),
                    IsDataBearing = table.Column<bool>(type: "boolean", nullable: false),
                    StorageDeviceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StorageCapacity = table.Column<decimal>(type: "numeric", nullable: true),
                    DataSanitizationMethod = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DataSanitizationStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DataSanitizationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataSanitizationCertificate = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DataDestructionCost = table.Column<decimal>(type: "numeric", nullable: true),
                    InternalAuditNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    InternalAuditScore = table.Column<decimal>(type: "numeric", nullable: true),
                    CurrentLocation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CurrentResponsibleUserId = table.Column<int>(type: "integer", nullable: true),
                    CurrentStatusId = table.Column<int>(type: "integer", nullable: true),
                    ReuseDisposition = table.Column<bool>(type: "boolean", nullable: false),
                    ResaleDisposition = table.Column<bool>(type: "boolean", nullable: false),
                    ReuseRecipient = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReusePurpose = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReuseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FairMarketValue = table.Column<decimal>(type: "numeric", nullable: true),
                    Buyer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SaleDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResalePlatform = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CostOfSale = table.Column<decimal>(type: "numeric", nullable: true),
                    SalesInvoice = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RecyclingVendor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RecyclingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RecyclingCost = table.Column<decimal>(type: "numeric", nullable: true),
                    CertificateOfRecycling = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ProcessingLotId = table.Column<int>(type: "integer", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_AssetCategories_AssetCategoryId",
                        column: x => x.AssetCategoryId,
                        principalTable: "AssetCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_AssetTrackingStatuses_CurrentStatusId",
                        column: x => x.CurrentStatusId,
                        principalTable: "AssetTrackingStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_ProcessingLots_ProcessingLotId",
                        column: x => x.ProcessingLotId,
                        principalTable: "ProcessingLots",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_Shipments_SourceShipmentId",
                        column: x => x.SourceShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_Users_CurrentResponsibleUserId",
                        column: x => x.CurrentResponsibleUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChainOfCustodyRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssetId = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    StatusChangeId = table.Column<int>(type: "integer", nullable: true),
                    StatusChange = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ActionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DocumentReference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChainOfCustodyRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChainOfCustodyRecords_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChainOfCustodyRecords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ClientContactId1",
                table: "Shipments",
                column: "ClientContactId1");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingLots_LotID_ClientId",
                table: "ProcessingLots",
                columns: new[] { "LotID", "ClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSettings_SettingKey_ClientId",
                table: "ApplicationSettings",
                columns: new[] { "SettingKey", "ClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetCategoryId",
                table: "Assets",
                column: "AssetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetID_ClientId",
                table: "Assets",
                columns: new[] { "AssetID", "ClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CurrentResponsibleUserId",
                table: "Assets",
                column: "CurrentResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CurrentStatusId",
                table: "Assets",
                column: "CurrentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ProcessingLotId",
                table: "Assets",
                column: "ProcessingLotId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SourceShipmentId",
                table: "Assets",
                column: "SourceShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTrackingStatuses_StatusName_ClientId",
                table: "AssetTrackingStatuses",
                columns: new[] { "StatusName", "ClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChainOfCustodyRecords_AssetId",
                table: "ChainOfCustodyRecords",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ChainOfCustodyRecords_UserId",
                table: "ChainOfCustodyRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBaseArticles_AuthorUserId",
                table: "KnowledgeBaseArticles",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedMaterials_MaterialTypeId",
                table: "ProcessedMaterials",
                column: "MaterialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedMaterials_MaterialTypeId1",
                table: "ProcessedMaterials",
                column: "MaterialTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedMaterials_ProcessingLotId",
                table: "ProcessedMaterials",
                column: "ProcessingLotId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingSteps_ProcessingLotId",
                table: "ProcessingSteps",
                column: "ProcessingLotId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingSteps_ResponsibleUserId",
                table: "ProcessingSteps",
                column: "ResponsibleUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessingLots_Users_OperatorUserId",
                table: "ProcessingLots",
                column: "OperatorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_ClientContacts_ClientContactId1",
                table: "Shipments",
                column: "ClientContactId1",
                principalTable: "ClientContacts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessingLots_Users_OperatorUserId",
                table: "ProcessingLots");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_ClientContacts_ClientContactId1",
                table: "Shipments");

            migrationBuilder.DropTable(
                name: "ApplicationSettings");

            migrationBuilder.DropTable(
                name: "ChainOfCustodyRecords");

            migrationBuilder.DropTable(
                name: "KnowledgeBaseArticles");

            migrationBuilder.DropTable(
                name: "ProcessedMaterials");

            migrationBuilder.DropTable(
                name: "ProcessingSteps");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "AssetTrackingStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_ClientContactId1",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_ProcessingLots_LotID_ClientId",
                table: "ProcessingLots");

            migrationBuilder.DropColumn(
                name: "ClientContactId1",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "ActualRevenue",
                table: "ProcessingLots");

            migrationBuilder.DropColumn(
                name: "CertificationNumber",
                table: "ProcessingLots");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProcessingLots");

            migrationBuilder.DropColumn(
                name: "ExpectedRevenue",
                table: "ProcessingLots");

            migrationBuilder.DropColumn(
                name: "IncomingMaterialCost",
                table: "ProcessingLots");

            migrationBuilder.DropColumn(
                name: "LotID",
                table: "ProcessingLots");

            migrationBuilder.DropColumn(
                name: "TotalIncomingWeight",
                table: "ProcessingLots");

            migrationBuilder.DropColumn(
                name: "DefaultPricePerUnit",
                table: "MaterialTypes");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "MaterialTypes");

            migrationBuilder.DropColumn(
                name: "DefaultDisposition",
                table: "AssetCategories");

            migrationBuilder.DropColumn(
                name: "RequiresDataSanitization",
                table: "AssetCategories");

            migrationBuilder.RenameColumn(
                name: "TotalProcessedWeight",
                table: "ProcessingLots",
                newName: "ActualReceivedWeight");

            migrationBuilder.RenameColumn(
                name: "QualityControlNotes",
                table: "ProcessingLots",
                newName: "ProcessingNotes");

            migrationBuilder.RenameColumn(
                name: "OperatorUserId",
                table: "ProcessingLots",
                newName: "SourceShipmentId");

            migrationBuilder.RenameColumn(
                name: "CompletionDate",
                table: "ProcessingLots",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "CertificationStatus",
                table: "ProcessingLots",
                newName: "ProcessingMethod");

            migrationBuilder.RenameIndex(
                name: "IX_ProcessingLots_OperatorUserId",
                table: "ProcessingLots",
                newName: "IX_ProcessingLots_SourceShipmentId");

            migrationBuilder.AddColumn<string>(
                name: "LotNumber",
                table: "ProcessingLots",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WeightUnit",
                table: "ProcessingLots",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MaterialTypes",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "MaterialTypes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecyclingMethod",
                table: "MaterialTypes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasure",
                table: "MaterialTypes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "ClientContacts",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ClientContacts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "ClientContacts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingLots_LotNumber_ClientId",
                table: "ProcessingLots",
                columns: new[] { "LotNumber", "ClientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientContacts_ClientId",
                table: "ClientContacts",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientContacts_Clients_ClientId",
                table: "ClientContacts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessingLots_Shipments_SourceShipmentId",
                table: "ProcessingLots",
                column: "SourceShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
