using System.Text.Json;
using auto_mover;

const string version = "0.0.0";

if (args.Length > 0 && args[0] == "--version")
{
    Console.WriteLine(version);
    return 0;
}

var argsList = args.ToList();
var watchLocations = new List<WatchLocation>();

var hasListArg = argsList.Any(arg => arg.StartsWith(ProgramArgs.List));

if (Utils.ConsoleErrorIf(argsList.Count == 0, "must pass watch args") ||
    Utils.ConsoleErrorIf(argsList.Count > 3, "accepts a maximum of 3 arguments") ||
    Utils.ConsoleErrorIf(hasListArg && argsList.Count > 1, $"cannot use the {ProgramArgs.List} arg with any other arguments"))
{
    return 1;
}
else if (hasListArg)
{
    var json = Utils.ParseListArg(argsList[0]);
    if (json == null)
    {
        return 1;
    }

    watchLocations = json.Locations;
}
else if (argsList.Any(arg => !ProgramArgs.All.Any(sys => arg.StartsWith(sys))))
{
    Console.Error.WriteLine("when not using list arg must pass only watch, move and filter args");
    return 1;
}
else
{
    var watchLocation = Utils.ParseLocationFromArgs(
            argsList.Find(arg => arg.StartsWith(ProgramArgs.WatchDir))
                ?.Replace(ProgramArgs.WatchDir, "")
                ?.Replace("=", ""),
            argsList.Find(arg => arg.StartsWith(ProgramArgs.MoveDir))
                ?.Replace(ProgramArgs.MoveDir, "")
                ?.Replace("=", ""),
            argsList.Find(arg => arg.StartsWith(ProgramArgs.Filter))
                ?.Replace(ProgramArgs.Filter, "")
                ?.Replace("=", ""));

    if (watchLocation == null)
    {
        return 1;
    }

    watchLocations.Add(watchLocation);
}

Console.WriteLine(JsonSerializer.Serialize(watchLocations));
using var watcher = new Watcher(watchLocations);
watcher.Start();

Console.WriteLine("Started...");
Console.ReadLine();
return 0;
