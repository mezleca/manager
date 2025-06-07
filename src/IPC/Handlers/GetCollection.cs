using Main.Manager;
using MessagePack;

namespace IPC.Handlers;

public class GetCollectionHandler : BaseMessageHandler<CollectionRequest, CollectionRespose>
{
    public override string MessageType => "get_collection";
    public override async Task Handle(int id, bool send, CollectionRequest data) => await Task.CompletedTask;

    protected override Task<CollectionRespose?> ProcessRequest(CollectionRequest request)
    {
        var result = new CollectionRespose { Found = false, Name = request.Name };

        if (Manager.config.Lazer == true)
        {
            var collection = Manager.GetLazerCollection(request.Name);
            if (collection != null) {      
                result.Found = true;
                result.Hashes = collection.BeatmapMD5Hashes?.ToList();
            }
        }
        else
        {
            var collection = Manager.GetStableCollection(request.Name);
            if (collection != null) {       
                result.Found = true;
                result.Hashes = collection.Hashes;
            }
        }

        return Task.FromResult<CollectionRespose?>(result);
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