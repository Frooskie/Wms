using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckConstraintsToBatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Batches_ExpiryAfterProduction",
                table: "Batches",
                sql: "\"ExpiryDate\" > \"ProductionDate\"");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Batches_ReservedNotExceedQuantity",
                table: "Batches",
                sql: "\"ReservedQuantity\" <= \"Quantity\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Batches_ExpiryAfterProduction",
                table: "Batches");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Batches_ReservedNotExceedQuantity",
                table: "Batches");
        }
    }
}
