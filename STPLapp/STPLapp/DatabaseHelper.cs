using System;
using System.IO;

namespace STPLapp
{
    public static class DatabaseHelper
    {
        private static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db_config.txt");
        private const string DefaultConnectionString = "Server=localhost;database=SI_STPL_DB;UID=root;Password=21914113";

        public static string ConnectionString
        {
            get
            {
                if (!File.Exists(ConfigFilePath))
                {
                    File.WriteAllText(ConfigFilePath, DefaultConnectionString);
                }
                return File.ReadAllText(ConfigFilePath);
            }
        }

        public static void SaveConnectionString(string host, string dbName, string username, string password)
        {
            string newConnString = $"Server={host};database={dbName};UID={username};Password={password}";
            File.WriteAllText(ConfigFilePath, newConnString);
        }
    }
}