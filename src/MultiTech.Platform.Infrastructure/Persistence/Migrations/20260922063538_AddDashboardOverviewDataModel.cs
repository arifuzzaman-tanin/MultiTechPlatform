using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiTech.Platform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDashboardOverviewDataModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dashboard");

            migrationBuilder.CreateTable(
                name: "Sites",
                schema: "dashboard",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                schema: "dashboard",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DeviceType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LastSeenAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsManaged = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "dashboard",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TelemetryMessageAggregates",
                schema: "dashboard",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BucketStartUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    BucketEndUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    MessageCount = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelemetryMessageAggregates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelemetryMessageAggregates_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "dashboard",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                schema: "dashboard",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ResolvedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alerts_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "dashboard",
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alerts_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "dashboard",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_CreatedAtUtc",
                schema: "dashboard",
                table: "Alerts",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_DeviceId",
                schema: "dashboard",
                table: "Alerts",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_Severity",
                schema: "dashboard",
                table: "Alerts",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_SiteId",
                schema: "dashboard",
                table: "Alerts",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_Status",
                schema: "dashboard",
                table: "Alerts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceType",
                schema: "dashboard",
                table: "Devices",
                column: "DeviceType");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_IsManaged",
                schema: "dashboard",
                table: "Devices",
                column: "IsManaged");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_LastSeenAtUtc",
                schema: "dashboard",
                table: "Devices",
                column: "LastSeenAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_SerialNumber",
                schema: "dashboard",
                table: "Devices",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_SiteId",
                schema: "dashboard",
                table: "Devices",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_Status",
                schema: "dashboard",
                table: "Devices",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_Code",
                schema: "dashboard",
                table: "Sites",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_IsActive",
                schema: "dashboard",
                table: "Sites",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryMessageAggregates_BucketStartUtc_BucketEndUtc",
                schema: "dashboard",
                table: "TelemetryMessageAggregates",
                columns: new[] { "BucketStartUtc", "BucketEndUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TelemetryMessageAggregates_SiteId",
                schema: "dashboard",
                table: "TelemetryMessageAggregates",
                column: "SiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerts",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "TelemetryMessageAggregates",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "Devices",
                schema: "dashboard");

            migrationBuilder.DropTable(
                name: "Sites",
                schema: "dashboard");
        }
    }
}
