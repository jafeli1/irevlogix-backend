using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace irevlogix_backend.Migrations
{
    /// <inheritdoc />
    public partial class FixOperatorUserIdConflict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                        -- Drop the existing OperatorUserId column if it exists
                        ALTER TABLE ""ProcessingLots"" DROP COLUMN ""OperatorUserId"";
                    END IF;
                END $$;
            ");

            migrationBuilder.AddColumn<int>(
                name: "OperatorUserId",
                table: "ProcessingLots",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingLots_OperatorUserId",
                table: "ProcessingLots",
                column: "OperatorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessingLots_Users_OperatorUserId",
                table: "ProcessingLots",
                column: "OperatorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessingLots_Users_OperatorUserId",
                table: "ProcessingLots");

            migrationBuilder.DropIndex(
                name: "IX_ProcessingLots_OperatorUserId",
                table: "ProcessingLots");

            migrationBuilder.DropColumn(
                name: "OperatorUserId",
                table: "ProcessingLots");
        }
    }
}
