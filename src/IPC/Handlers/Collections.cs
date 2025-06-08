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

public class GetCollectionsHandler : BaseMessageHandler<EmptyOBJ, CollectionsResponse>
{
    public override string MessageType => "get_collections";
    public override async Task Handle(int id, bool send, EmptyOBJ data) => await Task.CompletedTask;

    protected override Task<CollectionsResponse?> ProcessRequest(EmptyOBJ request)
    {
        CollectionsResponse result = new() {
            Success = false,
            Collections = []
        };   

        if (Manager.config.Lazer == true) 
        {
            var collections = Manager.GetAllLazerCollections();

            if (collections == null) {
                return Task.FromResult<CollectionsResponse?>(result);
            }

            foreach (var collection in collections) {

                if (string.IsNullOrEmpty(collection.Name)) {
                    continue;
                }

                result.Collections.Add(GetCollectionHandler.ProcessLazerCollection(collection, collection.Name));
            }

            result.Success = true;
        }
        else 
        {
            var collections = Manager.GetAllStableCollections();

            if (collections == null) {
                return Task.FromResult<CollectionsResponse?>(result);
            }

            foreach (var collection in collections) {

                if (string.IsNullOrEmpty(collection.Name)) {
                    continue;
                }

                result.Collections.Add(GetCollectionHandler.ProcessStableCollection(collection, collection.Name));
            }

            result.Success = true;
        }

        return Task.FromResult<CollectionsResponse?>(result);
    }
}

public class ReloadCollectionHandler : BaseMessageHandler<EmptyOBJ, LoadCollectionResponse>
{
    public override string MessageType => "load_collections";
    public override async Task Handle(int id, bool send, EmptyOBJ data) => await Task.CompletedTask;

    protected override async Task<LoadCollectionResponse?> ProcessRequest(EmptyOBJ request)
    {
        var result = new LoadCollectionResponse{ Success = false };

        try {
            result.Success = await Manager.LoadCollection();
        } catch(Exception ex) {
            Console.WriteLine(ex);
        }

        return result;
    }
}

public class SetCollectionHandler : BaseMessageHandler<SetCollectionRequest, SetCollectionResponse>
{
    public override string MessageType => "set_collection";
    public override async Task Handle(int id, bool send, SetCollectionRequest data) => await Task.CompletedTask;

    protected override Task<SetCollectionResponse?> ProcessRequest(SetCollectionRequest request)
    {
        var result = new SetCollectionResponse{ Success = false };
        Console.WriteLine(request.Name);

        try 
        {
            var beatmaps = Manager.config.Lazer == true ?
                Manager.GetLazerCollection(request.Name)?.BeatmapMD5Hashes :
                Manager.GetStableCollection(request.Name)?.Hashes;

            if (beatmaps == null) {
                return Task.FromResult<SetCollectionResponse?>(result);
            }
            
            result.Success = Manager.SetFilteredBeatmaps(request.Name, [..beatmaps]);
        } 
        catch(Exception) {}
        return Task.FromResult<SetCollectionResponse?>(result);
    }
}

[MessagePackObject]
public class EmptyOBJ {}

[MessagePackObject]
public class SetCollectionRequest {
    [Key("name")]
    public required string Name { get; set; }
}

[MessagePackObject]
public class SetCollectionResponse {
    [Key("success")]
    public required bool Success { get; set; }
}

[MessagePackObject]
public class LoadCollectionResponse {
    [Key("success")]
    public bool Success { get; set; }
}

[MessagePackObject]
public class CollectionsResponse {
    [Key("success")]
    public required bool Success { get; set; }
    [Key("collections")]
    public List<CollectionResponse>? Collections { get; set; }
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