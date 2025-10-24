using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBase64DataToPhotosAndDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Base64Data column to Photos table (for BusinessPhoto, EmployeePhoto, ServicePhoto, AppointmentPhoto)
            migrationBuilder.AddColumn<string>(
                name: "Base64Data",
                table: "Photos",
                type: "nvarchar(max)",
                nullable: true);

            // Add Base64Data column to EmployeeDocuments table
            migrationBuilder.AddColumn<string>(
                name: "Base64Data",
                table: "EmployeeDocuments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove Base64Data column from Photos table
            migrationBuilder.DropColumn(
                name: "Base64Data",
                table: "Photos");

            // Remove Base64Data column from EmployeeDocuments table
            migrationBuilder.DropColumn(
                name: "Base64Data",
                table: "EmployeeDocuments");
        }
    }
}
