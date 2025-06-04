using Realms;
namespace Main;

public static class RealmDB
{
    private static readonly ulong LAZER_SCHEMA_VERSION = 48;
    private static Realm? _instance = null;

    public static Realm GetInstance()
    {
        if (_instance != null)
        {
            return _instance;
        }

        // hardcode Lazer path for now
        RealmConfiguration config = new("C:\\Users\\rel\\AppData\\Roaming\\osu\\client.realm")
        {
            IsReadOnly = false,
            SchemaVersion = LAZER_SCHEMA_VERSION
        };

        _instance = Realm.GetInstance(config);

        var beatmaps = _instance.All<Beatmap>().ToArray();
        Console.WriteLine(beatmaps[0].Metadata.Title);

        return _instance;
    }
}