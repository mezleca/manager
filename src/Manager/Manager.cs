using Realms;

namespace Main.Manager;

public class Manager
{
    public static StableCollectionFile? StableCollection = new();
    public static StableDatabase? StableDatabase = new();
    public static Config config = new();

    public static Dictionary<string, List<string>> FilteredBeatmaps = new();

    public static bool IsStablePathValid() {
        return !string.IsNullOrEmpty(config.StablePath) && Directory.Exists(config.StablePath);
    }
    
    public static bool IsLazerPathValid() {
        return !string.IsNullOrEmpty(config.LazerPath) && File.Exists($"{config.LazerPath}/client.realm");
    }

    public static byte[]? GetFileBuffer(string filePath)
    {
        if (!File.Exists(filePath)) {   
            return null;
        }
        return File.ReadAllBytes(filePath);
    }

    public static Task<bool> LoadLazerCollection()  {
        // stupid
        return Task.FromResult(IsLazerPathValid());
    }

    public static Task<bool> LoadStableCollection() {
        if (!IsStablePathValid()) {    
            Console.WriteLine("stable path is not valid");
            return Task.FromResult(false);
        }

        byte[]? data = GetFileBuffer($"{config.StablePath}/collection.db");

        if (data == null) {
            Console.WriteLine("invalid stable collection buffer");
            return Task.FromResult(false);
        }

        if (StableCollection != null) {
            StableCollection = null;
        }

        StableCollection = Parser.Reader.ReadCollection(data);
        return Task.FromResult(true);
    }

    public static Task<bool> LoadStableDB() 
    {
        if (!IsStablePathValid()) {    
            Console.WriteLine("invalid path");
            return Task.FromResult(false);
        }

        byte[]? data = GetFileBuffer($"{config.StablePath}/osu!.db");

        if (data == null) {
            Console.WriteLine("invalid buffer");
            return Task.FromResult(false);
        }

        if (StableDatabase != null) {
            StableDatabase = null;
        }

        StableDatabase = Parser.Reader.ReadStableDB(data);
        return Task.FromResult(true);
    }

    public static Task<bool> LoadLazerDB() 
    {
        if (!IsLazerPathValid() || RealmDB.GetInstance(config.LazerPath) == null) {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public static StableCollection? GetStableCollection(string name)
    {
        if (StableCollection?.Collections == null) {      
            Console.WriteLine("collection is not initialized yet");
            return null;
        }

        if (!StableCollection.Collections.TryGetValue(name, out StableCollection collection)) { 
            return null;
        }

        return collection;
    }

    public static LazerCollection? GetLazerCollection(string name)
    {
        var instance = RealmDB.GetInstance(config.LazerPath);

        if (instance == null) {
            return null;
        }

        return instance.All<LazerCollection>().FirstOrDefault((c) => c.Name == name);
    }

    public static LazerBeatmap? GetLazerBeatmap(string md5) 
    {
        if (config.Lazer == false) {
            return null;
        }

        Realm? instance = RealmDB.GetInstance(config.LazerPath);

        if (instance == null) {
            return null;
        }

        return instance.All<LazerBeatmap>().FirstOrDefault((b) => b.MD5Hash == md5);
    }

    public static StableBeatmap? GetStableBeatmap(string md5) 
    {
        if (StableDatabase?.Beatmaps == null) {
            return null;
        }

        if (!StableDatabase.Beatmaps.TryGetValue(md5, out StableBeatmap beatmap)) { 
            return null;
        }

        return beatmap;
    }

    public static IEnumerable<StableCollection>? GetAllStableCollections()
    {
        if (StableCollection?.Collections == null) {
            return null;
        }

        return StableCollection.Collections.Values;
    }

    public static IEnumerable<LazerCollection>? GetAllLazerCollections()
    {
        var instance = RealmDB.GetInstance(config.LazerPath);
        if (instance == null) {
            return null;
        }

        return instance.All<LazerCollection>()?.ToList();
    }

    public static bool SetFilteredBeatmaps(string name, List<string> hashes) {
        FilteredBeatmaps = [];
        FilteredBeatmaps[name] = hashes;
        return true;
    }

    public static List<string>? GetFilteredBeatmaps(string name) {
        return FilteredBeatmaps.TryGetValue(name, out var hashes) ? hashes : null;
    }

    private static StableBeatmap? ConvertLazerToStable(LazerBeatmap lazerBeatmap)
    {
        if (lazerBeatmap == null) {
            return null;
        }

        return new StableBeatmap
        {
            Artist = lazerBeatmap.Metadata?.Artist,
            ArtistUnicode = lazerBeatmap.Metadata?.ArtistUnicode,
            Title = lazerBeatmap.Metadata?.Title,
            TitleUnicode = lazerBeatmap.Metadata?.TitleUnicode,
            Mapper = lazerBeatmap.Metadata?.Author?.Username,
            Difficulty = lazerBeatmap.DifficultyName,
            AudioFileName = lazerBeatmap.Metadata?.AudioFile,
            Md5 = lazerBeatmap.MD5Hash,
            Ar = lazerBeatmap.Difficulty?.ApproachRate ?? 0f,
            Cs = lazerBeatmap.Difficulty?.CircleSize ?? 0f,
            Hp = lazerBeatmap.Difficulty?.DrainRate ?? 0f,
            Od = lazerBeatmap.Difficulty?.OverallDifficulty ?? 0f,
            SliderVelocity = lazerBeatmap.Difficulty?.SliderMultiplier ?? 0.0,
            Length = (uint)lazerBeatmap.Length,
            AudioPreview = (uint)(lazerBeatmap.Metadata?.PreviewTime ?? 0),
            DifficultyId = (uint)lazerBeatmap.OnlineID,
            BeatmapsetId = (uint)lazerBeatmap.BeatmapSet?.OnlineID,
            Mode = (byte)lazerBeatmap.Ruleset?.OnlineID,
            Source = lazerBeatmap.Metadata?.Source,
            Tags = lazerBeatmap.Metadata?.Tags,
            LastPlayed = (ulong)(lazerBeatmap.LastPlayed?.ToUnixTimeSeconds() ?? 0),
            Status = (byte)lazerBeatmap.Status
        };
    }

    public static void FilterBeatmaps() {
        throw new NotImplementedException();
    }

    public static async Task<bool> LoadCollection() {
        return config.Lazer == true ? await LoadLazerCollection() : await LoadStableCollection();
    }

    public static async Task<bool> LoadDatabase() {
        return config.Lazer == true ? await LoadLazerDB() : await LoadStableDB();
    }

    public static bool UpdateCollection() {
        throw new NotImplementedException();
    }
}