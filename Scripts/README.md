# Knowledge Base Articles Update Script

This script updates the KnowledgeBaseArticles database table with the actual article content extracted from the Knowledge Base Articles.docx document.

## Overview

The script replaces placeholder content in the database with comprehensive, properly formatted articles for the following 10 knowledge base topics:

1. Asset Intake Procedures
2. Equipment Handling Guidelines  
3. Data Security Protocols
4. Data Sanitization Standards
5. Processing Lot Management
6. Inventory Management System
7. Vendor Relationship Guidelines
8. Material Recovery Optimization
9. Safety Protocols
10. Environmental Impact Assessment

## Features

- **Content Preservation**: Maintains all formatting including bold text, bullet points, and structured content
- **Header Exclusion**: Removes numbered headers (e.g., "1. Asset Intake Procedures") as requested
- **Database Safety**: Only updates the Content field, preserving all other metadata
- **Error Handling**: Comprehensive error handling with detailed logging
- **Verification**: Confirms successful updates for all articles

## Usage

### Prerequisites

1. Ensure the database connection string is configured in `appsettings.json`
2. Verify the KnowledgeBaseArticles table exists with the expected schema
3. Confirm you have appropriate database permissions for updates

### Running the Script

From the Scripts directory:

```bash
dotnet run
```

The script will:
1. Connect to the database using the configured connection string
2. Locate existing articles by matching titles exactly
3. Update the Content field with the new article content
4. Preserve all existing metadata (Category, Tags, Summary, etc.)
5. Log progress and any issues encountered

### Expected Output

```
Starting Knowledge Base Articles update...
Updated article: Asset Intake Procedures
Updated article: Equipment Handling Guidelines
Updated article: Data Security Protocols
Updated article: Data Sanitization Standards
Updated article: Processing Lot Management
Updated article: Inventory Management System
Updated article: Vendor Relationship Guidelines
Updated article: Material Recovery Optimization
Updated article: Safety Protocols
Updated article: Environmental Impact Assessment
All articles updated successfully!
Update completed successfully!
```

## Content Details

Each article contains:
- **Comprehensive Content**: Detailed, professional content ranging from 1,179 to 1,761 characters
- **Structured Format**: Organized with headers, bullet points, and logical sections
- **Rich Formatting**: Bold text, bullet points, and proper paragraph structure
- **Industry Standards**: Content aligned with ITAD and recycling industry best practices

## Database Impact

The script only modifies:
- `Content` field: Updated with new article content
- `DateUpdated` field: Set to current timestamp
- `UpdatedBy` field: Set to 1 (system update)

All other fields remain unchanged, including:
- Title, Category, Tags, Summary
- IsPublished, ViewCount, SortOrder
- AuthorUserId, PublishedDate
- CreatedBy, DateCreated, ClientId

## Verification

After running the script, you can verify the updates by:
1. Querying the database to confirm Content field updates
2. Checking the knowledge base page at https://irevlogix.ai/knowledge-base
3. Verifying that formatting is preserved and headers are excluded
4. Confirming all 10 articles show the new comprehensive content

## Troubleshooting

Common issues and solutions:

- **Connection String Error**: Ensure `appsettings.json` contains a valid `DefaultConnection`
- **Article Not Found**: Verify article titles match exactly in the database
- **Permission Denied**: Confirm database user has UPDATE permissions
- **Formatting Issues**: Check that content displays correctly in the web interface

## Safety Features

- **Read-Only Operations**: Script only reads existing records to match by title
- **Targeted Updates**: Only updates the specific Content field
- **Error Isolation**: Continues processing other articles if one fails
- **Detailed Logging**: Comprehensive logging for troubleshooting
- **No Data Loss**: Preserves all existing metadata and relationships
