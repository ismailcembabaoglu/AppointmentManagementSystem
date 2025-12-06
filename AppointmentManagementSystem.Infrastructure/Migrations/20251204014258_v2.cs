using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 1, 42, 57, 706, DateTimeKind.Utc).AddTicks(8150));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 1, 42, 57, 706, DateTimeKind.Utc).AddTicks(8154));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 1, 42, 57, 706, DateTimeKind.Utc).AddTicks(8299));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 4, 1, 42, 57, 706, DateTimeKind.Utc).AddTicks(8302));

            migrationBuilder.CreateTable(
       name: "AppointmentServices",
       columns: table => new
       {
           Id = table.Column<int>(type: "int", nullable: false)
               .Annotation("SqlServer:Identity", "1, 1"),
           AppointmentId = table.Column<int>(type: "int", nullable: false),
           ServiceId = table.Column<int>(type: "int", nullable: false),
           DurationMinutes = table.Column<int>(type: "int", nullable: false),
           Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
           ServiceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
           CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
           UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
           IsDeleted = table.Column<bool>(type: "bit", nullable: false)
       },
       constraints: table =>
       {
           table.PrimaryKey("PK_AppointmentServices", x => x.Id);
           table.ForeignKey(
               name: "FK_AppointmentServices_Appointments_AppointmentId",
               column: x => x.AppointmentId,
               principalTable: "Appointments",
               principalColumn: "Id",
               onDelete: ReferentialAction.NoAction);
           table.ForeignKey(
               name: "FK_AppointmentServices_Services_ServiceId",
               column: x => x.ServiceId,
               principalTable: "Services",
               principalColumn: "Id",
               onDelete: ReferentialAction.NoAction);
       });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentServices_ServiceId",
                table: "AppointmentServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentServices_AppointmentId_ServiceId",
                table: "AppointmentServices",
                columns: new[] { "AppointmentId", "ServiceId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
            migrationBuilder.DropTable(
        name: "AppointmentServices");
        
    }
    }
}
