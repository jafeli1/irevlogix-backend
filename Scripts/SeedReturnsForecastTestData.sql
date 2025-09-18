
INSERT INTO "MaterialTypes" ("Name", "Description", "IsActive", "DefaultPricePerUnit", "UnitOfMeasure", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
SELECT 'Electronics', 'Electronics for recycling', true, 10.0, 'kg', 'ADMIN_CLIENT_001', 1, 1, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "MaterialTypes" WHERE "Name" = 'Electronics' AND "ClientId" = 'ADMIN_CLIENT_001');

INSERT INTO "MaterialTypes" ("Name", "Description", "IsActive", "DefaultPricePerUnit", "UnitOfMeasure", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
SELECT 'Metals', 'Metals for recycling', true, 15.0, 'kg', 'ADMIN_CLIENT_001', 1, 1, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "MaterialTypes" WHERE "Name" = 'Metals' AND "ClientId" = 'ADMIN_CLIENT_001');

INSERT INTO "MaterialTypes" ("Name", "Description", "IsActive", "DefaultPricePerUnit", "UnitOfMeasure", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
SELECT 'Plastics', 'Plastics for recycling', true, 8.0, 'kg', 'ADMIN_CLIENT_001', 1, 1, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "MaterialTypes" WHERE "Name" = 'Plastics' AND "ClientId" = 'ADMIN_CLIENT_001');

INSERT INTO "MaterialTypes" ("Name", "Description", "IsActive", "DefaultPricePerUnit", "UnitOfMeasure", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
SELECT 'Batteries', 'Batteries for recycling', true, 20.0, 'kg', 'ADMIN_CLIENT_001', 1, 1, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "MaterialTypes" WHERE "Name" = 'Batteries' AND "ClientId" = 'ADMIN_CLIENT_001');

INSERT INTO "MaterialTypes" ("Name", "Description", "IsActive", "DefaultPricePerUnit", "UnitOfMeasure", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
SELECT 'Cables', 'Cables for recycling', true, 12.0, 'kg', 'ADMIN_CLIENT_001', 1, 1, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "MaterialTypes" WHERE "Name" = 'Cables' AND "ClientId" = 'ADMIN_CLIENT_001');

INSERT INTO "MaterialTypes" ("Name", "Description", "IsActive", "DefaultPricePerUnit", "UnitOfMeasure", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
SELECT 'Monitors', 'Monitors for recycling', true, 25.0, 'kg', 'ADMIN_CLIENT_001', 1, 1, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "MaterialTypes" WHERE "Name" = 'Monitors' AND "ClientId" = 'ADMIN_CLIENT_001');

INSERT INTO "MaterialTypes" ("Name", "Description", "IsActive", "DefaultPricePerUnit", "UnitOfMeasure", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
SELECT 'Laptops', 'Laptops for recycling', true, 30.0, 'kg', 'ADMIN_CLIENT_001', 1, 1, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "MaterialTypes" WHERE "Name" = 'Laptops' AND "ClientId" = 'ADMIN_CLIENT_001');

INSERT INTO "MaterialTypes" ("Name", "Description", "IsActive", "DefaultPricePerUnit", "UnitOfMeasure", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
SELECT 'Servers', 'Servers for recycling', true, 50.0, 'kg', 'ADMIN_CLIENT_001', 1, 1, NOW(), NOW()
WHERE NOT EXISTS (SELECT 1 FROM "MaterialTypes" WHERE "Name" = 'Servers' AND "ClientId" = 'ADMIN_CLIENT_001');


INSERT INTO "Shipments" ("ShipmentNumber", "OriginatorClientId", "ScheduledPickupDate", "ActualPickupDate", "ShipmentDate", "ReceivedDate", "Status", "TrackingNumber", "Carrier", "Weight", "WeightUnit", "NumberOfBoxes", "EstimatedValue", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
VALUES 
('SH202209001', 1, NOW() - INTERVAL '24 months' + INTERVAL '1 day', NOW() - INTERVAL '24 months' + INTERVAL '2 days', NOW() - INTERVAL '24 months', NOW() - INTERVAL '24 months' + INTERVAL '3 days', 'Received', 'TRK123456', 'FedEx', 125.5, 'kg', 3, 2500.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '24 months', NOW() - INTERVAL '24 months'),
('SH202209002', 2, NOW() - INTERVAL '24 months' + INTERVAL '3 days', NOW() - INTERVAL '24 months' + INTERVAL '4 days', NOW() - INTERVAL '24 months' + INTERVAL '2 days', NOW() - INTERVAL '24 months' + INTERVAL '5 days', 'Received', 'TRK123457', 'UPS', 89.2, 'kg', 2, 1800.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '24 months' + INTERVAL '2 days', NOW() - INTERVAL '24 months' + INTERVAL '2 days'),
('SH202209003', 3, NOW() - INTERVAL '24 months' + INTERVAL '5 days', NOW() - INTERVAL '24 months' + INTERVAL '6 days', NOW() - INTERVAL '24 months' + INTERVAL '4 days', NOW() - INTERVAL '24 months' + INTERVAL '7 days', 'Received', 'TRK123458', 'DHL', 156.8, 'kg', 4, 3200.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '24 months' + INTERVAL '4 days', NOW() - INTERVAL '24 months' + INTERVAL '4 days'),
('SH202209004', 1, NOW() - INTERVAL '24 months' + INTERVAL '8 days', NOW() - INTERVAL '24 months' + INTERVAL '9 days', NOW() - INTERVAL '24 months' + INTERVAL '7 days', NOW() - INTERVAL '24 months' + INTERVAL '10 days', 'Received', 'TRK123459', 'Local Courier', 203.4, 'kg', 5, 4100.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '24 months' + INTERVAL '7 days', NOW() - INTERVAL '24 months' + INTERVAL '7 days'),
('SH202209005', 4, NOW() - INTERVAL '24 months' + INTERVAL '12 days', NOW() - INTERVAL '24 months' + INTERVAL '13 days', NOW() - INTERVAL '24 months' + INTERVAL '11 days', NOW() - INTERVAL '24 months' + INTERVAL '14 days', 'Received', 'TRK123460', 'FedEx', 78.9, 'kg', 2, 1600.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '24 months' + INTERVAL '11 days', NOW() - INTERVAL '24 months' + INTERVAL '11 days'),
('SH202209006', 5, NOW() - INTERVAL '24 months' + INTERVAL '15 days', NOW() - INTERVAL '24 months' + INTERVAL '16 days', NOW() - INTERVAL '24 months' + INTERVAL '14 days', NOW() - INTERVAL '24 months' + INTERVAL '17 days', 'Received', 'TRK123461', 'UPS', 134.7, 'kg', 3, 2700.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '24 months' + INTERVAL '14 days', NOW() - INTERVAL '24 months' + INTERVAL '14 days'),
('SH202209007', 2, NOW() - INTERVAL '24 months' + INTERVAL '18 days', NOW() - INTERVAL '24 months' + INTERVAL '19 days', NOW() - INTERVAL '24 months' + INTERVAL '17 days', NOW() - INTERVAL '24 months' + INTERVAL '20 days', 'Received', 'TRK123462', 'DHL', 167.3, 'kg', 4, 3400.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '24 months' + INTERVAL '17 days', NOW() - INTERVAL '24 months' + INTERVAL '17 days'),
('SH202209008', 3, NOW() - INTERVAL '24 months' + INTERVAL '21 days', NOW() - INTERVAL '24 months' + INTERVAL '22 days', NOW() - INTERVAL '24 months' + INTERVAL '20 days', NOW() - INTERVAL '24 months' + INTERVAL '23 days', 'Received', 'TRK123463', 'Local Courier', 92.1, 'kg', 2, 1900.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '24 months' + INTERVAL '20 days', NOW() - INTERVAL '24 months' + INTERVAL '20 days'),
('SH202209009', 1, NOW() - INTERVAL '24 months' + INTERVAL '24 days', NOW() - INTERVAL '24 months' + INTERVAL '25 days', NOW() - INTERVAL '24 months' + INTERVAL '23 days', NOW() - INTERVAL '24 months' + INTERVAL '26 days', 'Received', 'TRK123464', 'FedEx', 189.6, 'kg', 5, 3800.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '24 months' + INTERVAL '23 days', NOW() - INTERVAL '24 months' + INTERVAL '23 days'),
('SH202209010', 4, NOW() - INTERVAL '24 months' + INTERVAL '27 days', NOW() - INTERVAL '24 months' + INTERVAL '28 days', NOW() - INTERVAL '24 months' + INTERVAL '26 days', NOW() - INTERVAL '24 months' + INTERVAL '29 days', 'Received', 'TRK123465', 'UPS', 112.8, 'kg', 3, 2300.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '24 months' + INTERVAL '26 days', NOW() - INTERVAL '24 months' + INTERVAL '26 days');

INSERT INTO "Shipments" ("ShipmentNumber", "OriginatorClientId", "ScheduledPickupDate", "ActualPickupDate", "ShipmentDate", "ReceivedDate", "Status", "TrackingNumber", "Carrier", "Weight", "WeightUnit", "NumberOfBoxes", "EstimatedValue", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
VALUES 
('SH202210001', 2, NOW() - INTERVAL '23 months' + INTERVAL '1 day', NOW() - INTERVAL '23 months' + INTERVAL '2 days', NOW() - INTERVAL '23 months', NOW() - INTERVAL '23 months' + INTERVAL '3 days', 'Received', 'TRK223456', 'FedEx', 145.2, 'kg', 4, 2900.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '23 months', NOW() - INTERVAL '23 months'),
('SH202210002', 1, NOW() - INTERVAL '23 months' + INTERVAL '4 days', NOW() - INTERVAL '23 months' + INTERVAL '5 days', NOW() - INTERVAL '23 months' + INTERVAL '3 days', NOW() - INTERVAL '23 months' + INTERVAL '6 days', 'Received', 'TRK223457', 'UPS', 98.7, 'kg', 2, 2000.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '23 months' + INTERVAL '3 days', NOW() - INTERVAL '23 months' + INTERVAL '3 days'),
('SH202210003', 5, NOW() - INTERVAL '23 months' + INTERVAL '7 days', NOW() - INTERVAL '23 months' + INTERVAL '8 days', NOW() - INTERVAL '23 months' + INTERVAL '6 days', NOW() - INTERVAL '23 months' + INTERVAL '9 days', 'Received', 'TRK223458', 'DHL', 176.4, 'kg', 5, 3500.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '23 months' + INTERVAL '6 days', NOW() - INTERVAL '23 months' + INTERVAL '6 days'),
('SH202210004', 3, NOW() - INTERVAL '23 months' + INTERVAL '10 days', NOW() - INTERVAL '23 months' + INTERVAL '11 days', NOW() - INTERVAL '23 months' + INTERVAL '9 days', NOW() - INTERVAL '23 months' + INTERVAL '12 days', 'Received', 'TRK223459', 'Local Courier', 87.3, 'kg', 2, 1750.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '23 months' + INTERVAL '9 days', NOW() - INTERVAL '23 months' + INTERVAL '9 days'),
('SH202210005', 4, NOW() - INTERVAL '23 months' + INTERVAL '13 days', NOW() - INTERVAL '23 months' + INTERVAL '14 days', NOW() - INTERVAL '23 months' + INTERVAL '12 days', NOW() - INTERVAL '23 months' + INTERVAL '15 days', 'Received', 'TRK223460', 'FedEx', 198.1, 'kg', 6, 4000.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '23 months' + INTERVAL '12 days', NOW() - INTERVAL '23 months' + INTERVAL '12 days'),
('SH202210006', 1, NOW() - INTERVAL '23 months' + INTERVAL '16 days', NOW() - INTERVAL '23 months' + INTERVAL '17 days', NOW() - INTERVAL '23 months' + INTERVAL '15 days', NOW() - INTERVAL '23 months' + INTERVAL '18 days', 'Received', 'TRK223461', 'UPS', 123.9, 'kg', 3, 2500.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '23 months' + INTERVAL '15 days', NOW() - INTERVAL '23 months' + INTERVAL '15 days'),
('SH202210007', 2, NOW() - INTERVAL '23 months' + INTERVAL '19 days', NOW() - INTERVAL '23 months' + INTERVAL '20 days', NOW() - INTERVAL '23 months' + INTERVAL '18 days', NOW() - INTERVAL '23 months' + INTERVAL '21 days', 'Received', 'TRK223462', 'DHL', 154.6, 'kg', 4, 3100.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '23 months' + INTERVAL '18 days', NOW() - INTERVAL '23 months' + INTERVAL '18 days'),
('SH202210008', 5, NOW() - INTERVAL '23 months' + INTERVAL '22 days', NOW() - INTERVAL '23 months' + INTERVAL '23 days', NOW() - INTERVAL '23 months' + INTERVAL '21 days', NOW() - INTERVAL '23 months' + INTERVAL '24 days', 'Received', 'TRK223463', 'Local Courier', 109.2, 'kg', 3, 2200.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '23 months' + INTERVAL '21 days', NOW() - INTERVAL '23 months' + INTERVAL '21 days'),
('SH202210009', 3, NOW() - INTERVAL '23 months' + INTERVAL '25 days', NOW() - INTERVAL '23 months' + INTERVAL '26 days', NOW() - INTERVAL '23 months' + INTERVAL '24 days', NOW() - INTERVAL '23 months' + INTERVAL '27 days', 'Received', 'TRK223464', 'FedEx', 187.8, 'kg', 5, 3800.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '23 months' + INTERVAL '24 days', NOW() - INTERVAL '23 months' + INTERVAL '24 days'),
('SH202210010', 4, NOW() - INTERVAL '23 months' + INTERVAL '28 days', NOW() - INTERVAL '23 months' + INTERVAL '29 days', NOW() - INTERVAL '23 months' + INTERVAL '27 days', NOW() - INTERVAL '23 months' + INTERVAL '30 days', 'Received', 'TRK223465', 'UPS', 134.5, 'kg', 4, 2700.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '23 months' + INTERVAL '27 days', NOW() - INTERVAL '23 months' + INTERVAL '27 days');

INSERT INTO "Shipments" ("ShipmentNumber", "OriginatorClientId", "ScheduledPickupDate", "ActualPickupDate", "ShipmentDate", "ReceivedDate", "Status", "TrackingNumber", "Carrier", "Weight", "WeightUnit", "NumberOfBoxes", "EstimatedValue", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
VALUES 
('SH202408001', 1, NOW() - INTERVAL '1 month' + INTERVAL '1 day', NOW() - INTERVAL '1 month' + INTERVAL '2 days', NOW() - INTERVAL '1 month', NOW() - INTERVAL '1 month' + INTERVAL '3 days', 'Received', 'TRK823456', 'FedEx', 165.3, 'kg', 4, 3300.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '1 month', NOW() - INTERVAL '1 month'),
('SH202408002', 3, NOW() - INTERVAL '1 month' + INTERVAL '4 days', NOW() - INTERVAL '1 month' + INTERVAL '5 days', NOW() - INTERVAL '1 month' + INTERVAL '3 days', NOW() - INTERVAL '1 month' + INTERVAL '6 days', 'Received', 'TRK823457', 'UPS', 142.8, 'kg', 3, 2900.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '1 month' + INTERVAL '3 days', NOW() - INTERVAL '1 month' + INTERVAL '3 days'),
('SH202408003', 2, NOW() - INTERVAL '1 month' + INTERVAL '7 days', NOW() - INTERVAL '1 month' + INTERVAL '8 days', NOW() - INTERVAL '1 month' + INTERVAL '6 days', NOW() - INTERVAL '1 month' + INTERVAL '9 days', 'Received', 'TRK823458', 'DHL', 198.7, 'kg', 5, 4000.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '1 month' + INTERVAL '6 days', NOW() - INTERVAL '1 month' + INTERVAL '6 days'),
('SH202408004', 5, NOW() - INTERVAL '1 month' + INTERVAL '10 days', NOW() - INTERVAL '1 month' + INTERVAL '11 days', NOW() - INTERVAL '1 month' + INTERVAL '9 days', NOW() - INTERVAL '1 month' + INTERVAL '12 days', 'Received', 'TRK823459', 'Local Courier', 123.4, 'kg', 3, 2500.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '1 month' + INTERVAL '9 days', NOW() - INTERVAL '1 month' + INTERVAL '9 days'),
('SH202408005', 4, NOW() - INTERVAL '1 month' + INTERVAL '13 days', NOW() - INTERVAL '1 month' + INTERVAL '14 days', NOW() - INTERVAL '1 month' + INTERVAL '12 days', NOW() - INTERVAL '1 month' + INTERVAL '15 days', 'Received', 'TRK823460', 'FedEx', 176.9, 'kg', 4, 3500.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '1 month' + INTERVAL '12 days', NOW() - INTERVAL '1 month' + INTERVAL '12 days'),
('SH202408006', 1, NOW() - INTERVAL '1 month' + INTERVAL '16 days', NOW() - INTERVAL '1 month' + INTERVAL '17 days', NOW() - INTERVAL '1 month' + INTERVAL '15 days', NOW() - INTERVAL '1 month' + INTERVAL '18 days', 'Received', 'TRK823461', 'UPS', 154.2, 'kg', 4, 3100.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '1 month' + INTERVAL '15 days', NOW() - INTERVAL '1 month' + INTERVAL '15 days'),
('SH202408007', 3, NOW() - INTERVAL '1 month' + INTERVAL '19 days', NOW() - INTERVAL '1 month' + INTERVAL '20 days', NOW() - INTERVAL '1 month' + INTERVAL '18 days', NOW() - INTERVAL '1 month' + INTERVAL '21 days', 'Received', 'TRK823462', 'DHL', 189.5, 'kg', 5, 3800.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '1 month' + INTERVAL '18 days', NOW() - INTERVAL '1 month' + INTERVAL '18 days'),
('SH202408008', 2, NOW() - INTERVAL '1 month' + INTERVAL '22 days', NOW() - INTERVAL '1 month' + INTERVAL '23 days', NOW() - INTERVAL '1 month' + INTERVAL '21 days', NOW() - INTERVAL '1 month' + INTERVAL '24 days', 'Received', 'TRK823463', 'Local Courier', 134.7, 'kg', 3, 2700.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '1 month' + INTERVAL '21 days', NOW() - INTERVAL '1 month' + INTERVAL '21 days'),
('SH202408009', 5, NOW() - INTERVAL '1 month' + INTERVAL '25 days', NOW() - INTERVAL '1 month' + INTERVAL '26 days', NOW() - INTERVAL '1 month' + INTERVAL '24 days', NOW() - INTERVAL '1 month' + INTERVAL '27 days', 'Received', 'TRK823464', 'FedEx', 167.1, 'kg', 4, 3400.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '1 month' + INTERVAL '24 days', NOW() - INTERVAL '1 month' + INTERVAL '24 days'),
('SH202408010', 4, NOW() - INTERVAL '1 month' + INTERVAL '28 days', NOW() - INTERVAL '1 month' + INTERVAL '29 days', NOW() - INTERVAL '1 month' + INTERVAL '27 days', NOW() - INTERVAL '1 month' + INTERVAL '30 days', 'Received', 'TRK823465', 'UPS', 145.8, 'kg', 4, 2900.00, 'ADMIN_CLIENT_001', 1, 1, NOW() - INTERVAL '1 month' + INTERVAL '27 days', NOW() - INTERVAL '1 month' + INTERVAL '27 days');


INSERT INTO "ShipmentItems" ("ShipmentId", "MaterialTypeId", "Description", "Category", "Brand", "Quantity", "UnitOfMeasure", "Condition", "EstimatedValue", "Weight", "WeightUnit", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
SELECT 
    s."Id",
    mt."Id",
    mt."Name" || ' items for recycling',
    mt."Name",
    CASE (s."Id" % 6) 
        WHEN 0 THEN 'Dell'
        WHEN 1 THEN 'HP' 
        WHEN 2 THEN 'Lenovo'
        WHEN 3 THEN 'Apple'
        WHEN 4 THEN 'Samsung'
        ELSE 'Generic'
    END,
    (s."Id" % 15) + 5, -- Quantity between 5-19
    'pieces',
    CASE (s."Id" % 4)
        WHEN 0 THEN 'Working'
        WHEN 1 THEN 'Non-Working'
        WHEN 2 THEN 'Damaged'
        ELSE 'Unknown'
    END,
    (s."Id" % 180) + 20, -- EstimatedValue between 20-199
    (s."Id" % 45) + 5, -- Weight between 5-49
    'kg',
    s."ClientId",
    1,
    1,
    s."DateCreated",
    s."DateCreated"
FROM "Shipments" s
CROSS JOIN "MaterialTypes" mt
WHERE s."ShipmentNumber" LIKE 'SH2022%' 
AND mt."ClientId" = 'ADMIN_CLIENT_001'
AND (s."Id" + mt."Id") % 3 = 0; -- Only create items for some material types per shipment

INSERT INTO "ShipmentItems" ("ShipmentId", "MaterialTypeId", "Description", "Category", "Brand", "Quantity", "UnitOfMeasure", "Condition", "EstimatedValue", "Weight", "WeightUnit", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
SELECT 
    s."Id",
    mt."Id",
    mt."Name" || ' items for recycling',
    mt."Name",
    CASE ((s."Id" + 1) % 6) 
        WHEN 0 THEN 'Dell'
        WHEN 1 THEN 'HP' 
        WHEN 2 THEN 'Lenovo'
        WHEN 3 THEN 'Apple'
        WHEN 4 THEN 'Samsung'
        ELSE 'Generic'
    END,
    ((s."Id" + 2) % 15) + 5, -- Quantity between 5-19
    'pieces',
    CASE ((s."Id" + 1) % 4)
        WHEN 0 THEN 'Working'
        WHEN 1 THEN 'Non-Working'
        WHEN 2 THEN 'Damaged'
        ELSE 'Unknown'
    END,
    ((s."Id" + 3) % 180) + 20, -- EstimatedValue between 20-199
    ((s."Id" + 1) % 45) + 5, -- Weight between 5-49
    'kg',
    s."ClientId",
    1,
    1,
    s."DateCreated",
    s."DateCreated"
FROM "Shipments" s
CROSS JOIN "MaterialTypes" mt
WHERE s."ShipmentNumber" LIKE 'SH202210%' 
AND mt."ClientId" = 'ADMIN_CLIENT_001'
AND (s."Id" + mt."Id") % 3 = 1; -- Different pattern for variety

INSERT INTO "ShipmentItems" ("ShipmentId", "MaterialTypeId", "Description", "Category", "Brand", "Quantity", "UnitOfMeasure", "Condition", "EstimatedValue", "Weight", "WeightUnit", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
SELECT 
    s."Id",
    mt."Id",
    mt."Name" || ' items for recycling',
    mt."Name",
    CASE ((s."Id" + 2) % 6) 
        WHEN 0 THEN 'Dell'
        WHEN 1 THEN 'HP' 
        WHEN 2 THEN 'Lenovo'
        WHEN 3 THEN 'Apple'
        WHEN 4 THEN 'Samsung'
        ELSE 'Generic'
    END,
    ((s."Id" + 4) % 15) + 5, -- Quantity between 5-19
    'pieces',
    CASE ((s."Id" + 2) % 4)
        WHEN 0 THEN 'Working'
        WHEN 1 THEN 'Non-Working'
        WHEN 2 THEN 'Damaged'
        ELSE 'Unknown'
    END,
    ((s."Id" + 5) % 180) + 20, -- EstimatedValue between 20-199
    ((s."Id" + 2) % 45) + 5, -- Weight between 5-49
    'kg',
    s."ClientId",
    1,
    1,
    s."DateCreated",
    s."DateCreated"
FROM "Shipments" s
CROSS JOIN "MaterialTypes" mt
WHERE s."ShipmentNumber" LIKE 'SH202408%' 
AND mt."ClientId" = 'ADMIN_CLIENT_001'
AND (s."Id" + mt."Id") % 3 = 2; -- Different pattern for variety

INSERT INTO "ShipmentItems" ("ShipmentId", "MaterialTypeId", "Description", "Category", "Brand", "Quantity", "UnitOfMeasure", "Condition", "EstimatedValue", "Weight", "WeightUnit", "ClientId", "CreatedBy", "UpdatedBy", "DateCreated", "DateUpdated")
SELECT 
    s."Id",
    mt."Id",
    mt."Name" || ' additional items',
    mt."Name",
    'Mixed Brand',
    ((s."Id" * 2) % 12) + 3, -- Quantity between 3-14
    'pieces',
    'Working',
    ((s."Id" * 3) % 150) + 30, -- EstimatedValue between 30-179
    ((s."Id" * 2) % 35) + 10, -- Weight between 10-44
    'kg',
    s."ClientId",
    1,
    1,
    s."DateCreated",
    s."DateCreated"
FROM "Shipments" s
CROSS JOIN "MaterialTypes" mt
WHERE s."ClientId" = 'ADMIN_CLIENT_001'
AND mt."ClientId" = 'ADMIN_CLIENT_001'
AND mt."Name" IN ('Electronics', 'Laptops', 'Monitors') -- Focus on key material types
AND s."Id" % 2 = 0; -- Only for even shipment IDs to avoid too much data

COMMIT;
