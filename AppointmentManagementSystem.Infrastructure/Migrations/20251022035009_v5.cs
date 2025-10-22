using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppointmentManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Icon", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 2, new DateTime(2025, 10, 22, 3, 50, 8, 937, DateTimeKind.Utc).AddTicks(4543), "Güzellik ve bakım hizmetleri", "spa", false, "Güzellik Merkezi", null },
                    { 3, new DateTime(2025, 10, 22, 3, 50, 8, 937, DateTimeKind.Utc).AddTicks(4546), "Diş sağlığı hizmetleri", "local_hospital", false, "Diş Hekimi", null },
                    { 4, new DateTime(2025, 10, 22, 3, 50, 8, 937, DateTimeKind.Utc).AddTicks(4548), "Tıbbi estetik hizmetleri", "healing", false, "Tıbbi Estetik", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
