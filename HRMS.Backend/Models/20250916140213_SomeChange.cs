using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Backend.Migrations
{
    /// <inheritdoc />
    public partial class SomeChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_applicants_ApplicantId",
                table: "Interviews");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicantId",
                table: "Interviews",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ShortlistId",
                table: "Interviews",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_ShortlistId",
                table: "Interviews",
                column: "ShortlistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_applicants_ApplicantId",
                table: "Interviews",
                column: "ApplicantId",
                principalTable: "applicants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_shortlists_ShortlistId",
                table: "Interviews",
                column: "ShortlistId",
                principalTable: "shortlists",
                principalColumn: "ShortlistID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_applicants_ApplicantId",
                table: "Interviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_shortlists_ShortlistId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_ShortlistId",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "ShortlistId",
                table: "Interviews");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicantId",
                table: "Interviews",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_applicants_ApplicantId",
                table: "Interviews",
                column: "ApplicantId",
                principalTable: "applicants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
