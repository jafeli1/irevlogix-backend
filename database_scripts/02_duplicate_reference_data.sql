
BEGIN;


INSERT INTO "AssetCategories" (
    "Name", 
    "Description", 
    "IsActive", 
    "DefaultDisposition", 
    "RequiresDataSanitization", 
    "ParentCategory", 
    "RequiresDataDestruction", 
    "IsRecoverable",
    "DateCreated", 
    "DateUpdated", 
    "CreatedBy", 
    "UpdatedBy", 
    "ClientId"
)
SELECT 
    "Name", 
    "Description", 
    "IsActive", 
    "DefaultDisposition", 
    "RequiresDataSanitization", 
    "ParentCategory", 
    "RequiresDataDestruction", 
    "IsRecoverable",
    NOW(), 
    NOW(), 
    1, 
    1, 
    '7d02a831-2bcd-4435-a944-ed0788dfe9e4'
FROM "AssetCategories" 
WHERE "ClientId" = 'ADMIN_CLIENT_001';


INSERT INTO "MaterialTypes" (
    "Name", 
    "Description", 
    "IsActive", 
    "DefaultPricePerUnit", 
    "UnitOfMeasure",
    "DateCreated", 
    "DateUpdated", 
    "CreatedBy", 
    "UpdatedBy", 
    "ClientId"
)
SELECT 
    "Name", 
    "Description", 
    "IsActive", 
    "DefaultPricePerUnit", 
    "UnitOfMeasure",
    NOW(), 
    NOW(), 
    1, 
    1, 
    '7d02a831-2bcd-4435-a944-ed0788dfe9e4'
FROM "MaterialTypes" 
WHERE "ClientId" = 'ADMIN_CLIENT_001';

COMMIT;
