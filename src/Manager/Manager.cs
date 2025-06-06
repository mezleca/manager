namespace Main.Manager;

public class Manager
{
    public static StableCollectionFile? stable_collection = null;
    public static Config config = new();

    public static Task<bool> LoadCollection()
    {
        if (string.IsNullOrEmpty(config.StablePath)) {    
            return Task.FromResult(false);
        }

        byte[] data = File.ReadAllBytes($"{config.StablePath}/collection.db");

        if (data.Length == 0) {
            return Task.FromResult(false);
        }

        stable_collection = Parser.Reader.ReadCollection(data);
        return Task.FromResult(true);
    }

    public static StableCollection? GetCollection(string name)
    {
        var stable_path = config.StablePath;

        if (string.IsNullOrEmpty(stable_path)) {
            return null;
        }

        if (stable_collection?.Collections == null) {      
            return null;
        }

        if (!stable_collection.Collections.TryGetValue(name, out StableCollection collection)) { 
            return null;
        }

        return collection;
    }

    public static bool UpdateCollection()
    {
        return true;
    }
}