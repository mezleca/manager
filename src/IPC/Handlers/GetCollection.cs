using Main.Manager;
using MessagePack;

namespace IPC.Handlers;

public class GetCollectionHandler : BaseMessageHandler<CollectionRequest, CollectionRespose>
{
    public override string MessageType => "get_collection";
    public override async Task Handle(int id, bool send, CollectionRequest data) => await Task.CompletedTask;

    protected override Task<CollectionRespose?> ProcessRequest(CollectionRequest request)
    {
        var collection = Manager.GetCollection(request.Name);

        if (collection == null) {
            return Task.FromResult<CollectionRespose?>(new CollectionRespose { Found = false });
        }

        return Task.FromResult<CollectionRespose?>(new CollectionRespose { Found = true, Name = collection.Name, Hashes = collection.Hashes, Size = collection.Size });
    }
}

[MessagePackObject]
public class CollectionRequest
{
    [Key("name")]
    public required string Name { get; set; }
}

[MessagePackObject]
public class CollectionRespose
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