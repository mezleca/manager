using Realms;
namespace Main;

public class RealmDB
{
    private static readonly ulong LAZER_SCHEMA_VERSION = 48;
    private static Realm? _instance = null;

    public static Realm GetInstance(string path)
    {
        if (_instance != null) {
            return _instance;
        }

        RealmConfiguration config = new(path)
        {
            IsReadOnly = false,
            SchemaVersion = LAZER_SCHEMA_VERSION
        };

        _instance = Realm.GetInstance(config);
        return _instance;
    }

    public static void DeleteInstance() {
        _instance = null;
    } 
}