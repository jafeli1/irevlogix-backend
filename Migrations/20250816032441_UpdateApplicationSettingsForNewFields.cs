using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace irevlogix_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApplicationSettingsForNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SettingValue",
                table: "ApplicationSettings",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "SettingKey",
                table: "ApplicationSettings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationErrorLogFolderPath",
                table: "ApplicationSettings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationLogoPath",
                table: "ApplicationSettings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUploadFolderPath",
                table: "ApplicationSettings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultLogoutPageUrl",
                table: "ApplicationSettings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LockoutDurationMinutes",
                table: "ApplicationSettings",
                type: "integer",
                nullable: true,
                defaultValue: 30);

            migrationBuilder.AddColumn<int>(
                name: "LoginTimeoutMinutes",
                table: "ApplicationSettings",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordComplexityRequirements",
                table: "ApplicationSettings",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                defaultValue: "Minimum 8 characters, at least one uppercase letter, one lowercase letter, one number, and one special character.");

            migrationBuilder.AddColumn<int>(
                name: "PasswordExpiryDays",
                table: "ApplicationSettings",
                type: "integer",
                nullable: true,
                defaultValue: 45);

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorAuthenticationFrequency",
                table: "ApplicationSettings",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "Never");

            migrationBuilder.AddColumn<int>(
                name: "UnsuccessfulLoginAttemptsBeforeLockout",
                table: "ApplicationSettings",
                type: "integer",
                nullable: true,
                defaultValue: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationErrorLogFolderPath",
                table: "ApplicationSettings");

            migrationBuilder.DropColumn(
                name: "ApplicationLogoPath",
                table: "ApplicationSettings");

            migrationBuilder.DropColumn(
                name: "ApplicationUploadFolderPath",
                table: "ApplicationSettings");

            migrationBuilder.DropColumn(
                name: "DefaultLogoutPageUrl",
                table: "ApplicationSettings");

            migrationBuilder.DropColumn(
                name: "LockoutDurationMinutes",
                table: "ApplicationSettings");

            migrationBuilder.DropColumn(
                name: "LoginTimeoutMinutes",
                table: "ApplicationSettings");

            migrationBuilder.DropColumn(
                name: "PasswordComplexityRequirements",
                table: "ApplicationSettings");

            migrationBuilder.DropColumn(
                name: "PasswordExpiryDays",
                table: "ApplicationSettings");

            migrationBuilder.DropColumn(
                name: "TwoFactorAuthenticationFrequency",
                table: "ApplicationSettings");

            migrationBuilder.DropColumn(
                name: "UnsuccessfulLoginAttemptsBeforeLockout",
                table: "ApplicationSettings");

            migrationBuilder.AlterColumn<string>(
                name: "SettingValue",
                table: "ApplicationSettings",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SettingKey",
                table: "ApplicationSettings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }
    }
}
