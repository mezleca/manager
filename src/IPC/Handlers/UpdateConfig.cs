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
            Id = data.Id,
            Secret = data.Secret,
            Token = data.Secret,
            StablePath = data.StablePath,
            LazerPath = data.LazerPath,
            StableSongsPath = data.StableSongsPath,
            AccessToken = data.AccessToken,
            Lazer = data.Lazer,
            Local = data.Local,
        });

        await Task.CompletedTask;
    }    
}

[MessagePackObject]
public class ConfigUpdateRequest
{
    [Key("osu_id")]
    public string? Id { get; set; }

    [Key("osu_secret")]
    public string? Secret { get; set; }

    [Key("token")]
    public string? Token { get; set; }

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

    [Key("local")]
    public bool? Local { get; set; }
    
}