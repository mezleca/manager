using Main;
using Main.Manager;
using MessagePack;

namespace IPC.Handlers;

public class GetBeatmapHandler : BaseMessageHandler<BeatmapRequest, BeatmapResponse>
{
    public override string MessageType => "get_beatmap";
    public override async Task Handle(int id, bool send, BeatmapRequest data) => await Task.CompletedTask;

    protected override Task<BeatmapResponse> ProcessRequest(BeatmapRequest request)
    {
        BeatmapResponse result = new() {
            Found = false
        };

        if (Manager.config.Lazer == true) {

            var beatmap = Manager.GetLazerBeatmap(request.Md5);

            if (beatmap != null) {
                result.Found = true;
                result.Title = beatmap.Metadata.Title;
                result.Artist = beatmap.Metadata.Artist;
                result.Difficulty = beatmap.DifficultyName;
                result.BeatmapsetId = beatmap.BeatmapSet.OnlineID;
                result.DifficultyId = beatmap.OnlineID;
                result.Md5 = beatmap.MD5Hash;
                result.Bpm = 0; // TODO
                result.Sr = 0; // TODO
                result.Ar = beatmap.Difficulty.ApproachRate;
                result.Cs = beatmap.Difficulty.CircleSize;
                result.Hp = beatmap.Difficulty.DrainRate;
                result.Od = beatmap.Difficulty.OverallDifficulty;
                result.mapper = beatmap.Metadata.Author?.Username;
            }
        } 
        else 
        {
            var beatmap = Manager.GetStableBeatmap(request.Md5);

            if (beatmap != null) {
                result.Found = true;
                result.Title = beatmap.Title;
                result.Artist = beatmap.Artist;
                result.Difficulty = beatmap.Difficulty;
                result.BeatmapsetId = (int)beatmap.BeatmapsetId;
                result.DifficultyId = (int)beatmap.DifficultyId;
                result.Md5 = beatmap.Md5;
                result.Bpm = 0; // TODO
                result.Sr = 0; // TODO
                result.Ar = beatmap.Ar;
                result.Cs = beatmap.Cs;
                result.Hp = beatmap.Hp;
                result.Od = beatmap.Od;
                result.mapper = beatmap.Mapper;
            }
        }

        return Task.FromResult(result);
    }
}

[MessagePackObject]
public class BeatmapRequest
{
    [Key("md5")]
    public required string Md5 { get; set; }
}

[MessagePackObject]
public class BeatmapResponse
{
    [Key("found")]
    public required bool Found { get; set; }

    [Key("title")]
    public string? Title { get; set; }

    [Key("artist")]
    public string? Artist { get; set; }

    [Key("mapper")]
    public string? mapper { get; set; }

    [Key("difficulty")]
    public string? Difficulty { get; set; }

    [Key("beatmapset_id")]
    public int? BeatmapsetId { get; set; }

    [Key("difficulty_id")]
    public int? DifficultyId { get; set; }

    [Key("md5")]
    public string? Md5 { get; set; }

    [Key("bpm")]
    public float? Bpm { get; set; }

    [Key("sr")]
    public float? Sr { get; set; }

    [Key("ar")]
    public float? Ar { get; set; }

    [Key("cs")]
    public float? Cs { get; set; }

    [Key("hp")]
    public float? Hp { get; set; }

    [Key("od")]
    public float? Od { get; set; }
}