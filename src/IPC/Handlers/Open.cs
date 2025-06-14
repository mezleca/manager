using IPC;
using MessagePack;

namespace Main.Manager;

public class OpenHandler : MessageHandler<OpenRequest>
{
    public override string MessageType => "open";

    protected override void ProcessMessage(OpenRequest data)
    {
        // scary
        if (!string.IsNullOrEmpty(data.Url) && Utils.IsValidURL(data.Url)) {
            Utils.Exec($"{(Window.is_linux ? "xdg-open" : "start")} {data.Url}");
        }
    }
}

[MessagePackObject]
public class OpenRequest {
    [Key("url")]
    public string? Url {get; set; }
}