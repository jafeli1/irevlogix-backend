DROP TABLE IF EXISTS tmp_client_ctx;
CREATE TEMP TABLE tmp_client_ctx AS
SELECT "ClientId" AS cid
FROM "Users"
WHERE LOWER("Email") = LOWER('admin@irevlogix.ai')
LIMIT 1;
DROP TABLE IF EXISTS tmp_originators;
CREATE TEMP TABLE tmp_originators AS
SELECT "Id"
FROM "Clients"
WHERE "ClientId" = (SELECT cid FROM tmp_client_ctx)
ORDER BY "Id" ASC
LIMIT 2;

DROP TABLE IF EXISTS tmp_o1;
CREATE TEMP TABLE tmp_o1 AS SELECT MIN("Id") AS oid FROM tmp_originators;

DROP TABLE IF EXISTS tmp_o2;
CREATE TEMP TABLE tmp_o2 AS SELECT MAX("Id") AS oid FROM tmp_originators;



INSERT INTO "MaterialTypes" ("Name","Description","IsActive","DateCreated","DateUpdated","CreatedBy","UpdatedBy","ClientId")
SELECT 'Mixed Plastics', 'Post-consumer mixed plastics', TRUE, NOW(), NOW(), 1, 1, (SELECT cid FROM tmp_client_ctx)
WHERE NOT EXISTS (SELECT 1 FROM "MaterialTypes" WHERE "Name"='Mixed Plastics' AND "ClientId"=(SELECT cid FROM tmp_client_ctx));

INSERT INTO "MaterialTypes" ("Name","Description","IsActive","DateCreated","DateUpdated","CreatedBy","UpdatedBy","ClientId")
SELECT 'Mixed Metals', 'Ferrous/non-ferrous mix', TRUE, NOW(), NOW(), 1, 1, (SELECT cid FROM tmp_client_ctx)
WHERE NOT EXISTS (SELECT 1 FROM "MaterialTypes" WHERE "Name"='Mixed Metals' AND "ClientId"=(SELECT cid FROM tmp_client_ctx));

INSERT INTO "MaterialTypes" ("Name","Description","IsActive","DateCreated","DateUpdated","CreatedBy","UpdatedBy","ClientId")
SELECT 'Cardboard', 'OCC & paper fiber', TRUE, NOW(), NOW(), 1, 1, (SELECT cid FROM tmp_client_ctx)
WHERE NOT EXISTS (SELECT 1 FROM "MaterialTypes" WHERE "Name"='Cardboard' AND "ClientId"=(SELECT cid FROM tmp_client_ctx));



INSERT INTO "Shipments" ("ShipmentNumber", "OriginatorClientId", "Carrier", "TrackingNumber", "ScheduledPickupDate", "ActualPickupDate", "ShipmentDate", "Status", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT 'SHP-CA-001', (SELECT oid FROM tmp_o1), 'CarrierX', 'TRK-CA-001', NOW() - INTERVAL '20 days', NOW() - INTERVAL '19 days', NOW() - INTERVAL '20 days', 'Received', NOW() - INTERVAL '20 days', NOW() - INTERVAL '19 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
WHERE NOT EXISTS (SELECT 1 FROM "Shipments" WHERE "TrackingNumber" = 'TRK-CA-001' AND "ClientId"=(SELECT cid FROM tmp_client_ctx));

INSERT INTO "Shipments" ("ShipmentNumber", "OriginatorClientId", "Carrier", "TrackingNumber", "ScheduledPickupDate", "ActualPickupDate", "ShipmentDate", "Status", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT 'SHP-CA-002', (SELECT oid FROM tmp_o2), 'CarrierY', 'TRK-CA-002', NOW() - INTERVAL '10 days', NOW() - INTERVAL '9 days', NOW() - INTERVAL '10 days', 'Received', NOW() - INTERVAL '10 days', NOW() - INTERVAL '9 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
WHERE NOT EXISTS (SELECT 1 FROM "Shipments" WHERE "TrackingNumber" = 'TRK-CA-002' AND "ClientId"=(SELECT cid FROM tmp_client_ctx));

INSERT INTO "ProcessingLots" ("LotNumber", "Status", "IncomingMaterialNotes", "ContaminationPercentage", "TotalIncomingWeight", "SourceShipmentId", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT 'LOT-CA-001', 'Created', 'Plastic wrap and labels present', 3.5, 120.0, s1."Id", NOW() - INTERVAL '18 days', NOW() - INTERVAL '18 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
FROM "Shipments" s1
WHERE s1."TrackingNumber" = 'TRK-CA-001'
AND NOT EXISTS (SELECT 1 FROM "ProcessingLots" WHERE "LotNumber" = 'LOT-CA-001');

INSERT INTO "ProcessingLots" ("LotNumber", "Status", "IncomingMaterialNotes", "ContaminationPercentage", "TotalIncomingWeight", "SourceShipmentId", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT 'LOT-CA-002', 'Created', 'Mixed metals with small glass shards', 6.2, 200.0, s2."Id", NOW() - INTERVAL '8 days', NOW() - INTERVAL '8 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
FROM "Shipments" s2
WHERE s2."TrackingNumber" = 'TRK-CA-002'
AND NOT EXISTS (SELECT 1 FROM "ProcessingLots" WHERE "LotNumber" = 'LOT-CA-002');

INSERT INTO "ProcessingLots" ("LotNumber", "Status", "IncomingMaterialNotes", "ContaminationPercentage", "TotalIncomingWeight", "SourceShipmentId", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT 'LOT-CA-003', 'Created', 'Cardboard pieces and adhesive residues', 4.1, 150.0, s2."Id", NOW() - INTERVAL '6 days', NOW() - INTERVAL '6 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
FROM "Shipments" s2
WHERE s2."TrackingNumber" = 'TRK-CA-002'
AND NOT EXISTS (SELECT 1 FROM "ProcessingLots" WHERE "LotNumber" = 'LOT-CA-003');
WITH s1 AS (
  SELECT "Id" as sid FROM "Shipments" WHERE "TrackingNumber"='TRK-CA-001'
), s2 AS (
  SELECT "Id" as sid FROM "Shipments" WHERE "TrackingNumber"='TRK-CA-002'
), mt_plastics AS (
  SELECT "Id" as mtid FROM "MaterialTypes" WHERE "Name"='Mixed Plastics' AND "ClientId"=(SELECT cid FROM tmp_client_ctx)
), mt_metals AS (
  SELECT "Id" as mtid FROM "MaterialTypes" WHERE "Name"='Mixed Metals' AND "ClientId"=(SELECT cid FROM tmp_client_ctx)
), mt_cardboard AS (
  SELECT "Id" as mtid FROM "MaterialTypes" WHERE "Name"='Cardboard' AND "ClientId"=(SELECT cid FROM tmp_client_ctx)
), lot1 AS (
  SELECT "Id" as lotid FROM "ProcessingLots" WHERE "LotNumber"='LOT-CA-001'
), lot2 AS (
  SELECT "Id" as lotid FROM "ProcessingLots" WHERE "LotNumber"='LOT-CA-002'
), lot3 AS (
  SELECT "Id" as lotid FROM "ProcessingLots" WHERE "LotNumber"='LOT-CA-003'
)
INSERT INTO "ShipmentItems" ("ShipmentId","MaterialTypeId","ProcessingLotId","Description","Quantity","UnitOfMeasure","Weight","WeightUnit","Notes","IsDataBearingDevice","DateCreated","DateUpdated","CreatedBy","UpdatedBy","ClientId")
SELECT s1.sid, mt_plastics.mtid, lot1.lotid, 'Baled plastics with film contamination', 1, 'pallet', 120.0, 'kg', 'Plastic wrap and labels present', FALSE, NOW() - INTERVAL '18 days', NOW() - INTERVAL '18 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
FROM s1, mt_plastics, lot1
WHERE NOT EXISTS (
  SELECT 1 FROM "ShipmentItems" si JOIN s1 ON si."ShipmentId"=s1.sid WHERE si."Description"='Baled plastics with film contamination'
);

WITH s2 AS (
  SELECT "Id" as sid FROM "Shipments" WHERE "TrackingNumber"='TRK-CA-002'
), mt_metals AS (
  SELECT "Id" as mtid FROM "MaterialTypes" WHERE "Name"='Mixed Metals' AND "ClientId"=(SELECT cid FROM tmp_client_ctx)
), lot2 AS (
  SELECT "Id" as lotid FROM "ProcessingLots" WHERE "LotNumber"='LOT-CA-002'
)
INSERT INTO "ShipmentItems" ("ShipmentId","MaterialTypeId","ProcessingLotId","Description","Quantity","UnitOfMeasure","Weight","WeightUnit","Notes","IsDataBearingDevice","DateCreated","DateUpdated","CreatedBy","UpdatedBy","ClientId")
SELECT s2.sid, mt_metals.mtid, lot2.lotid, 'Mixed metals with trace glass', 1, 'pallet', 200.0, 'kg', 'Mixed metals with small glass shards', FALSE, NOW() - INTERVAL '8 days', NOW() - INTERVAL '8 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
FROM s2, mt_metals, lot2
WHERE NOT EXISTS (
  SELECT 1 FROM "ShipmentItems" si JOIN s2 ON si."ShipmentId"=s2.sid WHERE si."Description"='Mixed metals with trace glass'
);

WITH s2 AS (
  SELECT "Id" as sid FROM "Shipments" WHERE "TrackingNumber"='TRK-CA-002'
), mt_cardboard AS (
  SELECT "Id" as mtid FROM "MaterialTypes" WHERE "Name"='Cardboard' AND "ClientId"=(SELECT cid FROM tmp_client_ctx)
), lot3 AS (
  SELECT "Id" as lotid FROM "ProcessingLots" WHERE "LotNumber"='LOT-CA-003'
)
INSERT INTO "ShipmentItems" ("ShipmentId","MaterialTypeId","ProcessingLotId","Description","Quantity","UnitOfMeasure","Weight","WeightUnit","Notes","IsDataBearingDevice","DateCreated","DateUpdated","CreatedBy","UpdatedBy","ClientId")
SELECT s2.sid, mt_cardboard.mtid, lot3.lotid, 'Paper/cardboard with adhesive', 1, 'pallet', 150.0, 'kg', 'Cardboard pieces and adhesive residues', FALSE, NOW() - INTERVAL '6 days', NOW() - INTERVAL '6 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
FROM s2, mt_cardboard, lot3
WHERE NOT EXISTS (
  SELECT 1 FROM "ShipmentItems" si JOIN s2 ON si."ShipmentId"=s2.sid WHERE si."Description"='Paper/cardboard with adhesive'
);

INSERT INTO "ProcessingLots" ("LotNumber", "Status", "IncomingMaterialNotes", "ContaminationPercentage", "TotalIncomingWeight", "SourceShipmentId", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT 'LOT-CA-004', 'Created', 'Food residue and films on plastics', 5.4, 110.0, s1."Id", NOW() - INTERVAL '15 days', NOW() - INTERVAL '15 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
FROM "Shipments" s1
WHERE s1."TrackingNumber" = 'TRK-CA-001'
AND NOT EXISTS (SELECT 1 FROM "ProcessingLots" WHERE "LotNumber" = 'LOT-CA-004');

INSERT INTO "ProcessingLots" ("LotNumber", "Status", "IncomingMaterialNotes", "ContaminationPercentage", "TotalIncomingWeight", "SourceShipmentId", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT 'LOT-CA-005', 'Created', 'Metal loads with painted parts', 2.3, 90.0, s2."Id", NOW() - INTERVAL '12 days', NOW() - INTERVAL '12 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
FROM "Shipments" s2
WHERE s2."TrackingNumber" = 'TRK-CA-002'
AND NOT EXISTS (SELECT 1 FROM "ProcessingLots" WHERE "LotNumber" = 'LOT-CA-005');

INSERT INTO "ProcessingLots" ("LotNumber", "Status", "IncomingMaterialNotes", "ContaminationPercentage", "TotalIncomingWeight", "SourceShipmentId", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT 'LOT-CA-006', 'Created', 'Cardboard with tape and staples', 3.1, 140.0, s1."Id", NOW() - INTERVAL '14 days', NOW() - INTERVAL '14 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
FROM "Shipments" s1
WHERE s1."TrackingNumber" = 'TRK-CA-001'
AND NOT EXISTS (SELECT 1 FROM "ProcessingLots" WHERE "LotNumber" = 'LOT-CA-006');

WITH s1 AS (
  SELECT "Id" as sid FROM "Shipments" WHERE "TrackingNumber"='TRK-CA-001'
), mt_plastics AS (
  SELECT "Id" as mtid FROM "MaterialTypes" WHERE "Name"='Mixed Plastics' AND "ClientId"=(SELECT cid FROM tmp_client_ctx)
), mt_cardboard AS (
  SELECT "Id" as mtid FROM "MaterialTypes" WHERE "Name"='Cardboard' AND "ClientId"=(SELECT cid FROM tmp_client_ctx)
), lot4 AS (
  SELECT "Id" as lotid FROM "ProcessingLots" WHERE "LotNumber"='LOT-CA-004'
), lot6 AS (
  SELECT "Id" as lotid FROM "ProcessingLots" WHERE "LotNumber"='LOT-CA-006'
)
INSERT INTO "ShipmentItems" ("ShipmentId","MaterialTypeId","ProcessingLotId","Description","Quantity","UnitOfMeasure","Weight","WeightUnit","Notes","IsDataBearingDevice","DateCreated","DateUpdated","CreatedBy","UpdatedBy","ClientId")
SELECT s1.sid, mt_plastics.mtid, lot4.lotid, 'Plastics with food/film residue', 1, 'pallet', 110.0, 'kg', 'Food residue and films on plastics', FALSE, NOW() - INTERVAL '15 days', NOW() - INTERVAL '15 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
FROM s1, mt_plastics, lot4
WHERE NOT EXISTS (
  SELECT 1 FROM "ShipmentItems" si JOIN s1 ON si."ShipmentId"=s1.sid WHERE si."Description"='Plastics with food/film residue'
);

WITH s2 AS (
  SELECT "Id" as sid FROM "Shipments" WHERE "TrackingNumber"='TRK-CA-002'
), mt_metals AS (
  SELECT "Id" as mtid FROM "MaterialTypes" WHERE "Name"='Mixed Metals' AND "ClientId"=(SELECT cid FROM tmp_client_ctx)
), lot5 AS (
  SELECT "Id" as lotid FROM "ProcessingLots" WHERE "LotNumber"='LOT-CA-005'
)
INSERT INTO "ShipmentItems" ("ShipmentId","MaterialTypeId","ProcessingLotId","Description","Quantity","UnitOfMeasure","Weight","WeightUnit","Notes","IsDataBearingDevice","DateCreated","DateUpdated","CreatedBy","UpdatedBy","ClientId")
SELECT s2.sid, mt_metals.mtid, lot5.lotid, 'Metal with painted components', 1, 'pallet', 90.0, 'kg', 'Metal loads with painted parts', FALSE, NOW() - INTERVAL '12 days', NOW() - INTERVAL '12 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
FROM s2, mt_metals, lot5
WHERE NOT EXISTS (
  SELECT 1 FROM "ShipmentItems" si JOIN s2 ON si."ShipmentId"=s2.sid WHERE si."Description"='Metal with painted components'
);

WITH s1 AS (
  SELECT "Id" as sid FROM "Shipments" WHERE "TrackingNumber"='TRK-CA-001'
), mt_cardboard AS (
  SELECT "Id" as mtid FROM "MaterialTypes" WHERE "Name"='Cardboard' AND "ClientId"=(SELECT cid FROM tmp_client_ctx)
), lot6 AS (
  SELECT "Id" as lotid FROM "ProcessingLots" WHERE "LotNumber"='LOT-CA-006'
)
INSERT INTO "ShipmentItems" ("ShipmentId","MaterialTypeId","ProcessingLotId","Description","Quantity","UnitOfMeasure","Weight","WeightUnit","Notes","IsDataBearingDevice","DateCreated","DateUpdated","CreatedBy","UpdatedBy","ClientId")
SELECT s1.sid, mt_cardboard.mtid, lot6.lotid, 'Cardboard with tape/staples', 1, 'pallet', 140.0, 'kg', 'Cardboard with tape and staples', FALSE, NOW() - INTERVAL '14 days', NOW() - INTERVAL '14 days', 1, 1, (SELECT cid FROM tmp_client_ctx)
FROM s1, mt_cardboard, lot6
WHERE NOT EXISTS (
  SELECT 1 FROM "ShipmentItems" si JOIN s1 ON si."ShipmentId"=s1.sid WHERE si."Description"='Cardboard with tape/staples'
);
