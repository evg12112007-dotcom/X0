using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.IO;

namespace X0.DB
{
    internal class ManagmentDB
    {
        private static string DbPath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "game_stats.db");

        public static void Initialize()
        {
            SqliteConnection connection = new SqliteConnection($"Data Source={DbPath}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS GameStats (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Level TEXT,
                Result TEXT,
                Date TEXT
            );
            ";
            cmd.ExecuteNonQuery();
        }

        public static void AddGame(string level, string result)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source={DbPath}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText =
            @"
            INSERT INTO GameStats (Level, Result, Date)
            VALUES ($level, $result, $date);
            ";

            cmd.Parameters.AddWithValue("$level", level);
            cmd.Parameters.AddWithValue("$result", result);
            cmd.Parameters.AddWithValue("$date", DateTime.Now.ToString("dd.MM.yyyy HH:mm"));

            cmd.ExecuteNonQuery();
        }

        public static SqliteDataReader GetStats()
        {
            var connection = new SqliteConnection($"Data Source={DbPath}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Level, Result, Date FROM GameStats";

            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
    }
}
