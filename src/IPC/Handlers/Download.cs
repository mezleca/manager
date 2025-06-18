using App;
using IPC;
using IPC.Handlers;
using MessagePack;

namespace Ipc.Handlers;

public class AddDownloadHandler : RequestResponseHandler<NewDownloadRequest, DownloadResponse>
{
    public override string MessageType => "add_download";

    protected override DownloadResponse? ProcessRequest(NewDownloadRequest request)
    {
        var result = new DownloadResponse{ Success = false };

        // only allow 5 downloads on queue (no reason to do more than 5)
        if (Downloader.Size() >= 5) {
            return result;
        }

        List<DownloadBeatmap> beatmaps = [];

        for (int i = 0; i < request.Beatmaps.Count; i++) {
            var beatmap = request.Beatmaps[i];
            beatmaps.Add(new DownloadBeatmap{ Id = beatmap.Id ?? null, Md5 = beatmap.Md5 });
        }

        var new_download = new Download{
            Id = request.Id,
            Name = request.Name,
            Beatmaps = beatmaps
        };

        Downloader.AddDownload(new_download);
        result.Success = true;

        // start download if is the first one
        if (Downloader.Size() == 1) {
            _ = Downloader.Initialize();
        }

        return result;
    }
}

// @TODO:
public class DownloadHandler : RequestResponseHandler<DownloadRequest, DownloadResponse>
{
    public override string MessageType => "download";

    protected override DownloadResponse? ProcessRequest(DownloadRequest request)
    {
        throw new NotImplementedException();
    }
}

// @TODO:
public class UpdateDownloadHandler : RequestResponseHandler<UpdateDownloadRequest, DownloadResponse>
{
    public override string MessageType => "update_download";

    protected override DownloadResponse? ProcessRequest(UpdateDownloadRequest request)
    {
        // ensure we have something on the queue
        if (Downloader.Size() == 0) {
            return null;
        }

        var result = new DownloadResponse{ Success = false };

        switch (request.Type) {
            case "pause": {
                Downloader.StopDownload();
                result.Success = true;
                break;
            }
            case "resume": {
                Downloader.ResumeDownload();
                break;
            }
            case "remove": {
                Downloader.RemoveDownload(request.Name);
                result.Success = true;
                break;
            }
        }

        return result;
    }
}

public class GetDownloadsHandler : RequestResponseHandler<EmptyOBJ, DownloadResponse>
{
    public override string MessageType => "get_downloads";

    protected override DownloadResponse? ProcessRequest(EmptyOBJ request)
    {
        var result = new DownloadResponse{ Success = true, List = [] };
        var downloads = Downloader.Downloads();

        for (int i = 0; i < downloads.Count; i++) {
            var download = downloads[i];

            result.List.Add(new DownloadListResponse{
                Id = download.Id,
                Name = download.Name
            });
        }

        return result;
    }
}

public class AddTokenHandler : MessageHandler<TokenRequest>
{
    public override string MessageType => "add_token";
    protected override void ProcessMessage(TokenRequest data) => Downloader.SetToken(data.Token);
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

public class RemoveMirrorHandler : MessageHandler<MirrorRequest>
{
    public override string MessageType => "remove_mirror";
    protected override void ProcessMessage(MirrorRequest request) => Downloader.RemoveMirror(request.Name);
}

// ts pmo

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
public class DownloadListResponse {
    [Key("id")]
    public required string Id { get; set; }
    [Key("name")]
    public required string Name { get; set; }
}

[MessagePackObject]
public class NewDownloadBeatmap {
    [Key("md5")]
    public required string Md5 { get; set; }
    [Key("id")]
    public int? Id { get; set; }
}

[MessagePackObject]
public class NewDownloadRequest {
    [Key("id")]
    public required string Id { get; set; }
    [Key("name")]
    public required string Name { get; set; }
    [Key("beatmaps")]
    public required List<NewDownloadBeatmap> Beatmaps { get; set; }
}

[MessagePackObject]
public class UpdateDownloadRequest {
    [Key("type")]
    public required string Type { get; set; }
    [Key("name")]
    public required string Name { get; set; }
}

[MessagePackObject]
public class TokenRequest {
    [Key("token")]
    public required string Token { get; set; }
}

[MessagePackObject]
public class DownloadRequest {
    [Key("md5")]
    public required string Md5 { get; set; }
    [Key("id")]
    public int? Id { get; set; }
}

[MessagePackObject]
public class DownloadResponse {
    [Key("success")]
    public required bool Success { get; set; }
    [Key("list")]
    public List<DownloadListResponse>? List { get; set; }
}