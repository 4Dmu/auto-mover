using System.Collections.Immutable;
using System.Text.Json;

namespace auto_mover
{

    public static class ProgramArgs
    {
        public const string WatchDir = "--watch-dir";
        public const string MoveDir = "--move-dir";
        public const string Filter = "--filter";
        public const string List = "--list";

        public static readonly ImmutableList<string> All = [WatchDir, MoveDir, Filter, List];
    }

    public static class Utils
    {
        public static WatchLocation? ParseLocationFromArgs(string? dirToWatch, string? dirToMoveTo, string? filter)
        {
            if (dirToWatch == null)
            {
                Console.Error.WriteLine($"{ProgramArgs.WatchDir} is missing or invalid");
                return null;
            }

            if (dirToMoveTo == null)
            {
                Console.Error.WriteLine($"{ProgramArgs.MoveDir} is missing or invalid");
                return null;
            }

            if (!Directory.Exists(dirToWatch))
            {
                Console.Error.WriteLine($"{ProgramArgs.WatchDir} is not a valid directory");
                return null;
            }

            if (!Directory.Exists(dirToMoveTo))
            {
                Console.Error.WriteLine($"{ProgramArgs.MoveDir} is not a valid directory");
                return null;
            }

            if (filter != null && (filter.Length > 500 || !filter.Contains('.')))
            {
                Console.Error.WriteLine($"{ProgramArgs.Filter} was provided but is invalid");
                return null;
            }

            var watch = new DirectoryInfo(dirToWatch);
            var move = new DirectoryInfo(dirToMoveTo);

            if (watch.FullName == move.FullName)
            {
                Console.Error.WriteLine($"{ProgramArgs.MoveDir} cannot be the same as {ProgramArgs.WatchDir}");
                return null;
            }

            return new WatchLocation(dirToWatch, dirToMoveTo, filter);
        }

        public static WatchLocationJson? ParseListArg(string? arg)
        {
            var list = arg?.Replace(ProgramArgs.List, "")?.Replace("=", "");
            if (list == null)
            {
                Console.Error.WriteLine($"cannot use the {ProgramArgs.List} arg with any other arguments");
                return null;
            }
            try
            {
                var json = JsonSerializer.Deserialize<WatchLocationJson>(list);
                if (json == null)
                {
                    Console.Error.WriteLine($"{ProgramArgs.List} json is invalid");
                    return null;
                }
                else if (json.Locations.Count == 0)
                {
                    Console.Error.WriteLine($"{ProgramArgs.List} locations cannot be empty");
                    return null;
                }
                return json;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{ProgramArgs.List} invalid json");
                Console.Error.WriteLine(ex);
                return null;
            }
        }

        public static bool ConsoleErrorIf(bool test, string message)
        {
            if (test)
            {
                Console.Error.WriteLine(message);
            }
            return test;
        }
    }
}