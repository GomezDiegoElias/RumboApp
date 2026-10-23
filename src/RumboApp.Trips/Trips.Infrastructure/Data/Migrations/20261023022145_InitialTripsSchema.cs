using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trips.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialTripsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trips",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PassengerId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginLatitude = table.Column<double>(type: "double precision", nullable: false),
                    OriginLongitude = table.Column<double>(type: "double precision", nullable: false),
                    DestinationLatitude = table.Column<double>(type: "double precision", nullable: false),
                    DestinationLongitude = table.Column<double>(type: "double precision", nullable: false),
                    State = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EstimatedFareCents = table.Column<long>(type: "bigint", nullable: false),
                    EstimatedFareCurrency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    FinalFareCents = table.Column<long>(type: "bigint", nullable: true),
                    FinalFareCurrency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: true),
                    DriverId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trips", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trips_PassengerId_State",
                table: "trips",
                columns: new[] { "PassengerId", "State" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trips");
        }
    }
}
