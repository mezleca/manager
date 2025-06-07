using Main.Manager;
using MessagePack;

namespace IPC.Handlers;

public class ReloadCollectionHandler : BaseMessageHandler<LoadCollectionRequest, LoadCollectionResponse>
{
    public override string MessageType => "load_collections";
    public override async Task Handle(int id, bool send, LoadCollectionRequest data) => await Task.CompletedTask;

    protected override async Task<LoadCollectionResponse?> ProcessRequest(LoadCollectionRequest request)
    {
        var response = new LoadCollectionResponse{ Success = false };

        try {
            response.Success = await Manager.LoadCollection();
        } catch(Exception ex) {
            Console.WriteLine(ex);
        }

        return response;
    }
}

[MessagePackObject]
public class LoadCollectionRequest {}

[MessagePackObject]
public class LoadCollectionResponse {
    [Key("success")]
    public bool Success {get; set;}
}