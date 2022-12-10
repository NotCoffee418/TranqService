using TranqService.Common.Models.Configs;

namespace TranqService.Database;

public class TranqDbContext : DbContext
{    
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
        optionsBuilder.UseSqlite("Data Source=" + AppPaths.Get().DatabasePath);
    }

}