using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentAndSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessId = table.Column<int>(type: "int", nullable: false),
                    PayTRUserToken = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PayTRCardToken = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CardBrand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaskedCardNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MonthlyAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SubscriptionStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NextBillingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastBillingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubscriptionStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubscriptionEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessSubscriptions_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessId = table.Column<int>(type: "int", nullable: false),
                    MerchantOid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    NextRetryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxRetries = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PayTRResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayTRTransactionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessSubscriptions_BusinessId",
                table: "BusinessSubscriptions",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BusinessId",
                table: "Payments",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_MerchantOid",
                table: "Payments",
                column: "MerchantOid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessSubscriptions");

            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
