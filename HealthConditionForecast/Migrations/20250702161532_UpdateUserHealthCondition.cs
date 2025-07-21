using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthConditionForecast.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserHealthCondition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserHealthConditions_AspNetUsers_UserId",
                table: "UserHealthConditions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserHealthConditions_HealthConditions_HealthConditionId1",
                table: "UserHealthConditions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSymptomSelections_UserHealthConditions_UserHealthConditionId1",
                table: "UserSymptomSelections");

            migrationBuilder.DropIndex(
                name: "IX_UserSymptomSelections_UserHealthConditionId1",
                table: "UserSymptomSelections");

            migrationBuilder.DropIndex(
                name: "IX_UserHealthConditions_HealthConditionId1",
                table: "UserHealthConditions");

            migrationBuilder.DropIndex(
                name: "IX_UserHealthConditions_UserId",
                table: "UserHealthConditions");

            migrationBuilder.DropColumn(
                name: "UserHealthConditionId1",
                table: "UserSymptomSelections");

            migrationBuilder.DropColumn(
                name: "HealthConditionId1",
                table: "UserHealthConditions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserHealthConditionId1",
                table: "UserSymptomSelections",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "HealthConditionId1",
                table: "UserHealthConditions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_UserSymptomSelections_UserHealthConditionId1",
                table: "UserSymptomSelections",
                column: "UserHealthConditionId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthConditions_HealthConditionId1",
                table: "UserHealthConditions",
                column: "HealthConditionId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserHealthConditions_UserId",
                table: "UserHealthConditions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserHealthConditions_AspNetUsers_UserId",
                table: "UserHealthConditions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserHealthConditions_HealthConditions_HealthConditionId1",
                table: "UserHealthConditions",
                column: "HealthConditionId1",
                principalTable: "HealthConditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSymptomSelections_UserHealthConditions_UserHealthConditionId1",
                table: "UserSymptomSelections",
                column: "UserHealthConditionId1",
                principalTable: "UserHealthConditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
