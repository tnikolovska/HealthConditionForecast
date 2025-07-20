using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthConditionForecast.Migrations
{
    /// <inheritdoc />
    public partial class FixJoinTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SymptomId",
                table: "UserSymptomSelections",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSymptomSelections_SymptomId",
                table: "UserSymptomSelections",
                column: "SymptomId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSymptomSelections_Symptoms_SymptomId",
                table: "UserSymptomSelections",
                column: "SymptomId",
                principalTable: "Symptoms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSymptomSelections_Symptoms_SymptomId",
                table: "UserSymptomSelections");

            migrationBuilder.DropIndex(
                name: "IX_UserSymptomSelections_SymptomId",
                table: "UserSymptomSelections");

            migrationBuilder.DropColumn(
                name: "SymptomId",
                table: "UserSymptomSelections");
        }
    }
}
