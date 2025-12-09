using System.IO;
using System.Data.SQLite;
using System;

namespace CineScope.Services
{
    public static class DatabaseService
    {
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cine.db");
        private static readonly string Conn = $"Data Source={DbPath}";


        public static void Initialize()
        {
            // Create DB file if missing
            // Create DB file if missing
            if (!File.Exists(DbPath))
                SQLiteConnection.CreateFile(DbPath);


            using var con = new SQLiteConnection(Conn);
            con.Open();

            string users = @"CREATE TABLE IF NOT EXISTS Users
                            (Id INTEGER PRIMARY KEY AUTOINCREMENT,
                             Name TEXT NOT NULL);";

            string watchlists = @"CREATE TABLE IF NOT EXISTS Watchlists
                                (Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                 UserId INTEGER,
                                 Name TEXT);";

            string movies = @"CREATE TABLE IF NOT EXISTS Movies
                              (Id INTEGER,
                               Title TEXT,
                               Year TEXT,
                               Poster TEXT,
                               Overview TEXT,
                               WatchlistId INTEGER);";

            new SQLiteCommand(users, con).ExecuteNonQuery();
            new SQLiteCommand(watchlists, con).ExecuteNonQuery();
            new SQLiteCommand(movies, con).ExecuteNonQuery();

            // Ensure a default user + watchlist exist
            string defaultUser = @"INSERT OR IGNORE INTO Users(Id, Name)
                                   VALUES(1, 'DefaultUser');";

            string defaultWatchlist = @"INSERT OR IGNORE INTO Watchlists(Id, UserId, Name)
                                        VALUES(1, 1, 'Default');";

            new SQLiteCommand(defaultUser, con).ExecuteNonQuery();
            new SQLiteCommand(defaultWatchlist, con).ExecuteNonQuery();
        }
    }
}
