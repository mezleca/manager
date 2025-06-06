using MessagePack;
using Main;

namespace IPC.Handlers;

public class ShowDialogHandler : BaseMessageHandler<DialogRequest, DialogResponse>
{
    public override string MessageType => "show_dialog";
    public override async Task Handle(int id, bool send, DialogRequest data) => await Task.CompletedTask;

    protected override async Task<DialogResponse?> ProcessRequest(DialogRequest request)
    {
        string[]? result = request.Type == "file" 
            ? await Window.window?.ShowOpenFileAsync(defaultPath: request.Default, multiSelect: request.Multi)
            : await Window.window?.ShowOpenFolderAsync(defaultPath: request.Default, multiSelect: false);

        // @TODO: if its file type also return file buffer
        // @TODO: multiple files support
        return result?.Length > 0 ? new DialogResponse { Path = result[0] } : null;
    }
}

[MessagePackObject]
public class DialogRequest 
{
    [Key("type")]
    public required string Type { get; set; }
    
    [Key("multi")]
    public bool Multi { get; set; }
    
    [Key("default")]
    public string? Default { get; set; }
}

[MessagePackObject]
public class DialogResponse 
{
    [Key("path")]
    public required string Path { get; set; }
    
    [Key("data")]
    public byte[]? Data { get; set; }
}