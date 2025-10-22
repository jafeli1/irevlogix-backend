
CREATE TABLE IF NOT EXISTS "ProductAnalysis" (
    "Id" SERIAL PRIMARY KEY,
    "ClientId" VARCHAR(500) NOT NULL,
    "ProductName" VARCHAR(500) NOT NULL,
    "Brand" VARCHAR(200),
    "Model" VARCHAR(200),
    "Category" VARCHAR(200),
    "ProductDescription" TEXT,
    "ImagePath" VARCHAR(1000),
    "Specifications" JSONB,
    "Components" JSONB,
    "MarketPrice" JSONB,
    "Summary" TEXT,
    "EbayListings" JSONB,
    "AnalysisDate" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "LastMarketRefresh" TIMESTAMP,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedBy" INTEGER NOT NULL,
    "DateCreated" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedBy" INTEGER,
    "DateUpdated" TIMESTAMP
);

CREATE INDEX IF NOT EXISTS "idx_productanalysis_clientid" ON "ProductAnalysis" ("ClientId");
CREATE INDEX IF NOT EXISTS "idx_productanalysis_analysisdate" ON "ProductAnalysis" ("AnalysisDate");
CREATE INDEX IF NOT EXISTS "idx_productanalysis_lastmarketrefresh" ON "ProductAnalysis" ("LastMarketRefresh");
CREATE INDEX IF NOT EXISTS "idx_productanalysis_isactive" ON "ProductAnalysis" ("IsActive");
CREATE INDEX IF NOT EXISTS "idx_productanalysis_productname" ON "ProductAnalysis" ("ProductName");

COMMENT ON TABLE "ProductAnalysis" IS 'Stores Market Intelligence product analysis results with market data that refreshes daily';
COMMENT ON COLUMN "ProductAnalysis"."Components" IS 'JSON array of recyclable components with materials and estimated values';
COMMENT ON COLUMN "ProductAnalysis"."MarketPrice" IS 'JSON object with averagePrice, priceRange, and marketTrend';
COMMENT ON COLUMN "ProductAnalysis"."EbayListings" IS 'JSON array of eBay listings used for market analysis';
COMMENT ON COLUMN "ProductAnalysis"."LastMarketRefresh" IS 'Timestamp of last market data refresh from scheduled job';
