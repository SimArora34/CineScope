using dotenv.net;
using System;
using CineScope.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CineScope.Services
{
    public class ApiService
    {
        private readonly string apiKey;
        private readonly HttpClient http = new HttpClient();

        // ✅ Constructor loads .env and retrieves API key
        public ApiService()
        {
            // Load environment variables from the .env file
            DotEnv.Load();

            // Read your TMDb key from .env
            apiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException(
                    "TMDB_API_KEY not found. Please create a .env file with TMDB_API_KEY=your_key_here and set it to 'Copy if newer'."
                );
            }
        }

        // ✅ Search for movies using the TMDb API
        public async Task<List<Movie>> SearchMoviesAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Search title cannot be empty.", nameof(title));

            string encoded = Uri.EscapeDataString(title);
            string url = $"https://api.themoviedb.org/3/search/movie?query={encoded}&api_key={apiKey}";

            try
            {
                string json = await http.GetStringAsync(url);
                JObject data = JObject.Parse(json);

                var list = new List<Movie>();

                foreach (var m in data["results"])
                {
                    list.Add(new Movie
                    {
                        Id = (int)m["id"],
                        Title = (string)m["title"],
                        Year = ((string)m["release_date"])?.Split('-')[0],
                        Overview = (string)m["overview"],
                        PosterUrl = "https://image.tmdb.org/t/p/w200" + (string)m["poster_path"]
                    });
                }

                return list;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error while contacting TMDb: {ex.Message}");
                return new List<Movie>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return new List<Movie>();
            }
        }
    }
}
