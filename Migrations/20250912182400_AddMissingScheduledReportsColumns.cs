using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace irevlogix_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingScheduledReportsColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DataSource",
                table: "ScheduledReports",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SelectedColumns",
                table: "ScheduledReports",
                type: "text",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "Filters",
                table: "ScheduledReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sorting",
                table: "ScheduledReports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "ScheduledReports",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Daily");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DeliveryTime",
                table: "ScheduledReports",
                type: "interval",
                nullable: false,
                defaultValue: TimeSpan.FromHours(9));

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "ScheduledReports",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DayOfMonth",
                table: "ScheduledReports",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "ScheduledReports",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataSource",
                table: "ScheduledReports");

            migrationBuilder.DropColumn(
                name: "SelectedColumns",
                table: "ScheduledReports");

            migrationBuilder.DropColumn(
                name: "Filters",
                table: "ScheduledReports");

            migrationBuilder.DropColumn(
                name: "Sorting",
                table: "ScheduledReports");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "ScheduledReports");

            migrationBuilder.DropColumn(
                name: "DeliveryTime",
                table: "ScheduledReports");

            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "ScheduledReports");

            migrationBuilder.DropColumn(
                name: "DayOfMonth",
                table: "ScheduledReports");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "ScheduledReports",
                type: "character varying(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
