using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Flowqueue_Backend.shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class InitialFlowQueueSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "institutions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    type = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_institutions", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "branch_offices",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    institution_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    address = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    district = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    schedule = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_branch_offices", x => x.id);
                    table.ForeignKey(
                        name: "f_k_branch_offices__institutions_institution_id",
                        column: x => x.institution_id,
                        principalTable: "institutions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "services",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    branch_office_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    average_duration_minutes = table.Column<int>(type: "int", nullable: false),
                    prefix = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_services", x => x.id);
                    table.ForeignKey(
                        name: "f_k_services_branch_offices_branch_office_id",
                        column: x => x.branch_office_id,
                        principalTable: "branch_offices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "turns",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    branch_office_id = table.Column<int>(type: "int", nullable: false),
                    service_id = table.Column<int>(type: "int", nullable: false),
                    citizen_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    citizen_document_number = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    turn_number = table.Column<int>(type: "int", nullable: false),
                    ticket_code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    registered_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    called_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    cancelled_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    marked_absent_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_turns", x => x.id);
                    table.ForeignKey(
                        name: "f_k_turns_branch_offices_branch_office_id",
                        column: x => x.branch_office_id,
                        principalTable: "branch_offices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_turns_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_branch_offices_institution_id_name",
                table: "branch_offices",
                columns: new[] { "institution_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_institutions_name",
                table: "institutions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_services_branch_office_id_name",
                table: "services",
                columns: new[] { "branch_office_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_turns_branch_office_id_service_id_turn_number",
                table: "turns",
                columns: new[] { "branch_office_id", "service_id", "turn_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_turns_service_id",
                table: "turns",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "i_x_turns_ticket_code",
                table: "turns",
                column: "ticket_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "turns");

            migrationBuilder.DropTable(
                name: "services");

            migrationBuilder.DropTable(
                name: "branch_offices");

            migrationBuilder.DropTable(
                name: "institutions");
        }
    }
}
