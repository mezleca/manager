using Main.Manager;
using MessagePack;

namespace IPC.Handlers;

public class GetCollectionHandler : BaseMessageHandler<CollectionRequest, CollectionResponse>
{
    public override string MessageType => "get_collection";
    public override async Task Handle(int id, bool send, CollectionRequest data) => await Task.CompletedTask;

    protected override Task<CollectionResponse?> ProcessRequest(CollectionRequest request)
    {
        var result = new CollectionResponse { Found = false, Name = request.Name };

        if (Manager.config.Lazer == true)
        {
            var collection = Manager.GetLazerCollection(request.Name);
            if (collection != null) {
                result = ProcessLazerCollection(collection, request.Name);
            }
        }
        else
        {
            var collection = Manager.GetStableCollection(request.Name);
            if (collection != null){
                result = ProcessStableCollection(collection, request.Name);
            }
        }

        return Task.FromResult<CollectionResponse?>(result);
    }

    public static CollectionResponse ProcessLazerCollection(LazerCollection collection, string name)
    {
        return new CollectionResponse
        {
            Found = true,
            Name = name,
            Size = (uint?)collection.BeatmapMD5Hashes?.Count,
            Hashes = collection.BeatmapMD5Hashes?.ToList()
        };
    }

    public static CollectionResponse ProcessStableCollection(StableCollection collection, string name)
    {
        return new CollectionResponse
        {
            Found = true,
            Name = name,
            Size = (uint?)collection.Hashes?.Count,
            Hashes = collection.Hashes
        };
    }
}

[MessagePackObject]
public class CollectionRequest
{
    [Key("name")]
    public required string Name { get; set; }
}

[MessagePackObject]
public class CollectionResponse
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