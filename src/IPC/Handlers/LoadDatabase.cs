using MessagePack;
using IPC;

namespace Main.Manager;

public class LoadOsuDataHandler : BaseMessageHandler<OsuDataRequest, OsuDataResponse>
{
    public override string MessageType => "load_database";
    public override async Task Handle(int id, bool send, OsuDataRequest data) => await Task.CompletedTask;

    protected override async Task<OsuDataResponse?> ProcessRequest(OsuDataRequest request)
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
public class OsuDataRequest {}

[MessagePackObject]
public class OsuDataResponse {
    [Key("success")]
    public bool Success {get; set;}
}