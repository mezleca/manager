using MessagePack;
using IPC;

namespace Main.Manager;

public class LoadOsuDataHandler : AsyncRequestResponseHandler<OsuDataRequest, OsuDataResponse>
{
    public override string MessageType => "load_database";

    protected override async Task<OsuDataResponse?> ProcessRequestAsync(OsuDataRequest request)
    {
        var response = new OsuDataResponse{ Success = false };

        try {
            response.Success = await Manager.LoadDatabase();
        } catch(Exception ex) {
            Console.WriteLine(ex);
        }

        return response;
    }
}

[MessagePackObject]
public class OsuDataRequest { }

[MessagePackObject]
public class OsuDataResponse {
    [Key("success")]
    public bool Success {get; set;}
}