using System.Collections.ObjectModel;
using IPC;
using IPC.Handlers;
using MessagePack;

namespace Main.Manager;

public class GetCollectionsHandler : BaseMessageHandler<CollectionsRequest, CollectionsResponse>
{
    public override string MessageType => "get_collections";
    public override async Task Handle(int id, bool send, CollectionsRequest data) => await Task.CompletedTask;

    protected override Task<CollectionsResponse?> ProcessRequest(CollectionsRequest request)
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
                result.Collections.Add(GetCollectionHandler.ProcessStableCollection(collection, collection.Name));
            }

            result.Success = true;
        }

        return Task.FromResult<CollectionsResponse?>(result);
    }
}

[MessagePackObject]
public class CollectionsRequest { }

[MessagePackObject]
public class CollectionsResponse {
    [Key("success")]
    public required bool Success { get; set; }
    [Key("collections")]
    public List<CollectionResponse>? Collections { get; set; }
}
