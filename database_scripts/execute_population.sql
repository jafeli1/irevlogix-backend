--

BEGIN;

\echo 'Step 1: Executing data cleanup...'
\i 01_data_cleanup.sql

\echo 'Step 2: Duplicating reference data for second client...'
\i 02_duplicate_reference_data.sql

\echo 'Step 3: Creating sample users, vendors, and knowledge base articles...'
\i 03_sample_data_population.sql

\echo 'Step 4: Creating shipments, assets, processing lots, and sales data...'
\i 04_shipments_and_assets.sql

\echo 'Executing verification queries...'

SELECT 'Users' as table_name, "ClientId", COUNT(*) as record_count 
FROM "Users" 
GROUP BY "ClientId" 
ORDER BY "ClientId";

SELECT 'Shipments' as table_name, "ClientId", COUNT(*) as record_count 
FROM "Shipments" 
GROUP BY "ClientId" 
ORDER BY "ClientId";

SELECT 'Assets' as table_name, "ClientId", COUNT(*) as record_count 
FROM "Assets" 
GROUP BY "ClientId" 
ORDER BY "ClientId";

SELECT 'ProcessingLots' as table_name, "ClientId", COUNT(*) as record_count 
FROM "ProcessingLots" 
GROUP BY "ClientId" 
ORDER BY "ClientId";

SELECT 'Vendors' as table_name, "ClientId", COUNT(*) as record_count 
FROM "Vendors" 
GROUP BY "ClientId" 
ORDER BY "ClientId";

SELECT 'KnowledgeBaseArticles' as table_name, "ClientId", COUNT(*) as record_count 
FROM "KnowledgeBaseArticles" 
GROUP BY "ClientId" 
ORDER BY "ClientId";

\echo 'Database population completed successfully!'

COMMIT;
