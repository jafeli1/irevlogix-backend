
UPDATE "ShipmentItems" 
SET "ProcessingLotId" = (
    SELECT pl."Id" 
    FROM "ProcessingLots" pl 
    JOIN "Shipments" s ON "ShipmentItems"."ShipmentId" = s."Id"
    WHERE pl."ClientId" = s."ClientId" 
    AND pl."DateCreated" >= s."DateCreated"
    ORDER BY pl."DateCreated" ASC 
    LIMIT 1
)
WHERE "ProcessingLotId" IS NULL;
