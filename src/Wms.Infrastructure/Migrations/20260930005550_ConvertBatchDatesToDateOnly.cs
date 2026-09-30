using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertBatchDatesToDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Batches\" ALTER COLUMN \"ProductionDate\" TYPE date USING \"ProductionDate\"::date;");
            migrationBuilder.Sql("ALTER TABLE \"Batches\" ALTER COLUMN \"ExpiryDate\" TYPE date USING \"ExpiryDate\"::date;");
            migrationBuilder.Sql("ALTER TABLE \"Batches\" ALTER COLUMN \"ReceivedDate\" TYPE date USING \"ReceivedDate\"::date;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Batches\" ALTER COLUMN \"ProductionDate\" TYPE timestamp with time zone USING \"ProductionDate\"::timestamp with time zone;");
            migrationBuilder.Sql("ALTER TABLE \"Batches\" ALTER COLUMN \"ExpiryDate\" TYPE timestamp with time zone USING \"ExpiryDate\"::timestamp with time zone;");
            migrationBuilder.Sql("ALTER TABLE \"Batches\" ALTER COLUMN \"ReceivedDate\" TYPE timestamp with time zone USING \"ReceivedDate\"::timestamp with time zone;");
        }
    }
}
