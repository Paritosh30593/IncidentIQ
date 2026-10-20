using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IC.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTestEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Test",
                columns: table => new
                {
                    SampleText = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.Sql(
                "INSERT INTO [Test] ([SampleText]) VALUES (N'Sample Text A');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Test");
        }
    }
}
