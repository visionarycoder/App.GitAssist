namespace vc.GitHelper;

public static class ConsoleHelper
{

    private static readonly string line = new('-', 80);

    public static void DisplayHeader()
    {
        Console.WriteLine(line);
        Console.WriteLine("VisionaryCoder: Git Helper");
        Console.WriteLine(line);
    }

    public static string RequestInput(string message, string suggested)
    {

        do
        {
            Console.Write($"{message}");
            var input = Console.ReadLine();
            input = input?.Trim();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            Console.WriteLine("Path is empty");
        } while (true);

    }

    public static void DisplayUpdate(string message, bool isError = false)
    {

        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        if (isError)
        {
            var backgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = Console.ForegroundColor;
            Console.ForegroundColor = backgroundColor;
            Console.WriteLine($"{message}");
            Console.ForegroundColor = Console.BackgroundColor;
            Console.BackgroundColor = backgroundColor;
        }
        else
        {
            Console.WriteLine(message);
        }

    }

    public static void DisplayUpdate(string message, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
    {

        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        var currentForegroundColor = Console.ForegroundColor;
        var currentBackgroundColor = Console.BackgroundColor;

        Console.ForegroundColor = foregroundColor;
        Console.BackgroundColor = backgroundColor;

        Console.WriteLine(message);

        Console.ForegroundColor = currentForegroundColor;
        Console.BackgroundColor = currentBackgroundColor;

    }

    public static void DisplayExit()
    {
        Console.WriteLine(line);
        Console.WriteLine("Press any key to exit");
        Console.WriteLine(line);
        Console.ReadKey();
    }

}