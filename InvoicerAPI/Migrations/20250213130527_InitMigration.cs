using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoicerAPI.Migrations
{
	/// <inheritdoc />
	public partial class InitMigration : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Invoices",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					TotalSum = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
					Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Status = table.Column<int>(type: "int", nullable: false),
					CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Invoices", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
					PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
					CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
					DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.Id);
				});

			migrationBuilder.CreateTable(
				name: "InvoiceRows",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Service = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Quantity = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
					Amount = table.Column<decimal>(type: "decimal(18,5)", nullable: false),
					Sum = table.Column<decimal>(type: "decimal(18,5)", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_InvoiceRows", x => x.Id);
					table.ForeignKey(
						name: "FK_InvoiceRows_Invoices_InvoiceId",
						column: x => x.InvoiceId,
						principalTable: "Invoices",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_InvoiceRows_InvoiceId",
				table: "InvoiceRows",
				column: "InvoiceId");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "InvoiceRows");

			migrationBuilder.DropTable(
				name: "Users");

			migrationBuilder.DropTable(
				name: "Invoices");
		}
	}
}
