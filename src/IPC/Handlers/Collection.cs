using Main.Manager;
using MessagePack;

namespace IPC.Handlers;

public class CollectionHandler
{
    static public async Task HandleGet(int id, bool send, CLRequest request)
    {
        var collection = Manager.GetStableCollection(request.Name);

        if (collection == null)
        {
            await MessageHandler.Send(id, "get_collection", new CLResponse
            {
                Found = false
            }); 
        }

        await MessageHandler.Send(id, "get_collection", new CLResponse
        {
            Found = true,
            Name = collection?.Name,
            Size = collection?.Size,
            Hashes = collection?.Hashes
        });
    }

    static public async Task HandleInitialize(int id, bool send, InitRequest request)
    {
        var success = Manager.InitializeStableCollection();

        if (!send) {
            return;
        }

        await MessageHandler.Send(id, "initialize_collections", new InitResponse
        {
            Success = success
        });
    }

    [MessagePackObject]
    public class CLRequest
    {
        [Key("name")]
        public required string Name { get; set; }
    }

    [MessagePackObject]
    public class InitRequest
    {
        
    }

    [MessagePackObject]
    public class InitResponse
    {
        [Key("success")]
        public required bool Success { get; set; }
    }

    [MessagePackObject]
    public class CLResponse
    {
        [Key("found")]
        public required bool Found { get; set; }
        [Key("name")]
        public string? Name { get; set; }
        [Key("size")]
        public uint? Size { get; set; }
        [Key("hashes")]
        public List<string>? Hashes { get; set; }
    }
}
