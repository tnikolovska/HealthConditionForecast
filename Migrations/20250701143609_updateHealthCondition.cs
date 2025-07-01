using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthConditionForecast.Migrations
{
    /// <inheritdoc />
    public partial class updateHealthCondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "HealthConditions",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "HealthConditions",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "HealthConditions",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "HealthConditions",
                newName: "description");
        }
    }
}
