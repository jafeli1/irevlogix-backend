CREATE TABLE IF NOT EXISTS "ComplianceTrackerCertifications" (
  "Id" SERIAL PRIMARY KEY,
  "DocumentUrl" VARCHAR(500),
  "Filename" VARCHAR(255),
  "ContentType" VARCHAR(150),
  "CertificationType" VARCHAR(150),
  "IssueDate" TIMESTAMP NULL,
  "ExpirationDate" TIMESTAMP NULL,
  "DateReceived" TIMESTAMP NULL,
  "ReviewComment" TEXT NULL,
  "LastReviewDate" TIMESTAMP NULL,
  "ReviewedBy" INT NULL,
  "ClientId" VARCHAR(50) NOT NULL,
  "DateCreated" TIMESTAMP NOT NULL DEFAULT NOW(),
  "DateUpdated" TIMESTAMP NOT NULL DEFAULT NOW(),
  "CreatedBy" INT NOT NULL,
  "UpdatedBy" INT NOT NULL
);
