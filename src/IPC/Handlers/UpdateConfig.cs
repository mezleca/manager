using MessagePack;
using Main;
using Main.Manager;

namespace IPC.Handlers;

public class UpdateConfigHandler : IMessageHandler<ConfigUpdateRequest>
{
    public string MessageType => "update_config";

    public async Task Handle(int id, bool send, ConfigUpdateRequest data)
    {
        Manager.config.UpdateFrom(new Config {
            StablePath = data.StablePath,
            LazerPath = data.LazerPath,
            StableSongsPath = data.StableSongsPath,
            AccessToken = data.AccessToken,
            Lazer = data.Lazer
        });

        await Task.CompletedTask;
    }    
}

[MessagePackObject]
public class ConfigUpdateRequest
{
    [Key("stable_path")]
    public string? StablePath { get; set; }
    [Key("lazer_path")]
    public string? LazerPath { get; set; }
    [Key("stable_songs_path")]
    public string? StableSongsPath { get; set; }
    [Key("access_token")]
    public string? AccessToken { get; set; }
    [Key("lazer")]
    public bool? Lazer { get; set; }
}