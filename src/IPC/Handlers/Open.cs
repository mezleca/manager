using IPC;
using MessagePack;

namespace Main.Manager;

public class OpenHandler : IMessageHandler<OpenRequest>
{
    public string MessageType => "open";

    public async Task Handle(int id, bool send, OpenRequest data)
    {
        // scary
        if (Utils.IsValidURL(data.Url)) {
            Utils.Exec($"start {data.Url}");
        }
   
        await Task.CompletedTask;
    }
}

[MessagePackObject]
public class OpenRequest {
    [Key("url")]
    public string? Url {get; set; }
}