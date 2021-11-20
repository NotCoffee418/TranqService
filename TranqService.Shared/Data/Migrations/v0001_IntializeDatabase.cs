namespace TranqService.Shared.Data.Migrations;
public class v0001_IntializeDatabase : IMigration
{
    public int DbVersion => 1;

    public string MigrationSql => @"
        CREATE TABLE youtube_processed_videos
        (
            id              serial          PRIMARY KEY,
            videoguid       varchar(32)     NOT NULL,
            playlistid      integer         NOT NULL,
            nodeid          varchar(16)     UNIQUE,

            UNIQUE(videoguid, playlistid)
        );

        CREATE TABLE youtube_playlists
        (
            id              serial          PRIMARY KEY,
            playlistguid    varchar(64)     UNIQUE NOT NULL
        );
    ";
}