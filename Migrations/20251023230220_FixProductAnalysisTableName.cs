using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace irevlogix_backend.Migrations
{
    /// <inheritdoc />
    public partial class FixProductAnalysisTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT FROM information_schema.tables 
                        WHERE table_schema = 'public' 
                        AND table_name = 'ProductAnalyses'
                    ) THEN
                        ALTER TABLE ""ProductAnalyses"" RENAME TO ""ProductAnalysis"";
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ProductAnalysis",
                newName: "ProductAnalyses");
        }
    }
}
