using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranqService.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddErrorColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "YoutubeVideoInfos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "YoutubeVideoInfos");
        }
    }
}
