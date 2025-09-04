BEGIN;



INSERT INTO "AssetTrackingStatuses" ("StatusName", "Description", "IsActive", "SortOrder", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") VALUES
('Received', 'Asset has been received and logged', true, 1, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('To Audit', 'Asset needs to be audited', true, 2, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('To Repair', 'Asset needs repair', true, 3, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('To Data Destruction', 'Asset needs data destruction', true, 4, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('To Resale Inventory', 'Asset ready for resale', true, 5, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('To Certified Recycling', 'Asset for certified recycling', true, 6, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Sold', 'Asset has been sold', true, 7, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Disposed', 'Asset has been disposed', true, 8, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Data Wipe Completed', 'Data wipe completed', true, 9, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Resale Listed', 'Asset listed for resale', true, 10, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4');


INSERT INTO "Users" ("FirstName", "LastName", "Email", "PasswordHash", "Username", "DisplayName", "IsActive", "IsEmailConfirmed", "FailedLoginAttempts", "TwoFactorAuthEnabled", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") VALUES
('Sarah', 'Johnson', 'sarah.johnson@adminlogix.com', 'AQAAAAEAACcQAAAAEKqJ8zKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQ==', 'sarah.johnson', 'Sarah Johnson', true, true, 0, false, NOW(), NOW(), 1, 1, 'ADMIN_CLIENT_001'),
('Michael', 'Chen', 'michael.chen@adminlogix.com', 'AQAAAAEAACcQAAAAEKqJ8zKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQ==', 'michael.chen', 'Michael Chen', true, true, 0, false, NOW(), NOW(), 1, 1, 'ADMIN_CLIENT_001'),
('Emily', 'Rodriguez', 'emily.rodriguez@adminlogix.com', 'AQAAAAEAACcQAAAAEKqJ8zKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQ==', 'emily.rodriguez', 'Emily Rodriguez', true, true, 0, false, NOW(), NOW(), 1, 1, 'ADMIN_CLIENT_001'),
('David', 'Thompson', 'david.thompson@adminlogix.com', 'AQAAAAEAACcQAAAAEKqJ8zKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQ==', 'david.thompson', 'David Thompson', true, true, 0, false, NOW(), NOW(), 1, 1, 'ADMIN_CLIENT_001'),
('Lisa', 'Wang', 'lisa.wang@adminlogix.com', 'AQAAAAEAACcQAAAAEKqJ8zKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQ==', 'lisa.wang', 'Lisa Wang', true, true, 0, false, NOW(), NOW(), 1, 1, 'ADMIN_CLIENT_001'),
('James', 'Miller', 'james.miller@adminlogix.com', 'AQAAAAEAACcQAAAAEKqJ8zKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQ==', 'james.miller', 'James Miller', true, true, 0, false, NOW(), NOW(), 1, 1, 'ADMIN_CLIENT_001'),
('Anna', 'Smith', 'anna.smith@techcorp.com', 'AQAAAAEAACcQAAAAEKqJ8zKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQ==', 'anna.smith', 'Anna Smith', true, true, 0, false, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Robert', 'Davis', 'robert.davis@techcorp.com', 'AQAAAAEAACcQAAAAEKqJ8zKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQ==', 'robert.davis', 'Robert Davis', true, true, 0, false, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Jennifer', 'Wilson', 'jennifer.wilson@techcorp.com', 'AQAAAAEAACcQAAAAEKqJ8zKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQ==', 'jennifer.wilson', 'Jennifer Wilson', true, true, 0, false, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Mark', 'Brown', 'mark.brown@techcorp.com', 'AQAAAAEAACcQAAAAEKqJ8zKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQ==', 'mark.brown', 'Mark Brown', true, true, 0, false, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Rachel', 'Garcia', 'rachel.garcia@techcorp.com', 'AQAAAAEAACcQAAAAEKqJ8zKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQ==', 'rachel.garcia', 'Rachel Garcia', true, true, 0, false, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Kevin', 'Lee', 'kevin.lee@techcorp.com', 'AQAAAAEAACcQAAAAEKqJ8zKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQzKfQ==', 'kevin.lee', 'Kevin Lee', true, true, 0, false, NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4');


INSERT INTO "UserRoles" ("UserId", "RoleId", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") 
SELECT u."Id", 2, NOW(), NOW(), 1, 1, u."ClientId" 
FROM "Users" u 
WHERE u."FirstName" IN ('Sarah', 'Michael', 'Anna', 'Robert') AND u."Id" > 2;

INSERT INTO "UserRoles" ("UserId", "RoleId", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") 
SELECT u."Id", 3, NOW(), NOW(), 1, 1, u."ClientId" 
FROM "Users" u 
WHERE u."FirstName" IN ('Emily', 'David', 'Jennifer', 'Mark') AND u."Id" > 2;

INSERT INTO "UserRoles" ("UserId", "RoleId", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") 
SELECT u."Id", 4, NOW(), NOW(), 1, 1, u."ClientId" 
FROM "Users" u 
WHERE u."FirstName" IN ('Lisa', 'James', 'Rachel', 'Kevin') AND u."Id" > 2;


INSERT INTO "Vendors" ("VendorName", "ContactPerson", "Email", "Phone", "Address", "City", "State", "PostalCode", "Country", "VendorTier", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") VALUES
('TechRecycle Solutions', 'John Anderson', 'john@techrecycle.com', '555-0101', '123 Industrial Blvd', 'Austin', 'TX', '78701', 'USA', 'Tier 1', NOW(), NOW(), 1, 1, 'ADMIN_CLIENT_001'),
('MetalWorks Recovery', 'Maria Santos', 'maria@metalworks.com', '555-0102', '456 Steel Ave', 'Houston', 'TX', '77001', 'USA', 'Tier 1', NOW(), NOW(), 1, 1, 'ADMIN_CLIENT_001'),
('GreenTech Disposal', 'Robert Kim', 'robert@greentech.com', '555-0103', '789 Eco Way', 'Dallas', 'TX', '75201', 'USA', 'Tier 2', NOW(), NOW(), 1, 1, 'ADMIN_CLIENT_001'),
('Precious Metals Inc', 'Lisa Chang', 'lisa@preciousmetals.com', '555-0104', '321 Gold St', 'San Antonio', 'TX', '78201', 'USA', 'Tier 1', NOW(), NOW(), 1, 1, 'ADMIN_CLIENT_001'),
('DataSecure Shredding', 'Mike Johnson', 'mike@datasecure.com', '555-0105', '654 Security Blvd', 'Fort Worth', 'TX', '76101', 'USA', 'Tier 2', NOW(), NOW(), 1, 1, 'ADMIN_CLIENT_001'),
('EcoTech Recycling', 'Jennifer White', 'jennifer@ecotech.com', '555-0201', '987 Green Lane', 'Phoenix', 'AZ', '85001', 'USA', 'Tier 1', NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Advanced Materials', 'David Brown', 'david@advancedmaterials.com', '555-0202', '147 Tech Park Dr', 'Tucson', 'AZ', '85701', 'USA', 'Tier 1', NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('SecureWipe Services', 'Sarah Wilson', 'sarah@securewipe.com', '555-0203', '258 Data Center Rd', 'Mesa', 'AZ', '85201', 'USA', 'Tier 2', NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('RefineMax Metals', 'Kevin Martinez', 'kevin@refinemax.com', '555-0204', '369 Refinery St', 'Scottsdale', 'AZ', '85251', 'USA', 'Tier 1', NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('CircuitBreaker Recovery', 'Amanda Davis', 'amanda@circuitbreaker.com', '555-0205', '741 Circuit Ave', 'Chandler', 'AZ', '85224', 'USA', 'Tier 2', NOW(), NOW(), 1, 1, '7d02a831-2bcd-4435-a944-ed0788dfe9e4');


INSERT INTO "KnowledgeBaseArticles" ("Title", "Content", "Category", "Tags", "IsPublished", "ViewCount", "AuthorUserId", "PublishedDate", "Summary", "SortOrder", "DateCreated", "DateUpdated", "CreatedBy", "UpdatedBy", "ClientId") VALUES
('Asset Intake Procedures', 'Detailed procedures for receiving and cataloging incoming IT assets...', 'Operations', 'intake,procedures,assets', true, 45, 3, NOW(), 'Step-by-step guide for asset intake process', 1, NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
('Data Sanitization Standards', 'Comprehensive guide to data destruction and sanitization methods...', 'Security', 'data,sanitization,security', true, 67, 5, NOW(), 'Standards and procedures for secure data destruction', 2, NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
('Processing Lot Management', 'Best practices for organizing and managing processing lots...', 'Operations', 'processing,lots,management', true, 32, 4, NOW(), 'Guidelines for efficient lot processing', 3, NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
('Vendor Relationship Guidelines', 'How to establish and maintain relationships with downstream vendors...', 'Business', 'vendors,relationships,sales', true, 28, 3, NOW(), 'Best practices for vendor management', 4, NOW(), NOW(), 3, 3, 'ADMIN_CLIENT_001'),
('Safety Protocols', 'Safety procedures for handling electronic waste and hazardous materials...', 'Safety', 'safety,protocols,hazmat', true, 89, 6, NOW(), 'Essential safety guidelines for operations', 5, NOW(), NOW(), 6, 6, 'ADMIN_CLIENT_001'),
('Quality Control Checklist', 'Quality assurance procedures for asset processing...', 'Quality', 'quality,control,checklist', true, 41, 7, NOW(), 'QC procedures and checklists', 6, NOW(), NOW(), 7, 7, 'ADMIN_CLIENT_001'),
('Compliance Requirements', 'Regulatory compliance requirements for e-waste processing...', 'Compliance', 'compliance,regulations,legal', true, 73, 5, NOW(), 'Overview of compliance obligations', 7, NOW(), NOW(), 5, 5, 'ADMIN_CLIENT_001'),
('Asset Valuation Methods', 'Techniques for determining asset values and recovery potential...', 'Finance', 'valuation,assets,finance', true, 36, 8, NOW(), 'Methods for asset valuation', 8, NOW(), NOW(), 8, 8, 'ADMIN_CLIENT_001'),
('Chain of Custody Procedures', 'Maintaining proper chain of custody documentation...', 'Operations', 'custody,documentation,tracking', true, 52, 4, NOW(), 'Chain of custody best practices', 9, NOW(), NOW(), 4, 4, 'ADMIN_CLIENT_001'),
('Emergency Response Plan', 'Emergency procedures for incidents during processing...', 'Safety', 'emergency,response,incidents', true, 19, 6, NOW(), 'Emergency response protocols', 10, NOW(), NOW(), 6, 6, 'ADMIN_CLIENT_001'),
('Equipment Handling Guidelines', 'Safe handling procedures for various types of IT equipment...', 'Operations', 'handling,equipment,safety', true, 38, 11, NOW(), 'Guidelines for safe equipment handling', 1, NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Data Security Protocols', 'Advanced data security measures for sensitive equipment...', 'Security', 'security,data,protocols', true, 61, 13, NOW(), 'Enhanced security procedures', 2, NOW(), NOW(), 13, 13, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Inventory Management System', 'Using the inventory system for tracking and reporting...', 'Technology', 'inventory,system,tracking', true, 44, 12, NOW(), 'System usage guidelines', 3, NOW(), NOW(), 12, 12, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Material Recovery Optimization', 'Strategies for maximizing material recovery value...', 'Business', 'recovery,optimization,value', true, 29, 9, NOW(), 'Value optimization strategies', 4, NOW(), NOW(), 9, 9, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Environmental Impact Assessment', 'Measuring and reporting environmental impact...', 'Environment', 'environment,impact,reporting', true, 55, 14, NOW(), 'Environmental assessment procedures', 5, NOW(), NOW(), 14, 14, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Customer Communication Standards', 'Best practices for client communication and reporting...', 'Business', 'communication,clients,reporting', true, 33, 10, NOW(), 'Client communication guidelines', 6, NOW(), NOW(), 10, 10, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Audit Preparation Checklist', 'Preparing for regulatory and client audits...', 'Compliance', 'audit,preparation,compliance', true, 47, 11, NOW(), 'Audit readiness procedures', 7, NOW(), NOW(), 11, 11, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Technology Refresh Planning', 'Planning for technology refresh and upgrade cycles...', 'Technology', 'refresh,planning,technology', true, 26, 12, NOW(), 'Technology planning guidelines', 8, NOW(), NOW(), 12, 12, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Cost Analysis Framework', 'Framework for analyzing processing costs and profitability...', 'Finance', 'cost,analysis,profitability', true, 39, 13, NOW(), 'Cost analysis methodologies', 9, NOW(), NOW(), 13, 13, '7d02a831-2bcd-4435-a944-ed0788dfe9e4'),
('Continuous Improvement Process', 'Process improvement methodologies and implementation...', 'Operations', 'improvement,process,methodology', true, 22, 14, NOW(), 'Continuous improvement practices', 10, NOW(), NOW(), 14, 14, '7d02a831-2bcd-4435-a944-ed0788dfe9e4');

COMMIT;
