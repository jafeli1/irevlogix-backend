BEGIN;

SELECT setval('"Shipments_Id_seq"', 1, false);
SELECT setval('"ProcessedMaterials_Id_seq"', 1, false);


INSERT INTO "Shipments" ("ShipmentNumber", "ShipmentDate", "Status", "TrackingNumber", "Carrier", "Weight", "WeightUnit", "NumberOfBoxes", "EstimatedValue", "PickupAddress", "Notes", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") VALUES
('SHP-2024-001', '2024-08-01', 'Delivered', 'FX123456789', 'FedEx', 150.5, 'lbs', 3, 25000.00, '100 Corporate Dr, Austin, TX 78701', 'IT equipment refresh', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
('SHP-2024-002', '2024-08-05', 'Delivered', 'UPS987654321', 'UPS', 200.3, 'lbs', 8, 35000.00, '200 Business Blvd, Houston, TX 77001', 'Server decommission', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
('SHP-2024-003', '2024-08-10', 'Delivered', 'DHL456789123', 'DHL', 175.8, 'lbs', 2, 28000.00, '300 Tech Way, Dallas, TX 75201', 'Laptop replacement program', NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
('SHP-2024-004', '2024-08-15', 'Delivered', 'FX789123456', 'FedEx', 120.2, 'lbs', 2, 18000.00, '400 Innovation St, San Antonio, TX 78201', 'Network equipment upgrade', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
('SHP-2024-005', '2024-08-20', 'Delivered', 'UPS321654987', 'UPS', 300.7, 'lbs', 4, 45000.00, '500 Digital Ave, Fort Worth, TX 76101', 'Data center consolidation', NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
('SHP-2024-006', '2024-08-25', 'Delivered', 'FX555666777', 'FedEx', 180.4, 'lbs', 6, 22000.00, '600 Tech Plaza, Austin, TX 78702', 'Office equipment disposal', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
('SHP-2024-007', '2024-08-30', 'Delivered', 'UPS888999000', 'UPS', 220.1, 'lbs', 3, 38000.00, '700 Innovation Dr, Houston, TX 77002', 'Manufacturing IT refresh', NOW(), NOW(), 6, 6, 'ADMIN_CLIENT_001'),
('SHP-2024-008', '2024-09-05', 'Delivered', 'DHL111222333', 'DHL', 195.6, 'lbs', 4, 32000.00, '800 Business Park, Dallas, TX 75202', 'R&D lab equipment', NOW(), NOW(), 7, 7, 'ADMIN_CLIENT_001'),
('SHP-2024-009', '2024-09-10', 'Delivered', 'FX444555666', 'FedEx', 165.3, 'lbs', 5, 26000.00, '900 Enterprise Way, San Antonio, TX 78202', 'Testing equipment refresh', NOW(), NOW(), 8, 8, 'ADMIN_CLIENT_001'),
('SHP-2024-010', '2024-09-15', 'Delivered', 'UPS777888999', 'UPS', 275.9, 'lbs', 7, 42000.00, '1000 Development Ave, Fort Worth, TX 76102', 'Cloud migration hardware', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
('SHP-2024-101', '2024-08-02', 'Delivered', 'FX111222333', 'FedEx', 180.4, 'lbs', 4, 30000.00, '1000 Enterprise Way, Phoenix, AZ 85001', 'Office relocation equipment', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('SHP-2024-102', '2024-08-06', 'Delivered', 'UPS444555666', 'UPS', 220.1, 'lbs', 9, 38000.00, '2000 Innovation Dr, Tucson, AZ 85701', 'Manufacturing equipment disposal', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('SHP-2024-103', '2024-08-11', 'Delivered', 'DHL777888999', 'DHL', 195.6, 'lbs', 3, 32000.00, '3000 Tech Center Blvd, Mesa, AZ 85201', 'R&D lab equipment', NOW(), NOW(), 10, 10, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('SHP-2024-104', '2024-08-16', 'Delivered', 'FX000111222', 'FedEx', 165.3, 'lbs', 2, 26000.00, '4000 Research Pkwy, Scottsdale, AZ 85251', 'Testing equipment refresh', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('SHP-2024-105', '2024-08-21', 'Delivered', 'UPS333444555', 'UPS', 275.9, 'lbs', 5, 42000.00, '5000 Development Ave, Chandler, AZ 85224', 'Cloud migration hardware', NOW(), NOW(), 10, 10, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('SHP-2024-106', '2024-08-26', 'Delivered', 'FX666777888', 'FedEx', 190.7, 'lbs', 7, 28000.00, '6000 Tech Plaza, Phoenix, AZ 85002', 'Office equipment disposal', NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('SHP-2024-107', '2024-08-31', 'Delivered', 'UPS999000111', 'UPS', 210.3, 'lbs', 4, 35000.00, '7000 Innovation Dr, Tucson, AZ 85702', 'Manufacturing IT refresh', NOW(), NOW(), 12, 12, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('SHP-2024-108', '2024-09-06', 'Delivered', 'DHL222333444', 'DHL', 185.2, 'lbs', 6, 29000.00, '8000 Business Park, Mesa, AZ 85202', 'R&D lab equipment', NOW(), NOW(), 13, 13, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('SHP-2024-109', '2024-09-11', 'Delivered', 'FX555666777', 'FedEx', 175.8, 'lbs', 5, 27000.00, '9000 Enterprise Way, Scottsdale, AZ 85252', 'Testing equipment refresh', NOW(), NOW(), 14, 14, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('SHP-2024-110', '2024-09-16', 'Delivered', 'UPS888999000', 'UPS', 285.4, 'lbs', 8, 44000.00, '10000 Development Ave, Chandler, AZ 85225', 'Cloud migration hardware', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4');


INSERT INTO "ShipmentItems" ("ShipmentId", "AssetCategoryId", "Description", "SerialNumber", "Brand", "Model", "Condition", "EstimatedValue", "Quantity", "Weight", "IsDataBearingDevice", "Notes", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") VALUES
(3, 2, 'Dell Laptop', 'DL123456789', 'Dell', 'Latitude 7420', 'Functional', 800.00, 1, 2.5, true, 'Corporate laptop with SSD', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
(3, 2, 'HP Laptop', 'HP987654321', 'HP', 'EliteBook 840', 'Functional', 750.00, 1, 2.3, true, 'Executive laptop', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
(3, 44, 'Dell Monitor', 'DM111222333', 'Dell', 'UltraSharp U2720Q', 'Functional', 400.00, 1, 8.2, false, '27-inch 4K monitor', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
(3, 23, 'Cisco Switch', 'CS444555666', 'Cisco', 'Catalyst 2960', 'Functional', 500.00, 1, 3.1, false, '24-port managed switch', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
(3, 30, 'iPhone 12', 'IP777888999', 'Apple', 'iPhone 12 Pro', 'Functional', 600.00, 1, 0.2, true, 'Company phone', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
(4, 16, 'Dell Server', 'DS123456789', 'Dell', 'PowerEdge R740', 'Functional', 3500.00, 1, 25.0, true, 'Production server with HDDs', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
(4, 16, 'HP Server', 'HS987654321', 'HP', 'ProLiant DL380', 'Functional', 3200.00, 1, 23.5, true, 'Database server', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
(4, 37, 'Storage Array', 'SA111222333', 'NetApp', 'FAS2750', 'Functional', 8000.00, 1, 45.0, true, 'Primary storage system', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
(4, 23, 'Core Switch', 'CS2444555666', 'Cisco', 'Catalyst 9300', 'Functional', 2500.00, 1, 8.5, false, 'Core network switch', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
(4, 58, 'UPS Unit', 'UPS777888999', 'APC', 'Smart-UPS 3000', 'Functional', 1200.00, 1, 35.0, false, 'Backup power supply', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
(5, 2, 'Lenovo Laptop', 'LN123456789', 'Lenovo', 'ThinkPad X1 Carbon', 'Functional', 900.00, 1, 2.4, true, 'Executive laptop', NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
(5, 2, 'MacBook Pro', 'MB987654321', 'Apple', 'MacBook Pro 16"', 'Functional', 2200.00, 1, 2.0, true, 'Design workstation', NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
(5, 44, 'LG Monitor', 'LG111222333', 'LG', '34WN80C-B', 'Functional', 450.00, 1, 9.1, false, 'Ultrawide monitor', NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
(5, 30, 'Samsung Tablet', 'ST444555666', 'Samsung', 'Galaxy Tab S7', 'Functional', 400.00, 1, 0.5, true, 'Field tablet', NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
(5, 51, 'Printer', 'PR777888999', 'HP', 'LaserJet Pro M404n', 'Functional', 200.00, 1, 12.0, false, 'Office printer', NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
(13, 59, 'Dell Laptop', 'DL2123456789', 'Dell', 'Latitude 5520', 'Functional', 850.00, 1, 2.6, true, 'Standard laptop', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(13, 59, 'HP Laptop', 'HP2987654321', 'HP', 'ProBook 450', 'Functional', 700.00, 1, 2.4, true, 'Business laptop', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(13, 65, 'ASUS Monitor', 'AS2111222333', 'ASUS', 'PA278QV', 'Functional', 350.00, 1, 7.8, false, '27-inch monitor', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(13, 62, 'Netgear Switch', 'NG2444555666', 'Netgear', 'GS724T', 'Functional', 300.00, 1, 2.8, false, '24-port switch', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(13, 63, 'iPad', 'IP2777888999', 'Apple', 'iPad Air', 'Functional', 500.00, 1, 0.5, true, 'Executive tablet', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(14, 61, 'Dell Server', 'DS2123456789', 'Dell', 'PowerEdge R750', 'Functional', 4000.00, 1, 26.0, true, 'Manufacturing server', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(14, 61, 'HP Server', 'HS2987654321', 'HP', 'ProLiant DL385', 'Functional', 3800.00, 1, 24.5, true, 'Application server', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(14, 64, 'Storage Array', 'SA2111222333', 'EMC', 'Unity 400', 'Functional', 9000.00, 1, 48.0, true, 'Manufacturing storage', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(14, 62, 'Enterprise Switch', 'ES2444555666', 'Juniper', 'EX4300-48T', 'Functional', 3500.00, 1, 9.2, false, 'Core enterprise switch', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(14, 67, 'UPS Unit', 'UPS2777888999', 'Eaton', '9PX 6000i', 'Functional', 2000.00, 1, 38.0, false, 'High-capacity UPS', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4');


INSERT INTO "ProcessingLots" ("LotNumber", "Description", "StartDate", "CompletionDate", "Status", "OperatorUserId", "ExpectedRevenue", "ActualRevenue", "ProcessingCost", "ProcessingNotes", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") VALUES
('LOT-20240801-001', 'Corporate Laptop Refresh Batch', '2024-08-04', '2024-08-10', 'Completed', 5, 15000.00, 12500.00, 2000.00, 'High-value laptop processing', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
('LOT-20240808-002', 'Server Decommission Lot', '2024-08-08', '2024-08-15', 'Completed', 6, 25000.00, 22000.00, 3500.00, 'Data center equipment', NOW(), NOW(), 6, 6, 'ADMIN_CLIENT_001'),
('LOT-20240815-003', 'Network Equipment Batch', '2024-08-18', '2024-08-25', 'Completed', 5, 8000.00, 7200.00, 1200.00, 'Switches and routers', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
('LOT-20240822-004', 'Mobile Device Lot', '2024-08-25', '2024-09-01', 'Completed', 7, 5000.00, 4200.00, 800.00, 'Phones and tablets', NOW(), NOW(), 7, 7, 'ADMIN_CLIENT_001'),
('LOT-20240901-005', 'Mixed IT Equipment', '2024-09-03', '2024-09-10', 'In Progress', 8, 18000.00, NULL, NULL, 'Various equipment types', NOW(), NOW(), 8, 8, 'ADMIN_CLIENT_001'),
('LOT-20240802-101', 'Office Relocation Equipment', '2024-08-05', '2024-08-12', 'Completed', 11, 16000.00, 13800.00, 2200.00, 'Office move equipment', NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('LOT-20240809-102', 'Manufacturing IT Refresh', '2024-08-12', '2024-08-19', 'Completed', 12, 22000.00, 19500.00, 3000.00, 'Manufacturing equipment', NOW(), NOW(), 12, 12, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('LOT-20240816-103', 'R&D Lab Equipment', '2024-08-19', '2024-08-26', 'Completed', 13, 12000.00, 10800.00, 1800.00, 'Research lab equipment', NOW(), NOW(), 13, 13, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('LOT-20240823-104', 'Testing Equipment Batch', '2024-08-26', '2024-09-02', 'Completed', 14, 9000.00, 8100.00, 1400.00, 'QA testing equipment', NOW(), NOW(), 14, 14, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('LOT-20240902-105', 'Cloud Migration Hardware', '2024-09-04', NULL, 'In Progress', 11, 20000.00, NULL, NULL, 'Cloud transition equipment', NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4');


INSERT INTO "Assets" ("AssetID", "SerialNumber", "Manufacturer", "Model", "AssetCategoryId", "CurrentStatusId", "Condition", "IsDataBearing", "DataSanitizationStatus", "DataSanitizationMethod", "EstimatedValue", "ActualSalePrice", "ProcessingLotId", "SourceShipmentId", "CurrentLocation", "Notes", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") VALUES
('AST-001-001', 'DL123456789', 'Dell', 'Latitude 7420', 2, 5, 'Functional', true, 'Completed', 'DoD 5220.22-M', 800.00, 750.00, 1, 3, 'Warehouse A-1', 'Corporate laptop - data wiped', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
('AST-001-002', 'HP987654321', 'HP', 'EliteBook 840', 2, 5, 'Functional', true, 'Completed', 'NIST 800-88', 750.00, 700.00, 1, 3, 'Warehouse A-1', 'Executive laptop - data wiped', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
('AST-001-003', 'DM111222333', 'Dell', 'UltraSharp U2720Q', 44, 4, 'Functional', false, 'Not Required', NULL, 400.00, 380.00, 1, 3, 'Warehouse A-2', '27-inch 4K monitor', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
('AST-001-004', 'CS444555666', 'Cisco', 'Catalyst 2960', 23, 4, 'Functional', false, 'Not Required', NULL, 500.00, 450.00, 3, 3, 'Warehouse A-3', '24-port managed switch', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
('AST-001-005', 'IP777888999', 'Apple', 'iPhone 12 Pro', 30, 5, 'Functional', true, 'Completed', 'Factory Reset + Encryption', 600.00, 550.00, 4, 3, 'Warehouse A-4', 'Company phone - factory reset', NOW(), NOW(), 7, 7, 'ADMIN_CLIENT_001'),
('AST-001-006', 'DS123456789', 'Dell', 'PowerEdge R740', 16, 5, 'Functional', true, 'Completed', 'NIST 800-88', 3500.00, 3200.00, 2, 4, 'Warehouse B-1', 'Production server - drives destroyed', NOW(), NOW(), 6, 6, 'ADMIN_CLIENT_001'),
('AST-001-007', 'HS987654321', 'HP', 'ProLiant DL380', 16, 5, 'Functional', true, 'Completed', 'DoD 5220.22-M', 3200.00, 2900.00, 2, 4, 'Warehouse B-1', 'Database server - drives destroyed', NOW(), NOW(), 6, 6, 'ADMIN_CLIENT_001'),
('AST-001-008', 'SA111222333', 'NetApp', 'FAS2750', 37, 5, 'Functional', true, 'Completed', 'Cryptographic Erase', 8000.00, 7500.00, 2, 4, 'Warehouse B-2', 'Primary storage - crypto erase', NOW(), NOW(), 6, 6, 'ADMIN_CLIENT_001'),
('AST-001-009', 'LN123456789', 'Lenovo', 'ThinkPad X1 Carbon', 2, 4, 'Functional', true, 'In Progress', NULL, 900.00, NULL, 5, 5, 'Processing Area', 'Executive laptop - processing', NOW(), NOW(), 8, 8, 'ADMIN_CLIENT_001'),
('AST-001-010', 'MB987654321', 'Apple', 'MacBook Pro 16"', 2, 4, 'Functional', true, 'In Progress', NULL, 2200.00, NULL, 5, 5, 'Processing Area', 'Design workstation - processing', NOW(), NOW(), 8, 8, 'ADMIN_CLIENT_001'),
('AST-001-011', 'LG111222333', 'LG', '34WN80C-B', 44, 3, 'Functional', false, 'Not Required', NULL, 450.00, NULL, NULL, 5, 'Receiving', 'Ultrawide monitor - received', NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
('AST-001-012', 'ST444555666', 'Samsung', 'Galaxy Tab S7', 30, 3, 'Functional', true, 'Pending', NULL, 400.00, NULL, NULL, 5, 'Receiving', 'Field tablet - received', NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
('AST-001-013', 'PR777888999', 'HP', 'LaserJet Pro M404n', 51, 3, 'Functional', false, 'Not Required', NULL, 200.00, NULL, NULL, 5, 'Receiving', 'Office printer - received', NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
('AST-001-014', 'UPS777888999', 'APC', 'Smart-UPS 3000', 58, 4, 'Functional', false, 'Not Required', NULL, 1200.00, 1100.00, 3, 12, 'Warehouse A-5', 'Backup power supply', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
('AST-001-015', 'CS2444555666', 'Cisco', 'Catalyst 9300', 23, 4, 'Functional', false, 'Not Required', NULL, 2500.00, 2300.00, 3, 4, 'Warehouse A-3', 'Core network switch', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
('AST-001-016', 'LAP016', 'Dell', 'Inspiron 15', 2, 2, 'Functional', true, 'Pending', NULL, 600.00, NULL, NULL, NULL, 'Processing Area', 'Standard laptop', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
('AST-001-017', 'MON017', 'Samsung', 'C27F390', 44, 1, 'Functional', false, 'Not Required', NULL, 250.00, NULL, NULL, NULL, 'Receiving', 'Curved monitor', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
('AST-001-018', 'TAB018', 'Apple', 'iPad Pro', 30, 1, 'Functional', true, 'Pending', NULL, 800.00, NULL, NULL, NULL, 'Receiving', 'Professional tablet', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
('AST-001-019', 'PRT019', 'Canon', 'PIXMA TR8620', 51, 2, 'Functional', false, 'Not Required', NULL, 150.00, NULL, NULL, NULL, 'Processing Area', 'All-in-one printer', NOW(), NOW(), 6, 6, 'ADMIN_CLIENT_001'),
('AST-001-020', 'NET020', 'Linksys', 'EA7500', 23, 1, 'Functional', false, 'Not Required', NULL, 180.00, NULL, NULL, NULL, 'Receiving', 'Wireless router', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
('AST-001-021', 'SRV021', 'IBM', 'System x3650', 16, 3, 'Functional', true, 'Pending', NULL, 2800.00, NULL, NULL, NULL, 'Receiving', 'Legacy server', NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
('AST-001-022', 'STG022', 'Synology', 'DS920+', 37, 2, 'Functional', true, 'Pending', NULL, 500.00, NULL, NULL, NULL, 'Processing Area', 'NAS storage', NOW(), NOW(), 7, 7, 'ADMIN_CLIENT_001'),
('AST-001-023', 'UPS023', 'CyberPower', 'CP1500PFCLCD', 58, 1, 'Functional', false, 'Not Required', NULL, 200.00, NULL, NULL, NULL, 'Receiving', 'Desktop UPS', NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
('AST-001-024', 'PHN024', 'Samsung', 'Galaxy S21', 30, 2, 'Functional', true, 'Pending', NULL, 400.00, NULL, NULL, NULL, 'Processing Area', 'Corporate phone', NOW(), NOW(), 8, 8, 'ADMIN_CLIENT_001'),
('AST-001-025', 'WKS025', 'HP', 'Z4 G4', 2, 3, 'Functional', true, 'Pending', NULL, 1800.00, NULL, NULL, NULL, 'Receiving', 'Engineering workstation', NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
('AST-002-001', 'DL2123456789', 'Dell', 'Latitude 5520', 59, 5, 'Functional', true, 'Completed', 'DoD 5220.22-M', 850.00, 800.00, 6, 13, 'Warehouse C-1', 'Standard laptop - data wiped', NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-002', 'HP2987654321', 'HP', 'ProBook 450', 59, 5, 'Functional', true, 'Completed', 'NIST 800-88', 700.00, 650.00, 6, 13, 'Warehouse C-1', 'Business laptop - data wiped', NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-003', 'AS2111222333', 'ASUS', 'PA278QV', 65, 4, 'Functional', false, 'Not Required', NULL, 350.00, 320.00, 6, 13, 'Warehouse C-2', '27-inch monitor', NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-004', 'NG2444555666', 'Netgear', 'GS724T', 62, 4, 'Functional', false, 'Not Required', NULL, 300.00, 280.00, 8, 13, 'Warehouse C-3', '24-port switch', NOW(), NOW(), 13, 13, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-005', 'IP2777888999', 'Apple', 'iPad Air', 63, 5, 'Functional', true, 'Completed', 'Factory Reset + Encryption', 500.00, 450.00, 9, 13, 'Warehouse C-4', 'Executive tablet - factory reset', NOW(), NOW(), 14, 14, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-006', 'DS2123456789', 'Dell', 'PowerEdge R750', 61, 5, 'Functional', true, 'Completed', 'NIST 800-88', 4000.00, 3700.00, 7, 14, 'Warehouse D-1', 'Manufacturing server', NOW(), NOW(), 12, 12, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-007', 'HS2987654321', 'HP', 'ProLiant DL385', 61, 5, 'Functional', true, 'Completed', 'DoD 5220.22-M', 3800.00, 3500.00, 7, 14, 'Warehouse D-1', 'Application server', NOW(), NOW(), 12, 12, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-008', 'SA2111222333', 'EMC', 'Unity 400', 64, 5, 'Functional', true, 'Completed', 'Cryptographic Erase', 9000.00, 8500.00, 7, 14, 'Warehouse D-2', 'Manufacturing storage', NOW(), NOW(), 12, 12, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-009', 'LAP2009', 'Lenovo', 'ThinkPad P1', 59, 4, 'Functional', true, 'In Progress', NULL, 1800.00, NULL, 10, NULL, 'Processing Area', 'Workstation laptop', NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-010', 'LAP2010', 'Apple', 'MacBook Pro 14"', 59, 4, 'Functional', true, 'In Progress', NULL, 2000.00, NULL, 10, NULL, 'Processing Area', 'Development laptop', NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-011', 'MON2011', 'Samsung', 'Odyssey G7', 65, 3, 'Functional', false, 'Not Required', NULL, 600.00, NULL, NULL, NULL, 'Receiving', 'Gaming monitor', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-012', 'TAB2012', 'Microsoft', 'Surface Pro 8', 63, 3, 'Functional', true, 'Pending', NULL, 1200.00, NULL, NULL, NULL, 'Receiving', 'Executive tablet', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-013', 'PRT2013', 'Canon', 'imageRUNNER ADVANCE', 66, 3, 'Functional', false, 'Not Required', NULL, 800.00, NULL, NULL, NULL, 'Receiving', 'Multifunction printer', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-014', 'UPS2014', 'Eaton', '9PX 6000i', 67, 4, 'Functional', false, 'Not Required', NULL, 2000.00, 1800.00, 8, 14, 'Warehouse C-5', 'High-capacity UPS', NOW(), NOW(), 13, 13, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-015', 'NET2015', 'Juniper', 'EX4300-48T', 62, 4, 'Functional', false, 'Not Required', NULL, 3500.00, 3200.00, 8, 14, 'Warehouse C-3', 'Enterprise switch', NOW(), NOW(), 13, 13, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-016', 'LAP2016', 'Acer', 'Aspire 5', 59, 2, 'Functional', true, 'Pending', NULL, 500.00, NULL, NULL, NULL, 'Processing Area', 'Budget laptop', NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-017', 'MON2017', 'LG', '27UL500-W', 65, 1, 'Functional', false, 'Not Required', NULL, 300.00, NULL, NULL, NULL, 'Receiving', '4K monitor', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-018', 'TAB2018', 'Samsung', 'Galaxy Tab A7', 63, 1, 'Functional', true, 'Pending', NULL, 200.00, NULL, NULL, NULL, 'Receiving', 'Basic tablet', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-019', 'PRT2019', 'Brother', 'HL-L2350DW', 66, 2, 'Functional', false, 'Not Required', NULL, 100.00, NULL, NULL, NULL, 'Processing Area', 'Laser printer', NOW(), NOW(), 12, 12, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-020', 'NET2020', 'ASUS', 'RT-AX88U', 62, 1, 'Functional', false, 'Not Required', NULL, 350.00, NULL, NULL, NULL, 'Receiving', 'WiFi 6 router', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-021', 'SRV2021', 'Supermicro', 'SuperServer 6029P', 61, 3, 'Functional', true, 'Pending', NULL, 3200.00, NULL, NULL, NULL, 'Receiving', 'Dual processor server', NOW(), NOW(), 10, 10, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-022', 'STG2022', 'QNAP', 'TS-464', 64, 2, 'Functional', true, 'Pending', NULL, 400.00, NULL, NULL, NULL, 'Processing Area', '4-bay NAS', NOW(), NOW(), 13, 13, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-023', 'UPS2023', 'Tripp Lite', 'SMART1500LCDT', 67, 1, 'Functional', false, 'Not Required', NULL, 180.00, NULL, NULL, NULL, 'Receiving', 'Line interactive UPS', NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-024', 'PHN2024', 'Google', 'Pixel 6', 63, 2, 'Functional', true, 'Pending', NULL, 350.00, NULL, NULL, NULL, 'Processing Area', 'Android phone', NOW(), NOW(), 14, 14, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('AST-002-025', 'WKS2025', 'Dell', 'Precision 7760', 59, 3, 'Functional', true, 'Pending', NULL, 2500.00, NULL, NULL, NULL, 'Receiving', 'Mobile workstation', NOW(), NOW(), 10, 10, '7d02a831-2bcd-4435-a944-ed0788dfe9e4');


INSERT INTO "ChainOfCustody" ("AssetId", "UserId", "VendorId", "Location", "StatusChange", "ActionType", "Notes", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") VALUES
(1, 3, 2, 'Receiving Dock', 'Received', 'Status Change', 'Asset received from shipment SHP-2024-001', NOW() - INTERVAL '10 days', NOW() - INTERVAL '10 days', 3, 3, 'ADMIN_CLIENT_001'),
(1, 5, 2, 'Processing Area', 'Processing', 'Status Change', 'Moved to processing for data sanitization', NOW() - INTERVAL '8 days', NOW() - INTERVAL '8 days', 5, 5, 'ADMIN_CLIENT_001'),
(1, 5, 2, 'Processing Area', 'Data Sanitized', 'Status Change', 'Data sanitization completed using DoD 5220.22-M', NOW() - INTERVAL '6 days', NOW() - INTERVAL '6 days', 5, 5, 'ADMIN_CLIENT_001'),
(1, 5, 2, 'Warehouse A-1', 'Ready for Disposition', 'Status Change', 'Ready for disposition - functional laptop', NOW() - INTERVAL '4 days', NOW() - INTERVAL '4 days', 5, 5, 'ADMIN_CLIENT_001'),
(1, 5, 2, 'Warehouse A-1', 'Disposed', 'Status Change', 'Disposed to TechRecycle Solutions', NOW() - INTERVAL '2 days', NOW() - INTERVAL '2 days', 5, 5, 'ADMIN_CLIENT_001'),
(26, 9, 7, 'Receiving Dock', 'Received', 'Status Change', 'Asset received from shipment SHP-2024-101', NOW() - INTERVAL '12 days', NOW() - INTERVAL '12 days', 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(26, 11, 7, 'Processing Area', 'Processing', 'Status Change', 'Moved to processing for data sanitization', NOW() - INTERVAL '10 days', NOW() - INTERVAL '10 days', 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(26, 11, 7, 'Processing Area', 'Data Sanitized', 'Status Change', 'Data sanitization completed using DoD 5220.22-M', NOW() - INTERVAL '8 days', NOW() - INTERVAL '8 days', 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(26, 11, 7, 'Warehouse C-1', 'Ready for Disposition', 'Status Change', 'Ready for disposition - functional laptop', NOW() - INTERVAL '6 days', NOW() - INTERVAL '6 days', 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(26, 11, 7, 'Warehouse C-1', 'Disposed', 'Status Change', 'Disposed to EcoTech Recycling', NOW() - INTERVAL '4 days', NOW() - INTERVAL '4 days', 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4');


INSERT INTO "ProcessedMaterials" ("ProcessingLotId", "MaterialTypeId", "Description", "Quantity", "UnitOfMeasure", "QualityGrade", "ExpectedSalesPrice", "ActualSalesPrice", "Notes", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") VALUES
(1, 1, 'Aluminum from laptop cases', 25.5, 'kg', 'Grade A', 1200.00, 1150.00, 'High-grade aluminum from laptop cases', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
(1, 2, 'Copper from cables and components', 8.2, 'kg', 'Grade A', 2800.00, 2650.00, 'Copper from cables and components', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
(2, 3, 'Steel from server chassis', 45.0, 'kg', 'Grade B', 800.00, 750.00, 'Steel from server chassis', NOW(), NOW(), 6, 6, 'ADMIN_CLIENT_001'),
(2, 4, 'Precious metals from server components', 2.1, 'kg', 'Grade A', 1500.00, 1400.00, 'Precious metals from server components', NOW(), NOW(), 6, 6, 'ADMIN_CLIENT_001'),
(6, 1, 'Aluminum from office equipment', 22.8, 'kg', 'Grade A', 1100.00, 1050.00, 'Aluminum from office equipment', NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(6, 2, 'Copper from networking equipment', 7.5, 'kg', 'Grade A', 2500.00, 2400.00, 'Copper from networking equipment', NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(7, 3, 'Steel from manufacturing equipment', 52.0, 'kg', 'Grade B', 900.00, 850.00, 'Steel from manufacturing equipment', NOW(), NOW(), 12, 12, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(7, 4, 'Precious metals from high-end equipment', 2.8, 'kg', 'Grade A', 2000.00, 1900.00, 'Precious metals from high-end equipment', NOW(), NOW(), 12, 12, '7d02a831-2bcd-4435-a944-ed0788dfe9e4');

INSERT INTO "ProcessedMaterialSales" ("ProcessedMaterialId", "VendorId", "InvoiceDate", "SalesQuantity", "AgreedPricePerUnit", "InvoiceTotal", "InvoiceStatus", "ShipmentDate", "InvoiceId", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") VALUES
(1, 2, '2024-08-12', 25.5, 45.10, 1150.05, 'Paid', '2024-08-14', 'INV-2024-001', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
(2, 2, '2024-08-12', 8.2, 323.17, 2650.00, 'Paid', '2024-08-14', 'INV-2024-002', NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
(3, 2, '2024-08-18', 45.0, 16.67, 750.15, 'Paid', '2024-08-20', 'INV-2024-003', NOW(), NOW(), 6, 6, 'ADMIN_CLIENT_001'),
(4, 4, '2024-08-18', 2.1, 666.67, 1400.00, 'Paid', '2024-08-20', 'INV-2024-004', NOW(), NOW(), 6, 6, 'ADMIN_CLIENT_001'),
(5, 7, '2024-08-14', 22.8, 46.05, 1050.00, 'Paid', '2024-08-16', 'INV-2024-101', NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(6, 7, '2024-08-14', 7.5, 320.00, 2400.00, 'Paid', '2024-08-16', 'INV-2024-102', NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(7, 7, '2024-08-21', 52.0, 16.35, 850.20, 'Paid', '2024-08-23', 'INV-2024-103', NOW(), NOW(), 12, 12, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
(8, 9, '2024-08-21', 2.8, 678.57, 1900.00, 'Paid', '2024-08-23', 'INV-2024-104', NOW(), NOW(), 12, 12, '7d02a831-2bcd-4435-a944-ed0788dfe9e4');

COMMIT;
