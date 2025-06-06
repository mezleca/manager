using IPC;
using MessagePack;

namespace Main.Manager;

public class OpenHandler : IMessageHandler<OpenRequest>
{
    public string MessageType => "open";

    public async Task Handle(int id, bool send, OpenRequest data)
    {
        // scary
        if (!String.IsNullOrEmpty(data.Url) && Utils.IsValidURL(data.Url)) {
            Utils.Exec($"{(Window.is_linux ? "xdg-open" : "start")} {data.Url}");
        }
   
        await Task.CompletedTask;
    }
}

[MessagePackObject]
public class OpenRequest {
    [Key("url")]
    public string? Url {get; set; }
}