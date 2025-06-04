namespace Main.Manager;

public class Manager
{
    public static StableCollectionFile? stable_collection;

    public static bool InitializeStableCollection()
    {
        if (string.IsNullOrEmpty(Config.StablePath)) {
            return false;
        }

        byte[] data = File.ReadAllBytes($"{Config.StablePath}/collection.db");

        if (data.Length == 0) {
            return false;
        }

        stable_collection = Parser.Reader.ReadCollection(data);
        return true;
    }

    public static StableCollection? GetStableCollection(string name)
    {
        var stable_path = Config.StablePath;

        if (string.IsNullOrEmpty(stable_path)) {
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