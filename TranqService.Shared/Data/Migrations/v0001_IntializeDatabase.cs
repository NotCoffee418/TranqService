namespace TranqService.Shared.Data.Migrations;
public class v0001_IntializeDatabase : IMigration
{
    public int DbVersion => 1;

    public string MigrationSql => @"
        CREATE TABLE processedyoutubevideos
        (
            id              serial          PRIMARY KEY,
            videoid         varchar(32)     NOT NULL
        );
    ";
}