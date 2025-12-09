using dotenv.net;
using System;
using System.Threading.Tasks;
using CineScope.Services;
using CineScope.Models;

namespace Cinescope3
{
    internal class Program
    {
        // Async Main so we can await API calls
        static async Task Main(string[] args)
        {
            // ✅ 1. Load .env and verify TMDb key
            DotEnv.Load();
            string key = Environment.GetEnvironmentVariable("TMDB_API_KEY");

            Console.WriteLine("=== TMDb API Key Check ===");
            if (!string.IsNullOrWhiteSpace(key))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"✔ TMDB_API_KEY loaded (length: {key.Length} characters).");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ TMDB_API_KEY not found. Check your .env file.");
                Console.WriteLine("   It should contain: TMDB_API_KEY=your_real_key_here");
            }
            Console.ResetColor();

            // ✅ 2. Ensure database and tables exist
            DatabaseService.Initialize();

            // ✅ 3. Create service objects
            var api = new ApiService();
            var watchlistService = new WatchlistService();

            int watchlistId = 1; // default simple watchlist

            // ✅ 4. Main menu loop
            while (true)
            {
                Console.WriteLine("\n=== C I N E S C O P E ===");
                Console.WriteLine("1. Search for a Movie");
                Console.WriteLine("2. View Watchlist");
                Console.WriteLine("3. Exit");
                Console.Write("Choose > ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    await SearchFlow(api, watchlistService, watchlistId);
                }
                else if (choice == "2")
                {
                    ShowWatchlist(watchlistService, watchlistId);
                }
                else if (choice == "3")
                {
                    break; // exit app
                }
                else
                {
                    Console.WriteLine("Invalid choice. Press Enter to continue.");
                    Console.ReadLine();
                }
            }
        }

        private static async Task SearchFlow(ApiService api, WatchlistService watchlistService, int watchlistId)
        {
            Console.Write("\nSearch title: ");
            string title = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Title cannot be empty. Press Enter to return.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nSearching TMDb...");
            try
            {
                var results = await api.SearchMoviesAsync(title);

                Console.WriteLine("\n--- Results ---");
                if (results.Count == 0)
                {
                    Console.WriteLine("No movies found.");
                    Console.WriteLine("Press Enter to return.");
                    Console.ReadLine();
                    return;
                }

                int i = 1;
                foreach (var m in results)
                {
                    Console.WriteLine($"{i}. {m.Title} ({m.Year})");
                    i++;
                }

                Console.Write("\nPick a movie number to add to watchlist (0 = cancel): ");
                if (int.TryParse(Console.ReadLine(), out int pick) &&
                    pick > 0 && pick <= results.Count)
                {
                    var chosen = results[pick - 1];
                    watchlistService.AddMovie(chosen, watchlistId);
                    Console.WriteLine("✔ Added to watchlist!");
                }
                else
                {
                    Console.WriteLine("Cancelled or invalid choice.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError talking to TMDb: {ex.Message}");
            }

            Console.WriteLine("Press Enter to return to menu.");
            Console.ReadLine();
        }

        private static void ShowWatchlist(WatchlistService watchlistService, int watchlistId)
        {
            Console.WriteLine("\n--- My Watchlist ---");

            var movies = watchlistService.GetMovies(watchlistId);

            if (movies.Count == 0)
            {
                Console.WriteLine("Your watchlist is empty.");
            }
            else
            {
                foreach (var m in movies)
                {
                    Console.WriteLine($"{m.Title} ({m.Year})");
                }
            }

            Console.WriteLine("Press Enter to return to menu.");
            Console.ReadLine();
        }
    }
}
