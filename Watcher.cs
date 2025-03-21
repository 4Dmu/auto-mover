using System.Text.Json.Serialization;

namespace auto_mover
{

    public class WatchLocation(string watchDir, string moveDir, string? filter)
    {
        [JsonPropertyName("watchDir")]
        [JsonRequired]
        public string WatchDir { get; set; } = watchDir;

        [JsonPropertyName("moveDir")]
        [JsonRequired]
        public string MoveDir { get; set; } = moveDir;

        [JsonPropertyName("filter")]
        public string? Filter { get; set; } = filter;
    }

    public class WatchLocationJson
    {
        [JsonPropertyName("locations")]
        public List<WatchLocation> Locations { get; set; } = [];
    }

    public class Watcher : IDisposable
    {
        private readonly List<FileSystemWatcher> watchers = [];

        public Watcher(List<WatchLocation> locations)
        {
            foreach (var location in locations)
            {
                var watcher = new FileSystemWatcher
                {
                    Path = location.WatchDir,
                };

                if (location.Filter != null)
                {
                    watcher.Filter = location.Filter;
                }

                watcher.Changed += new FileSystemEventHandler((object sender, FileSystemEventArgs args) =>
                {
                    var fileExists = File.Exists(args.FullPath);
                    if (fileExists && args.Name != null)
                    {

                        var newPath = $"{location.MoveDir}/{args.Name}";
                        Console.WriteLine($"moving ${args.FullPath} to ${newPath}");
                        File.Move(args.FullPath, newPath);
                    }
                });

                watchers.Add(watcher);
            }
        }

        public void Start()
        {
            watchers.ForEach(watcher => watcher.EnableRaisingEvents = true);
        }

        public void Dispose()
        {
            watchers.ForEach(watcher => watcher.Dispose());
            GC.SuppressFinalize(this);
        }
    }
}