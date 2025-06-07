using Realms;
namespace Main;

public class RealmDB
{
    private static readonly ulong LAZER_SCHEMA_VERSION = 48;
    public static Realm? GetInstance(string? path)
    {
        if (string.IsNullOrEmpty(path)) {
            return null;
        }

        RealmConfiguration config = new($"{path}/client.realm")
        {
            IsReadOnly = false,
            SchemaVersion = LAZER_SCHEMA_VERSION
        };

        return Realm.GetInstance(config);
    }
}