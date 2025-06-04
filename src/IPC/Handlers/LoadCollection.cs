using Main.Manager;
using MessagePack;

namespace IPC.Handlers;

public class ReloadCollectionHandler : IMessageHandler<LoadCollectionResponse>
{
    public string MessageType => "load_collections";

    public async Task Handle(int id, bool send, LoadCollectionResponse data)
    {
        await Manager.LoadStableCollection();
    }
}

[MessagePackObject]
public class LoadCollectionResponse {}