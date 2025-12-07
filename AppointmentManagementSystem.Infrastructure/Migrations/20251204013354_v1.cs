using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 1, 33, 53, 706, DateTimeKind.Utc).AddTicks(9005));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 1, 33, 53, 706, DateTimeKind.Utc).AddTicks(9007));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 1, 33, 53, 706, DateTimeKind.Utc).AddTicks(9009));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 1, 33, 53, 706, DateTimeKind.Utc).AddTicks(9010));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 1, 31, 25, 776, DateTimeKind.Utc).AddTicks(6074));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 1, 31, 25, 776, DateTimeKind.Utc).AddTicks(6077));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 1, 31, 25, 776, DateTimeKind.Utc).AddTicks(6078));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 1, 31, 25, 776, DateTimeKind.Utc).AddTicks(6080));
        }
    }
}
