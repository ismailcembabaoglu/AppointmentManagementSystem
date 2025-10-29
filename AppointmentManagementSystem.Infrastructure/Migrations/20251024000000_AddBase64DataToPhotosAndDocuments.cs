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
            // Add Base64Data column to Photo table (TPH: BusinessPhoto, EmployeePhoto, ServicePhoto, AppointmentPhoto)
            migrationBuilder.AddColumn<string>(
                name: "Base64Data",
                table: "Photo",
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
            // Remove Base64Data column from Photo table
            migrationBuilder.DropColumn(
                name: "Base64Data",
                table: "Photo");

            // Remove Base64Data column from EmployeeDocuments table
            migrationBuilder.DropColumn(
                name: "Base64Data",
                table: "EmployeeDocuments");
        }
    }
}
