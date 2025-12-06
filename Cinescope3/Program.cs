
using CineScope.Services;
using CineScope.Models;
using System;

DatabaseService.Initialize();
ApiService api = new ApiService();
WatchlistService watchlist = new WatchlistService();

int watchlistId = 1; // default simple watchlist

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
        Console.Write("\nSearch title: ");
        string title = Console.ReadLine();

        var results = await api.SearchMoviesAsync(title);

        Console.WriteLine("\n--- Results ---");
        int i = 1;
        foreach (var m in results)
        {
            Console.WriteLine($"{i}. {m.Title} ({m.Year})");
            i++;
        }

        Console.Write("\nPick a movie number to add to watchlist (0 = cancel): ");
        int pick = int.Parse(Console.ReadLine());
        if (pick > 0 && pick <= results.Count)
        {
            var chosen = results[pick - 1];
            watchlist.AddMovie(chosen, watchlistId);
            Console.WriteLine("✔ Added to watchlist!");
        }
    }
    else if (choice == "2")
    {
        Console.WriteLine("\n--- My Watchlist ---");

        var movies = watchlist.GetMovies(watchlistId);

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
    }
    else if (choice == "3")
    {
        break;
    }
    else
    {
        Console.WriteLine("Invalid choice.");
    }
}
