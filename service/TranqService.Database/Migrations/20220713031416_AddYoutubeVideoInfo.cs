using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranqService.Database.Migrations
{
    public partial class AddYoutubeVideoInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YoutubeVideoInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VideoGuid = table.Column<string>(type: "TEXT", nullable: false),
                    PlaylistGuid = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Uploader = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YoutubeVideoInfos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_YoutubeVideoInfos_VideoGuid_PlaylistGuid",
                table: "YoutubeVideoInfos",
                columns: new[] { "VideoGuid", "PlaylistGuid" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YoutubeVideoInfos");
        }
    }
}
