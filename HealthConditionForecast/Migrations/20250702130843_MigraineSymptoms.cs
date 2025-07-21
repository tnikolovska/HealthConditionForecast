using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthConditionForecast.Migrations
{
    /// <inheritdoc />
    public partial class MigraineSymptoms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MigraineSympton_Type",
                table: "Symptoms",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigraineSympton_Type",
                table: "Symptoms");
        }
    }
}
