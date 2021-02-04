using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebMaze.Migrations
{
    public partial class deleted_PoliceCertificate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CitizenUserPoliceCertificate");

            migrationBuilder.DropTable(
                name: "PoliceCertificates");

            migrationBuilder.AlterColumn<int>(
                name: "Rank",
                table: "Policemen",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Rank",
                table: "Policemen",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "PoliceCertificates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfIssue = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Speciality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Validity = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoliceCertificates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CitizenUserPoliceCertificate",
                columns: table => new
                {
                    PoliceCertificatesId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenUserPoliceCertificate", x => new { x.PoliceCertificatesId, x.UserId });
                    table.ForeignKey(
                        name: "FK_CitizenUserPoliceCertificate_CitizenUser_UserId",
                        column: x => x.UserId,
                        principalTable: "CitizenUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitizenUserPoliceCertificate_PoliceCertificates_PoliceCertificatesId",
                        column: x => x.PoliceCertificatesId,
                        principalTable: "PoliceCertificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CitizenUserPoliceCertificate_UserId",
                table: "CitizenUserPoliceCertificate",
                column: "UserId");
        }
    }
}
