using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Trips.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverCommitments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");

            migrationBuilder.CreateTable(
                name: "driver_commitments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DriverId = table.Column<Guid>(type: "uuid", nullable: false),
                    TripId = table.Column<Guid>(type: "uuid", nullable: false),
                    Period = table.Column<NpgsqlRange<DateTimeOffset>>(type: "tstzrange", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_driver_commitments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_driver_commitments_DriverId",
                table: "driver_commitments",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_driver_commitments_TripId",
                table: "driver_commitments",
                column: "TripId");

            migrationBuilder.Sql("""
                ALTER TABLE driver_commitments
                ADD CONSTRAINT no_overlap
                EXCLUDE USING gist ("DriverId" WITH =, "Period" WITH &&);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE driver_commitments DROP CONSTRAINT IF EXISTS no_overlap;");

            migrationBuilder.DropTable(
                name: "driver_commitments");
        }
    }
}
