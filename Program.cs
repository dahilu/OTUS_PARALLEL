using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();


        string?[] files = { configuration["Task1:File1"], configuration["Task1:File2"], configuration["Task1:File3"] };

        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();

        Task<int>[] tasks = files.Select(file => Task.Run(() => ReadAndCountSpacesAsync(file ?? ""))).ToArray(); // Параллельное чтение и подсчет пробелов

        int[] results = await Task.WhenAll(tasks);

        int totalSpaces = results.Sum();

        stopwatch.Stop();

        Console.WriteLine($"Общее количество пробелов: {totalSpaces}. Время: {stopwatch.ElapsedMilliseconds} мс");





        string dir = AppDomain.CurrentDomain.BaseDirectory;

        stopwatch.Restart();

        totalSpaces = await CountSpacesInDirectoryAsync(dir);

        stopwatch.Stop();

        Console.WriteLine($"Общее количество пробелов во всех файлах: {totalSpaces}");

        Console.WriteLine($"Время выполнения: {stopwatch.ElapsedMilliseconds} мс");

        Console.ReadLine();

    }

    static async Task<int> ReadAndCountSpacesAsync(string filePath)
    {
        int res = 0;

        if (filePath == "")
        {
            return 0;
        }

        using (StreamReader reader = new StreamReader(filePath))
        {
            string content = await reader.ReadToEndAsync();

            res = content.Count(c => c == ' ');
            
            Console.WriteLine($"{res}");

            return res;
        }
    }


    static async Task<int> CountSpacesInDirectoryAsync(string directoryPath)
    {
        string[] filePaths = Directory.GetFiles(directoryPath, "*.txt");

        Task<int>[] tasks = filePaths.Select(ReadAndCountSpacesAsync).ToArray();

        int[] results = await Task.WhenAll(tasks);

        return results.Sum();
    }
}







