using MessagePack;
using Main; 

namespace IPC.Handlers;

public class ConfigHandler
{
    static public async Task HandleConfigUpdate(int id, bool send, Request request)
    {
        ConfigData data = new()
        {
            StablePath = request.StablePath,
            StableSongsPath = request.StableSongsPath,
            AccessToken = request.AccessToken
        };

        Config.UpdateFrom(data);

        if (!send) {
            return;
        }
        
        await MessageHandler.Send(id, "update_config", new Response
        {
            Success = true
        });
    }

    [MessagePackObject]
    public class Request
    {
        [Key("stable_path")]
        public string? StablePath { get; set; }
        [Key("table_songs_path")]
        public string? StableSongsPath { get; set; }
        [Key("access_token")]
        public string? AccessToken { get; set; } 
    }

    [MessagePackObject]
    public class Response
    {
        [Key("success")]
        public bool? Success { get; set; }
    }
}
