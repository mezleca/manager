namespace Main.Manager;

public class Manager
{
    public static StableCollectionFile? stable_collection;
    public static bool loaded = false;

    public static Task<bool> LoadStableCollection()
    {
        if (string.IsNullOrEmpty(Config.StablePath)) {
            Console.WriteLine("path is null");
            return Task.FromResult(false);
        }

        byte[] data = File.ReadAllBytes($"{Config.StablePath}/collection.db");

        if (data.Length == 0) {
            Console.WriteLine("invalid buffer");
            return Task.FromResult(false);
        }

        stable_collection = Parser.Reader.ReadCollection(data);
        loaded = true;
        return Task.FromResult(true);
    }

    public static StableCollection? GetStableCollection(string name)
    {
        var stable_path = Config.StablePath;
 
        if (string.IsNullOrEmpty(stable_path)) {
            return null;
        }

        if (stable_collection.Collections.Count == 0) {
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