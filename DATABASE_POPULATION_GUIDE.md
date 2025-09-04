# Database Population Guide

## Overview

This guide documents the comprehensive sample data population for the iRevLogix backend database. The population scripts create realistic sample data for two clients across all core modules including users, reverse logistics, processing, assets, vendors, and knowledge base.

## Clients

The sample data is populated for two clients:

1. **ADMIN_CLIENT_001** - Primary administrative client
2. **7d02a831-2bcd-4435-a944-ed0788dfe9e4** - Second client for multi-tenant testing

## Sample Data Structure

### Users (6 per client)
- **ADMIN_CLIENT_001**: Sarah Johnson, Michael Chen, Emily Rodriguez, David Thompson, Lisa Wang, James Miller
- **Second Client**: Anna Smith, Robert Davis, Jennifer Wilson, Mark Brown, Rachel Garcia, Kevin Lee

**Role Distribution:**
- 2 Project Managers per client
- 2 Logistics Managers per client  
- 2 Logistics Analysts per client

**Sample Credentials:**
- All users have the same default password (contact development team for credentials)
- Email format: `firstname.lastname@domain.com`
- Usernames: `firstname.lastname`

### Shipments (10 per client)
- Realistic tracking numbers from FedEx, UPS, DHL
- Various pickup locations across Texas (ADMIN_CLIENT_001) and Arizona (Second Client)
- 5-10 shipment items per shipment
- Total estimated values ranging from $18,000 to $45,000

### Assets (25 per client)
- Mix of laptops, servers, monitors, networking equipment, mobile devices
- Various conditions: Functional, Non-functional, Data-bearing
- Proper data sanitization tracking with methods like DoD 5220.22-M, NIST 800-88
- Chain of custody records showing asset lifecycle
- Current locations: Receiving, Processing Area, various warehouse locations

### Processing Lots (5 per client)
- Batch processing of related assets
- Start and completion dates spanning August-September 2024
- Processing costs and recovery values tracked
- Status: Completed or In Progress

### Vendors (5 per client)
- **ADMIN_CLIENT_001**: TechRecycle Solutions, MetalWorks Recovery, GreenTech Disposal, Precious Metals Inc, DataSecure Shredding
- **Second Client**: EcoTech Recycling, Advanced Materials, SecureWipe Services, RefineMax Metals, CircuitBreaker Recovery
- Various vendor types: Electronics Recyclers, Metal Dealers, Data Destruction, Precious Metal Refiners

### Processed Materials & Sales
- Materials extracted from processing lots: Aluminum, Copper, Steel, Precious Metals
- Quality grades: Grade A, Grade B
- Sales transactions with realistic pricing
- Payment status: Paid
- Invoice numbers and shipping dates

### Knowledge Base Articles (10 per client)
- Categories: Operations, Security, Business, Safety, Quality, Compliance, Finance, Technology, Environment
- Published articles with view counts
- Topics covering asset intake, data sanitization, vendor management, safety protocols, etc.

## Execution Instructions

### Prerequisites
1. PostgreSQL client installed
2. Database connection credentials (contact development team)
3. Development/testing environment (NOT production)

### Running the Population Scripts

**Option 1: Master Script (Recommended)**
```bash
PGPASSWORD=$DB_PASSWORD psql -h $DB_HOST -U $DB_USER $DB_NAME -f execute_population.sql
```

**Option 2: Individual Scripts**
```bash
# Step 1: Data cleanup
PGPASSWORD=$DB_PASSWORD psql -h $DB_HOST -U $DB_USER $DB_NAME -f 01_data_cleanup.sql

# Step 2: Reference data duplication
PGPASSWORD=$DB_PASSWORD psql -h $DB_HOST -U $DB_USER $DB_NAME -f 02_duplicate_reference_data.sql

# Step 3: Sample data population
PGPASSWORD=$DB_PASSWORD psql -h $DB_HOST -U $DB_USER $DB_NAME -f 03_sample_data_population.sql

# Step 4: Shipments and assets
PGPASSWORD=$DB_PASSWORD psql -h $DB_HOST -U $DB_USER $DB_NAME -f 04_shipments_and_assets.sql
```

**Environment Variables Required:**
- `DB_HOST`: Database host address
- `DB_USER`: Database username  
- `DB_PASSWORD`: Database password
- `DB_NAME`: Database name

## Verification Queries

### Data Count Verification
```sql
-- Verify user counts
SELECT "ClientId", COUNT(*) as user_count FROM "Users" GROUP BY "ClientId";

-- Verify shipment counts  
SELECT "ClientId", COUNT(*) as shipment_count FROM "Shipments" GROUP BY "ClientId";

-- Verify asset counts
SELECT "ClientId", COUNT(*) as asset_count FROM "Assets" GROUP BY "ClientId";

-- Verify processing lot counts
SELECT "ClientId", COUNT(*) as lot_count FROM "ProcessingLots" GROUP BY "ClientId";

-- Verify vendor counts
SELECT "ClientId", COUNT(*) as vendor_count FROM "Vendors" GROUP BY "ClientId";

-- Verify knowledge base article counts
SELECT "ClientId", COUNT(*) as article_count FROM "KnowledgeBaseArticles" GROUP BY "ClientId";
```

### Multi-Tenant Isolation Verification
```sql
-- Verify no cross-client data leakage
SELECT DISTINCT "ClientId" FROM "Assets" ORDER BY "ClientId";
SELECT DISTINCT "ClientId" FROM "Shipments" ORDER BY "ClientId";
SELECT DISTINCT "ClientId" FROM "ProcessingLots" ORDER BY "ClientId";
```

### Relationship Verification
```sql
-- Verify asset to shipment item relationships
SELECT COUNT(*) FROM "Assets" a 
JOIN "ShipmentItems" si ON a."ShipmentItemId" = si."Id" 
WHERE a."ClientId" = si."ClientId";

-- Verify processing lot to asset relationships
SELECT COUNT(*) FROM "Assets" a 
JOIN "ProcessingLots" pl ON a."ProcessingLotId" = pl."Id" 
WHERE a."ClientId" = pl."ClientId";

-- Verify chain of custody records
SELECT COUNT(*) FROM "ChainOfCustody" cc 
JOIN "Assets" a ON cc."AssetId" = a."Id" 
WHERE cc."ClientId" = a."ClientId";
```

## Important Notes

⚠️ **WARNING**: These scripts will DELETE existing data in most tables. Only run on development/testing databases.

### Data Cleanup Performed
- Deletes specified user records and resets IDs
- Cleans all transactional data tables
- Resets identity sequences
- Updates audit fields (CreatedBy/UpdatedBy) to user ID 1
- Preserves protected tables: Clients, Roles, UserRoles, Permissions, RolePermissions, ApplicationSettings

### Multi-Tenant Considerations
- All data properly isolated by ClientId
- Foreign key relationships respect tenant boundaries
- Audit fields reference appropriate user IDs within each tenant
- No cross-client data contamination

### Password Information
- All sample users use the same default password (contact development team for credentials)
- Passwords are hashed using ASP.NET Core Identity format
- Hash format follows ASP.NET Core Identity standard (64 characters)

## Troubleshooting

### Common Issues
1. **Connection Errors**: Verify database credentials and network connectivity
2. **Permission Errors**: Ensure database user has sufficient privileges
3. **Foreign Key Violations**: Check that reference data exists before running dependent scripts
4. **Identity Sequence Issues**: Scripts include sequence resets to prevent ID conflicts

### Rollback
If you need to rollback changes:
1. Restore from database backup (recommended)
2. Or manually delete inserted data using ClientId filters

## File Structure
```
database_scripts/
├── execute_population.sql      # Master execution script
├── 01_data_cleanup.sql        # Data cleanup and user resets
├── 02_duplicate_reference_data.sql  # Reference data duplication
├── 03_sample_data_population.sql   # Users, vendors, knowledge base
└── 04_shipments_and_assets.sql     # Shipments, assets, processing
```

## Support
For issues or questions regarding the database population scripts, please refer to the iRevLogix backend documentation or contact the development team.
