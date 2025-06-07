using Realms;

namespace Main.Manager;

public class Manager
{
    public static StableCollectionFile? StableCollection = new();
    public static StableDatabase? StableDatabase = new();
    public static Config config = new();

    public static bool IsStablePathValid() {
        return !string.IsNullOrEmpty(config.StablePath) && Directory.Exists(config.StablePath);
    }
    
    public static bool IsLazerPathValid() {
        return !string.IsNullOrEmpty(config.LazerPath) && Directory.Exists(config.LazerPath);
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

    public static async Task<bool> LoadCollection() {
        return config.Lazer == true ? await LoadLazerCollection() : await LoadStableCollection();
    }

    public static async Task<bool> LoadDatabase() {
        return config.Lazer == true ? await LoadLazerDB() : await LoadStableDB();
    }

    public static bool UpdateCollection() {
        return true;
    }
}