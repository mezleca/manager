using System;
using App;
using IPC;
using MessagePack;

namespace Ipc.Handlers;

public class UpdateDownloaderHandler : RequestResponseHandler<DownloadRequest, DownloadResponse>
{
    public override string MessageType => "update_downloader";

    protected override DownloadResponse? ProcessRequest(DownloadRequest request)
    {
        var result = new DownloadResponse{ Success = false };

        Downloader.SetToken(request.Token);
        
        foreach (var mirror in request.Mirrors) {

            if (string.IsNullOrEmpty(mirror.Url)) {
                return result;
            }
            
            Downloader.AddMirror(new Mirror{ Name = mirror.Name, Url = mirror.Url });
        }

        result.Success = true;
        return result;
    }
}

public class AddMirrorHandler : RequestResponseHandler<MirrorRequest, MirrorResponse>
{
    public override string MessageType => "add_mirror";

    protected override MirrorResponse? ProcessRequest(MirrorRequest request)
    {
        if (string.IsNullOrEmpty(request.Url)) {
            return new MirrorResponse{ Success = false };
        }

        Downloader.AddMirror(new Mirror{
            Name = request.Name,
            Url = request.Url
        });

        return new MirrorResponse{ Success = true };
    }
}

public class RemoveMirrorHandler : RequestResponseHandler<MirrorRequest, MirrorResponse>
{
    public override string MessageType => "remove_mirror";

    protected override MirrorResponse? ProcessRequest(MirrorRequest request)
    {
        Downloader.RemoveMirror(request.Name);
        return new MirrorResponse{ Success = true };
    }
}

[MessagePackObject]
public class MirrorRequest {
    [Key("name")]
    public required string Name { get; set; }
    [Key("url")]
    public string? Url { get; set; }
}

[MessagePackObject]
public class MirrorResponse {
    [Key("success")]
    public required bool Success { get; set; }
}

[MessagePackObject]
public class DownloadRequest {
    [Key("token")]
    public required string Token { get; set; }
    [Key("mirrors")]
    public required List<MirrorRequest> Mirrors { get; set; } 
}

[MessagePackObject]
public class DownloadResponse {
    [Key("success")]
    public required bool Success { get; set; }
}