using System.Data.SQLite;

namespace CineScope.Services
{
    public static class DatabaseService
    {
        private static string Conn = "Data Source=cine.db";

        public static void Initialize()
        {
            if (!File.Exists("cine.db"))
                SQLiteConnection.CreateFile("cine.db");

            using var con = new SQLiteConnection(Conn);
            con.Open();

            string users = @"CREATE TABLE IF NOT EXISTS Users
                            (Id INTEGER PRIMARY KEY AUTOINCREMENT,
                             Name TEXT NOT NULL);";

            string watchlist = @"CREATE TABLE IF NOT EXISTS Watchlists
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
            new SQLiteCommand(watchlist, con).ExecuteNonQuery();
            new SQLiteCommand(movies, con).ExecuteNonQuery();
        }
    }
}
