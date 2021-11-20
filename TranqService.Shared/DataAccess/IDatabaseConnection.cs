
namespace TranqService.Shared.DataAccess
{
    public interface IDatabaseConnection
    {
        Task<NpgsqlConnection> GetConnectionAsync();
    }
}