using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassLibrary1.Migrations
{
    /// <inheritdoc />
    public partial class InitialDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffMember",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Role = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StaffMem__0CF04B3886BCD49E", x => x.MemberID);
                });

            migrationBuilder.CreateTable(
                name: "SupplierCompany",
                columns: table => new
                {
                    SupplierId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SupplierDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PlaceOfOrigin = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Supplier__4BE666B4D29D1BE3", x => x.SupplierId);
                });

            migrationBuilder.CreateTable(
                name: "AirConditioner",
                columns: table => new
                {
                    AirConditionerId = table.Column<int>(type: "int", nullable: false),
                    AirConditionerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Warranty = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    SoundPressureLevel = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    FeatureFunction = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    DollarPrice = table.Column<double>(type: "float", nullable: true),
                    SupplierId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__AirCondi__EE2EB739C63FB1A1", x => x.AirConditionerId);
                    table.ForeignKey(
                        name: "FK__AirCondit__Suppl__29572725",
                        column: x => x.SupplierId,
                        principalTable: "SupplierCompany",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirConditioner_SupplierId",
                table: "AirConditioner",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "UQ__StaffMem__49A14740C75E125A",
                table: "StaffMember",
                column: "EmailAddress",
                unique: true,
                filter: "[EmailAddress] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirConditioner");

            migrationBuilder.DropTable(
                name: "StaffMember");

            migrationBuilder.DropTable(
                name: "SupplierCompany");
        }
    }
}
