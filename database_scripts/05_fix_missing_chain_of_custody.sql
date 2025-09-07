
BEGIN;

INSERT INTO "ChainOfCustody" ("AssetId", "UserId", "VendorId", "Location", "StatusChange", "ActionType", "Notes", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT DISTINCT
    a."Id" as "AssetId",
    a."CreatedBy" as "UserId",
    v."Id" as "VendorId",
    a."CurrentLocation" as "Location",
    'Asset Received' as "StatusChange",
    'Status Change' as "ActionType",
    'Initial chain of custody record - retroactively added' as "Notes",
    a."DateCreated" as "DateCreated",
    a."DateCreated" as "DateUpdated",
    a."CreatedBy",
    a."CreatedBy" as "UpdatedBy",
    a."ClientId"
FROM "Assets" a
LEFT JOIN "ChainOfCustody" coc ON coc."AssetId" = a."Id"
CROSS JOIN LATERAL (
    SELECT "Id" 
    FROM "Vendors" v 
    WHERE v."ClientId" = a."ClientId" 
    ORDER BY v."Id" 
    LIMIT 1
) v
WHERE coc."Id" IS NULL;

COMMIT;
