using CineScope.Models;
using System.Collections.Generic;
using System;
using System.Data.SQLite;

namespace CineScope.Services
{
    public class WatchlistService
    {
        private string Conn = "Data Source=cine.db";

        public void AddMovie(Movie m, int watchlistId)
        {
            using var con = new SQLiteConnection(Conn);
            con.Open();

            string sql = @"INSERT INTO Movies(Id, Title, Year, Poster, Overview, WatchlistId)
                           VALUES(@id, @title, @year, @poster, @overview, @wid);";

            var cmd = new SQLiteCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", m.Id);
            cmd.Parameters.AddWithValue("@title", m.Title);
            cmd.Parameters.AddWithValue("@year", m.Year);
            cmd.Parameters.AddWithValue("@poster", m.PosterUrl);
            cmd.Parameters.AddWithValue("@overview", m.Overview);
            cmd.Parameters.AddWithValue("@wid", watchlistId);

            cmd.ExecuteNonQuery();
        }

        public List<Movie> GetMovies(int watchlistId)
        {
            var list = new List<Movie>();

            using var con = new SQLiteConnection(Conn);
            con.Open();

            string sql = "SELECT * FROM Movies WHERE WatchlistId=@wid";

            var cmd = new SQLiteCommand(sql, con);
            cmd.Parameters.AddWithValue("@wid", watchlistId);

            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(new Movie
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Title = (string)r["Title"],
                    Year = (string)r["Year"],
                    PosterUrl = (string)r["Poster"],
                    Overview = (string)r["Overview"]
                });
            }

            return list;
        }
    }
}
