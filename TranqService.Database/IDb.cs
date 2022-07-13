namespace TranqService.Database
{
    public interface IDb
    {
        TranqDbContext CreateDbContext(string[] args);
        TranqDbContext GetContext();
    }
}