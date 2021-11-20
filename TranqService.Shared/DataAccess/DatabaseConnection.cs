﻿namespace TranqService.Shared.DataAccess
{
    public class DatabaseConnection : IDatabaseConnection
    {
        private const string connStr = "Server=db;Port=5432;Database=TranqService.Database;Userid=TranqService;Password=kXakVYj7WEZYQgfH;Include Error Detail=true";
        private static bool IsUpgraded { get; set; } = false;
        private static bool UpgradeInProgress { get; set; } = false;

        public async Task<NpgsqlConnection> GetConnectionAsync()
        {
            // Open connection
            var connection = new NpgsqlConnection(connStr);

            // Run upgrader
            if (!IsUpgraded)
            {
                // Only allow upgrader to run on one instance
                if (UpgradeInProgress)
                    while (UpgradeInProgress)
                        await Task.Delay(100);
                else
                {
                    UpgradeInProgress = true;
                    await RunUpgradeAsync(connection);
                    IsUpgraded = true;
                    UpgradeInProgress = false;
                }
            }

            return connection;
        }

        private async Task RunUpgradeAsync(NpgsqlConnection connection)
        {
            // Upgrade database
            var upgrader = new DatabaseUpgrader<NpgsqlConnection>(connection);
            await upgrader.UpgradeAsyc();
        }
    }
}
