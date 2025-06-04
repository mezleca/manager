namespace Main.Parser;

public unsafe class Reader
{
    public static byte[]? ReadFile(string path)
    {
        bool exists = File.Exists(path);

        if (!exists)
        {
            return null;
        }

        return File.ReadAllBytes(path);
    }

    public static StableCollectionFile ReadCollection(byte[] buffer)
    {
        var collections = new StableCollectionFile();
        var offset = 0;

        fixed (byte* ptr = &buffer[0])
        {
            collections.Version = Binary.Read<uint>(ptr, &offset);
            collections.Size = Binary.Read<uint>(ptr, &offset);
            collections.Collections = [];

            for (int i = 0; i < collections.Size; i++)
            {
                var collection = new StableCollection
                {
                    Hashes = [],
                    Name = Binary.ReadString(ptr, &offset),
                    Size = Binary.Read<uint>(ptr, &offset)
                };

                for (int j = 0; j < collection.Size; j++)
                {
                    var hash = Binary.ReadString(ptr, &offset);
                    collection.Hashes.Add(hash);
                }

                collections.Collections.Add(collection.Name, collection);
            }
        }

        return collections;
    }
}