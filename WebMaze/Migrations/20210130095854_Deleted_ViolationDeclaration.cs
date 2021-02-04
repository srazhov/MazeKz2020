using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMaze.Migrations
{
    public partial class Deleted_ViolationDeclaration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Violations_CitizenUser_UserId",
                table: "Violations");

            migrationBuilder.DropForeignKey(
                name: "FK_Violations_Policemen_BlamingPolicemanId",
                table: "Violations");

            migrationBuilder.DropTable(
                name: "ViolationDeclarations");

            migrationBuilder.DropColumn(
                name: "Article",
                table: "Violations");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Violations",
                newName: "ViewingPolicemanId");

            migrationBuilder.RenameColumn(
                name: "Punishment",
                table: "Violations",
                newName: "Explanation");

            migrationBuilder.RenameColumn(
                name: "BlamingPolicemanId",
                table: "Violations",
                newName: "BlamingUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Violations_UserId",
                table: "Violations",
                newName: "IX_Violations_ViewingPolicemanId");

            migrationBuilder.RenameIndex(
                name: "IX_Violations_BlamingPolicemanId",
                table: "Violations",
                newName: "IX_Violations_BlamingUserId");

            migrationBuilder.AddColumn<long>(
                name: "BlamedUserId",
                table: "Violations",
                type: "bigint",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "OffenseType",
                table: "Violations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Violations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Violations_BlamedUserId",
                table: "Violations",
                column: "BlamedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Violations_CitizenUser_BlamedUserId",
                table: "Violations",
                column: "BlamedUserId",
                principalTable: "CitizenUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Violations_CitizenUser_BlamingUserId",
                table: "Violations",
                column: "BlamingUserId",
                principalTable: "CitizenUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Violations_Policemen_ViewingPolicemanId",
                table: "Violations",
                column: "ViewingPolicemanId",
                principalTable: "Policemen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Violations_CitizenUser_BlamedUserId",
                table: "Violations");

            migrationBuilder.DropForeignKey(
                name: "FK_Violations_CitizenUser_BlamingUserId",
                table: "Violations");

            migrationBuilder.DropForeignKey(
                name: "FK_Violations_Policemen_ViewingPolicemanId",
                table: "Violations");

            migrationBuilder.DropIndex(
                name: "IX_Violations_BlamedUserId",
                table: "Violations");

            migrationBuilder.DropColumn(
                name: "BlamedUserId",
                table: "Violations");

            migrationBuilder.DropColumn(
                name: "OffenseType",
                table: "Violations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Violations");

            migrationBuilder.RenameColumn(
                name: "ViewingPolicemanId",
                table: "Violations",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Explanation",
                table: "Violations",
                newName: "Punishment");

            migrationBuilder.RenameColumn(
                name: "BlamingUserId",
                table: "Violations",
                newName: "BlamingPolicemanId");

            migrationBuilder.RenameIndex(
                name: "IX_Violations_ViewingPolicemanId",
                table: "Violations",
                newName: "IX_Violations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Violations_BlamingUserId",
                table: "Violations",
                newName: "IX_Violations_BlamingPolicemanId");

            migrationBuilder.AddColumn<string>(
                name: "Article",
                table: "Violations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ViolationDeclarations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlamedUserId = table.Column<long>(type: "bigint", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OffenseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    ViewedPolicemanId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViolationDeclarations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViolationDeclarations_CitizenUser_BlamedUserId",
                        column: x => x.BlamedUserId,
                        principalTable: "CitizenUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ViolationDeclarations_CitizenUser_UserId",
                        column: x => x.UserId,
                        principalTable: "CitizenUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ViolationDeclarations_Policemen_ViewedPolicemanId",
                        column: x => x.ViewedPolicemanId,
                        principalTable: "Policemen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ViolationDeclarations_BlamedUserId",
                table: "ViolationDeclarations",
                column: "BlamedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationDeclarations_UserId",
                table: "ViolationDeclarations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationDeclarations_ViewedPolicemanId",
                table: "ViolationDeclarations",
                column: "ViewedPolicemanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Violations_CitizenUser_UserId",
                table: "Violations",
                column: "UserId",
                principalTable: "CitizenUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Violations_Policemen_BlamingPolicemanId",
                table: "Violations",
                column: "BlamingPolicemanId",
                principalTable: "Policemen",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
