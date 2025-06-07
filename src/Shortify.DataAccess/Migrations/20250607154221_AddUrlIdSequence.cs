using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shortify.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlIdSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<long>(
                name: "url_id_seq",
                startValue: 1000L
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(name: "url_id_seq");
        }
    }
}
