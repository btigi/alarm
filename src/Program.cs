using Microsoft.Extensions.Configuration;
using NAudio.Wave;
using System.Text.RegularExpressions;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowUsage();
            return;
        }

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        string? audioFile = null;
        string? timeOffset = null;
        string? message = null;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "-t":
                    if (i + 1 < args.Length)
                    {
                        timeOffset = args[++i];
                    }
                    break;
                case "-f":
                    if (i + 1 < args.Length)
                    {
                        audioFile = args[++i];
                    }
                    break;
                case "-m":
                    // Collect all remaining arguments as the message
                    message = string.Join(" ", args.Skip(i + 1));
                    i = args.Length; // Skip remaining arguments
                    break;
            }
        }

        // Validate required parameters
        if (string.IsNullOrEmpty(timeOffset))
        {
            Console.WriteLine("Error: Time offset (-t) is required.");
            ShowUsage();
            return;
        }

        audioFile ??= config["DefaultAudioFile"];

        if (string.IsNullOrEmpty(audioFile))
        {
            Console.WriteLine("Error: No audio file specified and no default audio file configured.");
            return;
        }

        var (seconds, isValid) = ParseTimeOffset(timeOffset);
        if (!isValid)
        {
            Console.WriteLine("Error: Invalid time offset format. Use format like '5m' or '30s'.");
            return;
        }

        Console.WriteLine($"Alarm will trigger in {seconds} seconds...");
        Console.WriteLine($"Audio file: {audioFile}");
        if (!string.IsNullOrEmpty(message))
        {
            Console.WriteLine($"Message: {message}");
        }

        await Task.Delay(TimeSpan.FromSeconds(seconds));

        try
        {
            using var audioFileReader = new AudioFileReader(audioFile);
            using var outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFileReader);
            outputDevice.Play();

            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine($"\nALARM: {message}");
            }
            else
            {
                Console.WriteLine("\nALARM!");
            }

            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                await Task.Delay(100);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error playing audio file: {ex.Message}");
        }
    }

    static void ShowUsage()
    {
        Console.WriteLine("Usage: AlarmApp -t <time> [-f <audiofile>] [-m <message>]");
        Console.WriteLine("  -t: Time offset (e.g., 5m for 5 minutes, 30s for 30 seconds)");
        Console.WriteLine("  -f: Audio file to play (optional, uses default from appsettings.json if not specified)");
        Console.WriteLine("  -m: Message to display (optional)");
    }

    static (int seconds, bool isValid) ParseTimeOffset(string timeOffset)
    {
        var match = Regex.Match(timeOffset, @"^(\d+)([sm])$");
        if (!match.Success)
        {
            return (0, false);
        }

        var value = int.Parse(match.Groups[1].Value);
        var unit = match.Groups[2].Value;

        return unit switch
        {
            "s" => (value, true),
            "m" => (value * 60, true),
            _ => (0, false)
        };
    }
}