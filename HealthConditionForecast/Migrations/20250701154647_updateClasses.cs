using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthConditionForecast.Migrations
{
    /// <inheritdoc />
    public partial class updateClasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "Symptoms",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Symptoms",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "Forecasts",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Forecasts",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "mobileLink",
                table: "Forecasts",
                newName: "MobileLink");

            migrationBuilder.RenameColumn(
                name: "link",
                table: "Forecasts",
                newName: "Link");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "Forecasts",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "categoryValue",
                table: "Forecasts",
                newName: "CategoryValue");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "Forecasts",
                newName: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Symptoms",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Symptoms",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Forecasts",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Forecasts",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "MobileLink",
                table: "Forecasts",
                newName: "mobileLink");

            migrationBuilder.RenameColumn(
                name: "Link",
                table: "Forecasts",
                newName: "link");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Forecasts",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "CategoryValue",
                table: "Forecasts",
                newName: "categoryValue");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Forecasts",
                newName: "category");
        }
    }
}
