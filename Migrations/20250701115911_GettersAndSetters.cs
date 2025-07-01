using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthConditionForecast.Migrations
{
    /// <inheritdoc />
    public partial class GettersAndSetters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Symptoms_HealthConditions_HealthConditionId1",
                table: "Symptoms");

            migrationBuilder.RenameColumn(
                name: "HealthConditionId1",
                table: "Symptoms",
                newName: "UserSymptomSelectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Symptoms_HealthConditionId1",
                table: "Symptoms",
                newName: "IX_Symptoms_UserSymptomSelectionId");

            migrationBuilder.AddColumn<int>(
                name: "UserHealthConditionId",
                table: "UserSymptomSelections",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "UserHealthConditionId1",
                table: "UserSymptomSelections",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "HealthConditionId",
                table: "UserHealthConditions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "HealthConditionId1",
                table: "UserHealthConditions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserHealthConditions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ArthritisSymtom_UserSymptomSelectionId",
                table: "Symptoms",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HealthConditionId",
                table: "Symptoms",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MigraineSympton_UserSymptomSelectionId",
                table: "Symptoms",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Symptoms",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "Symptoms",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "HealthConditions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "HealthConditions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "Forecasts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "categoryValue",
                table: "Forecasts",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "date",
                table: "Forecasts",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "link",
                table: "Forecasts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "mobileLink",
                table: "Forecasts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "Forecasts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "value",
                table: "Forecasts",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

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

            migrationBuilder.CreateIndex(
                name: "IX_Symptoms_ArthritisSymtom_UserSymptomSelectionId",
                table: "Symptoms",
                column: "ArthritisSymtom_UserSymptomSelectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Symptoms_HealthConditionId",
                table: "Symptoms",
                column: "HealthConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Symptoms_MigraineSympton_UserSymptomSelectionId",
                table: "Symptoms",
                column: "MigraineSympton_UserSymptomSelectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Symptoms_HealthConditions_HealthConditionId",
                table: "Symptoms",
                column: "HealthConditionId",
                principalTable: "HealthConditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Symptoms_UserSymptomSelections_ArthritisSymtom_UserSymptomSelectionId",
                table: "Symptoms",
                column: "ArthritisSymtom_UserSymptomSelectionId",
                principalTable: "UserSymptomSelections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Symptoms_UserSymptomSelections_MigraineSympton_UserSymptomSelectionId",
                table: "Symptoms",
                column: "MigraineSympton_UserSymptomSelectionId",
                principalTable: "UserSymptomSelections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Symptoms_UserSymptomSelections_UserSymptomSelectionId",
                table: "Symptoms",
                column: "UserSymptomSelectionId",
                principalTable: "UserSymptomSelections",
                principalColumn: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Symptoms_HealthConditions_HealthConditionId",
                table: "Symptoms");

            migrationBuilder.DropForeignKey(
                name: "FK_Symptoms_UserSymptomSelections_ArthritisSymtom_UserSymptomSelectionId",
                table: "Symptoms");

            migrationBuilder.DropForeignKey(
                name: "FK_Symptoms_UserSymptomSelections_MigraineSympton_UserSymptomSelectionId",
                table: "Symptoms");

            migrationBuilder.DropForeignKey(
                name: "FK_Symptoms_UserSymptomSelections_UserSymptomSelectionId",
                table: "Symptoms");

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

            migrationBuilder.DropIndex(
                name: "IX_Symptoms_ArthritisSymtom_UserSymptomSelectionId",
                table: "Symptoms");

            migrationBuilder.DropIndex(
                name: "IX_Symptoms_HealthConditionId",
                table: "Symptoms");

            migrationBuilder.DropIndex(
                name: "IX_Symptoms_MigraineSympton_UserSymptomSelectionId",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "UserHealthConditionId",
                table: "UserSymptomSelections");

            migrationBuilder.DropColumn(
                name: "UserHealthConditionId1",
                table: "UserSymptomSelections");

            migrationBuilder.DropColumn(
                name: "HealthConditionId",
                table: "UserHealthConditions");

            migrationBuilder.DropColumn(
                name: "HealthConditionId1",
                table: "UserHealthConditions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserHealthConditions");

            migrationBuilder.DropColumn(
                name: "ArthritisSymtom_UserSymptomSelectionId",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "HealthConditionId",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "MigraineSympton_UserSymptomSelectionId",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "description",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "name",
                table: "Symptoms");

            migrationBuilder.DropColumn(
                name: "description",
                table: "HealthConditions");

            migrationBuilder.DropColumn(
                name: "name",
                table: "HealthConditions");

            migrationBuilder.DropColumn(
                name: "category",
                table: "Forecasts");

            migrationBuilder.DropColumn(
                name: "categoryValue",
                table: "Forecasts");

            migrationBuilder.DropColumn(
                name: "date",
                table: "Forecasts");

            migrationBuilder.DropColumn(
                name: "link",
                table: "Forecasts");

            migrationBuilder.DropColumn(
                name: "mobileLink",
                table: "Forecasts");

            migrationBuilder.DropColumn(
                name: "name",
                table: "Forecasts");

            migrationBuilder.DropColumn(
                name: "value",
                table: "Forecasts");

            migrationBuilder.RenameColumn(
                name: "UserSymptomSelectionId",
                table: "Symptoms",
                newName: "HealthConditionId1");

            migrationBuilder.RenameIndex(
                name: "IX_Symptoms_UserSymptomSelectionId",
                table: "Symptoms",
                newName: "IX_Symptoms_HealthConditionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Symptoms_HealthConditions_HealthConditionId1",
                table: "Symptoms",
                column: "HealthConditionId1",
                principalTable: "HealthConditions",
                principalColumn: "Id");
        }
    }
}
