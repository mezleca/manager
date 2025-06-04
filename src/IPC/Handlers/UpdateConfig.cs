using MessagePack;
using Main;

namespace IPC.Handlers;

public class UpdateConfigHandler : IMessageHandler<ConfigUpdateRequest>
{
    public string MessageType => "update_config";

    public async Task Handle(int id, bool send, ConfigUpdateRequest data)
    {
        ConfigData updated_config = new()
        {
            StablePath = data.StablePath,
            StableSongsPath = data.StableSongsPath,
            AccessToken = data.AccessToken
        };

        Config.UpdateFrom(updated_config);

        await Task.CompletedTask;
    }    
}

[MessagePackObject]
public class ConfigUpdateRequest
{
    [Key("stable_path")]
    public string? StablePath { get; set; }
    [Key("table_songs_path")]
    public string? StableSongsPath { get; set; }
    [Key("access_token")]
    public string? AccessToken { get; set; } 
}