
INSERT INTO "Shipments" ("ShipmentNumber", "OriginatorClientId", "Carrier", "TrackingNumber", "ScheduledPickupDate", "ActualPickupDate", "ShipmentDate", "Status", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT 'SHP-CA-001', 101, 'CarrierX', 'TRK-CA-001', NOW() - INTERVAL '20 days', NOW() - INTERVAL '19 days', NOW() - INTERVAL '20 days', 'Received', NOW() - INTERVAL '20 days', NOW() - INTERVAL '19 days', 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'
WHERE NOT EXISTS (SELECT 1 FROM "Shipments" WHERE "TrackingNumber" = 'TRK-CA-001');

INSERT INTO "Shipments" ("ShipmentNumber", "OriginatorClientId", "Carrier", "TrackingNumber", "ScheduledPickupDate", "ActualPickupDate", "ShipmentDate", "Status", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT 'SHP-CA-002', 102, 'CarrierY', 'TRK-CA-002', NOW() - INTERVAL '10 days', NOW() - INTERVAL '9 days', NOW() - INTERVAL '10 days', 'Received', NOW() - INTERVAL '10 days', NOW() - INTERVAL '9 days', 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'
WHERE NOT EXISTS (SELECT 1 FROM "Shipments" WHERE "TrackingNumber" = 'TRK-CA-002');

INSERT INTO "ProcessingLots" ("LotNumber", "IncomingMaterialNotes", "ContaminationPercentage", "TotalIncomingWeight", "SourceShipmentId", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT 'LOT-CA-001', 'Plastic wrap and labels present', 3.5, 120.0, s1."Id", NOW() - INTERVAL '18 days', NOW() - INTERVAL '18 days', 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'
FROM "Shipments" s1
WHERE s1."TrackingNumber" = 'TRK-CA-001'
AND NOT EXISTS (SELECT 1 FROM "ProcessingLots" WHERE "LotNumber" = 'LOT-CA-001');

INSERT INTO "ProcessingLots" ("LotNumber", "IncomingMaterialNotes", "ContaminationPercentage", "TotalIncomingWeight", "SourceShipmentId", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT 'LOT-CA-002', 'Mixed metals with small glass shards', 6.2, 200.0, s2."Id", NOW() - INTERVAL '8 days', NOW() - INTERVAL '8 days', 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'
FROM "Shipments" s2
WHERE s2."TrackingNumber" = 'TRK-CA-002'
AND NOT EXISTS (SELECT 1 FROM "ProcessingLots" WHERE "LotNumber" = 'LOT-CA-002');

INSERT INTO "ProcessingLots" ("LotNumber", "IncomingMaterialNotes", "ContaminationPercentage", "TotalIncomingWeight", "SourceShipmentId", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId")
SELECT 'LOT-CA-003', 'Cardboard pieces and adhesive residues', 4.1, 150.0, s2."Id", NOW() - INTERVAL '6 days', NOW() - INTERVAL '6 days', 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'
FROM "Shipments" s2
WHERE s2."TrackingNumber" = 'TRK-CA-002'
AND NOT EXISTS (SELECT 1 FROM "ProcessingLots" WHERE "LotNumber" = 'LOT-CA-003');
