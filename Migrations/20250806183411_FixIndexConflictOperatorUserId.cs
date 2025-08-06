using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace irevlogix_backend.Migrations
{
    /// <inheritdoc />
    public partial class FixIndexConflictOperatorUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    -- Drop IX_ProcessingLots_SourceShipmentId if it exists
                    IF EXISTS (
                        SELECT 1 
                        FROM pg_indexes 
                        WHERE indexname = 'IX_ProcessingLots_SourceShipmentId'
                        AND schemaname = 'public'
                    ) THEN
                        DROP INDEX ""IX_ProcessingLots_SourceShipmentId"";
                    END IF;
                    
                    -- Drop IX_ProcessingLots_OperatorUserId if it exists
                    IF EXISTS (
                        SELECT 1 
                        FROM pg_indexes 
                        WHERE indexname = 'IX_ProcessingLots_OperatorUserId'
                        AND schemaname = 'public'
                    ) THEN
                        DROP INDEX ""IX_ProcessingLots_OperatorUserId"";
                    END IF;
                END $$;
            ");

            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name = 'ProcessingLots' 
                        AND column_name = 'OperatorUserId'
                        AND table_schema = 'public'
                    ) THEN
                        -- Create the index for OperatorUserId
                        CREATE INDEX ""IX_ProcessingLots_OperatorUserId"" ON ""ProcessingLots"" (""OperatorUserId"");
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    -- Drop IX_ProcessingLots_OperatorUserId if it exists
                    IF EXISTS (
                        SELECT 1 
                        FROM pg_indexes 
                        WHERE indexname = 'IX_ProcessingLots_OperatorUserId'
                        AND schemaname = 'public'
                    ) THEN
                        DROP INDEX ""IX_ProcessingLots_OperatorUserId"";
                    END IF;
                    
                    -- Create IX_ProcessingLots_SourceShipmentId if SourceShipmentId column exists
                    IF EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name = 'ProcessingLots' 
                        AND column_name = 'SourceShipmentId'
                        AND table_schema = 'public'
                    ) THEN
                        CREATE INDEX ""IX_ProcessingLots_SourceShipmentId"" ON ""ProcessingLots"" (""SourceShipmentId"");
                    END IF;
                END $$;
            ");
        }
    }
}
