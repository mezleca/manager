namespace Main.Manager;

public class Manager
{
    public static StableCollectionFile? StableCollection = new();
    public static StableDatabase? StableDatabase = new();
    public static Config config = new();

    public static bool IsStablePathPresent() {
        return !string.IsNullOrEmpty(config.StablePath);
    }

    public static byte[]? GetFileBuffer(string filePath)
    {
        if (!File.Exists(filePath)) {   
            return null;
        }
        return File.ReadAllBytes(filePath);
    }

    public static Task<bool> LoadCollection()
    {
        if (!IsStablePathPresent()) {    
            return Task.FromResult(false);
        }

        byte[]? data = GetFileBuffer($"{config.StablePath}/collection.db");

        if (data == null) {
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
        if (!IsStablePathPresent()) {    
            return Task.FromResult(false);
        }

        byte[]? data = GetFileBuffer($"{config.StablePath}/osu!.db");

        if (data == null) {
            return Task.FromResult(false);
        }

        if (StableDatabase != null) {
            StableDatabase = null;
        }

        StableDatabase = Parser.Reader.ReadStableDB(data);
        return Task.FromResult(true);
    }

    public static StableCollection? GetCollection(string name)
    {
        if (StableCollection?.Collections == null) {      
            return null;
        }

        if (!StableCollection.Collections.TryGetValue(name, out StableCollection collection)) { 
            return null;
        }

        return collection;
    }

    public static StableBeatmap? GetBeatmap(string md5) {

        if (StableDatabase?.Beatmaps == null) {
            return null;
        }

        if (!StableDatabase.Beatmaps.TryGetValue(md5, out StableBeatmap beatmap)) { 
            return null;
        }

        return beatmap;
    }

    public static bool UpdateCollection()
    {
        return true;
    }
}