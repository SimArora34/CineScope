using CineScope.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CineScope.Services
{
    public class ApiService
    {
        private readonly string apiKey = "YOUR_API_KEY";
        private readonly HttpClient http = new HttpClient();

        public async Task<List<Movie>> SearchMoviesAsync(string title)
        {
            string url = $"https://api.themoviedb.org/3/search/movie?query={title}&api_key={apiKey}";

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
    }
}
