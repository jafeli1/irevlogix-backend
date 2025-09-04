
BEGIN;


DELETE FROM "UserRoles";

DELETE FROM "ProcessingLots";

DELETE FROM "ShipmentStatusHistories";

DELETE FROM "Users" WHERE "Id" > 2;

SELECT setval('"Users_Id_seq"', 2);


DELETE FROM "AssetCategories" WHERE "ClientId" != 'ADMIN_CLIENT_001';

SELECT setval('"AssetCategories_Id_seq"', (SELECT COALESCE(MAX("Id"), 0) FROM "AssetCategories"));


DELETE FROM "MaterialTypes" WHERE "ClientId" != 'ADMIN_CLIENT_001';

SELECT setval('"MaterialTypes_Id_seq"', (SELECT COALESCE(MAX("Id"), 0) FROM "MaterialTypes"));


DELETE FROM "Assets";
SELECT setval('"Assets_Id_seq"', 1);

DELETE FROM "ChainOfCustody";
SELECT setval('"ChainOfCustody_Id_seq"', 1);

DELETE FROM "Shipments";
SELECT setval('"Shipments_Id_seq"', 1, false);

DELETE FROM "ShipmentItems";
SELECT setval('"ShipmentItems_Id_seq"', 1);

DELETE FROM "ShipmentStatusHistories";
SELECT setval('"ShipmentStatusHistories_Id_seq"', 1);

DELETE FROM "ShipmentDocuments";
SELECT setval('"ShipmentDocuments_Id_seq"', 1);

DELETE FROM "ProcessingLots";
SELECT setval('"ProcessingLots_Id_seq"', 1);

DELETE FROM "ProcessedMaterials";
SELECT setval('"ProcessedMaterials_Id_seq"', 1);

DELETE FROM "ProcessedMaterialSales";
SELECT setval('"ProcessedMaterialSales_Id_seq"', 1);

DELETE FROM "ProcessingSteps";
SELECT setval('"ProcessingSteps_Id_seq"', 1);

DELETE FROM "Vendors";
SELECT setval('"Vendors_Id_seq"', 1);

DELETE FROM "VendorCommunications";
SELECT setval('"VendorCommunications_Id_seq"', 1);

DELETE FROM "VendorContracts";
SELECT setval('"VendorContracts_Id_seq"', 1);

DELETE FROM "VendorDocuments";
SELECT setval('"VendorDocuments_Id_seq"', 1);

DELETE FROM "VendorFacilities";
SELECT setval('"VendorFacilities_Id_seq"', 1);

DELETE FROM "VendorPricing";
SELECT setval('"VendorPricing_Id_seq"', 1);

DELETE FROM "KnowledgeBaseArticles";
SELECT setval('"KnowledgeBaseArticles_Id_seq"', 1);

DELETE FROM "AssetTrackingStatuses";
SELECT setval('"AssetTrackingStatuses_Id_seq"', 1);

DELETE FROM "AssetDocuments";
SELECT setval('"AssetDocuments_Id_seq"', 1);

DELETE FROM "ReverseRequests";
SELECT setval('"ReverseRequests_Id_seq"', 1);

DELETE FROM "RecoveryRequests";
SELECT setval('"RecoveryRequests_Id_seq"', 1);

DELETE FROM "ProcessedMaterialDocuments";
SELECT setval('"ProcessedMaterialDocuments_Id_seq"', 1);

DELETE FROM "ProcessedMaterialTests";
SELECT setval('"ProcessedMaterialTests_Id_seq"', 1);

DELETE FROM "FreightLossDamageClaims";
SELECT setval('"FreightLossDamageClaims_Id_seq"', 1);

DELETE FROM "ContractorTechnicians";
SELECT setval('"ContractorTechnicians_Id_seq"', 1);

DELETE FROM "ContractorTechnicianDocuments";
SELECT setval('"ContractorTechnicianDocuments_Id_seq"', 1);

DELETE FROM "ClientContacts";
SELECT setval('"ClientContacts_Id_seq"', 1);

DELETE FROM "Tenants";
SELECT setval('"Tenants_Id_seq"', 1);


UPDATE "Clients" SET "CreatedBy" = 1, "UpdatedBy" = 1;

UPDATE "Roles" SET "CreatedBy" = 1, "UpdatedBy" = 1;

UPDATE "UserRoles" SET "CreatedBy" = 1, "UpdatedBy" = 1;

UPDATE "Permissions" SET "CreatedBy" = 1, "UpdatedBy" = 1;

UPDATE "RolePermissions" SET "CreatedBy" = 1, "UpdatedBy" = 1;

UPDATE "ApplicationSettings" SET "CreatedBy" = 1, "UpdatedBy" = 1;

UPDATE "MaterialTypes" SET "CreatedBy" = 1, "UpdatedBy" = 1;
UPDATE "AssetCategories" SET "CreatedBy" = 1, "UpdatedBy" = 1;

COMMIT;
