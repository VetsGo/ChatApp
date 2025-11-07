using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Backend.Migrations
{
    /// <inheritdoc />
    public partial class SentimentAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "NegativeScore",
                table: "ChatMessages",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NeutralScore",
                table: "ChatMessages",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PositiveScore",
                table: "ChatMessages",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sentiment",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NegativeScore",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "NeutralScore",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "PositiveScore",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "Sentiment",
                table: "ChatMessages");
        }
    }
}
