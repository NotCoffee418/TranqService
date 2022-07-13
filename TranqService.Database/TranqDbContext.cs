using TranqService.Common.Data;

namespace TranqService.Database;

public class TranqDbContext : DbContext
{
    private IConfiguration _configuration;

    /// <summary>
    /// Default constructor used by the application
    /// </summary>
    public TranqDbContext(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Empty constructor is used by EF to generate migrations
    /// </summary>
    public TranqDbContext() { }

    public DbSet<YoutubeVideoInfo> YoutubeVideoInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /// DexToken
        modelBuilder.Entity<YoutubeVideoInfo>()
            .HasIndex(x => new { x.VideoGuid, x.PlaylistGuid })
            .IsUnique();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string sqliteFilePath = _configuration.GetRequiredSection("Config:SqliteFilePath").Get<string>();
        optionsBuilder.UseSqlite("Data Source=" + sqliteFilePath);
    }

}