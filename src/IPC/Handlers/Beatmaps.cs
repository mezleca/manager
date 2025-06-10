using System.Net.Cache;
using Main;
using Main.Manager;
using MessagePack;

namespace IPC.Handlers;

public class GetBeatmapHandler : BaseMessageHandler<BeatmapRequest, BeatmapResponse>
{
    public override string MessageType => "get_beatmap";
    public override async Task Handle(int id, bool send, BeatmapRequest data) => await Task.CompletedTask;

    protected override Task<BeatmapResponse?> ProcessRequest(BeatmapRequest request)
    {
        BeatmapResponse? result = Manager.config.Lazer == true ? ProcessLazerBeatmap(request.Md5) : ProcessStableBeatmap(request.Md5);
        return Task.FromResult<BeatmapResponse?>(result);
    }

    public static BeatmapResponse ProcessLazerBeatmap(string md5)
    {
        BeatmapResponse result = new() {
            Found = false
        };

        var beatmap = Manager.GetLazerBeatmap(md5);

        if (beatmap == null) {
            return result;
        }

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

        return result;
    }

    public static BeatmapResponse ProcessStableBeatmap(string md5)
    {
        BeatmapResponse result = new() {
            Found = false
        };

        var beatmap = Manager.GetStableBeatmap(md5);

        if (beatmap == null) {
            return result;
        }

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

        return result;
    }

    public static BeatmapResponse ProcessLazerBeatmap(LazerBeatmap beatmap)
    {
        if (beatmap == null) {
            return new BeatmapResponse { Found = false };
        }

        return new BeatmapResponse
        {
            Found = true,
            Title = beatmap.Metadata.Title,
            Artist = beatmap.Metadata.Artist,
            Difficulty = beatmap.DifficultyName,
            BeatmapsetId = beatmap.BeatmapSet.OnlineID,
            DifficultyId = beatmap.OnlineID,
            Md5 = beatmap.MD5Hash,
            Bpm = 0, // TODO
            Sr = 0, // TODO
            Ar = beatmap.Difficulty.ApproachRate,
            Cs = beatmap.Difficulty.CircleSize,
            Hp = beatmap.Difficulty.DrainRate,
            Od = beatmap.Difficulty.OverallDifficulty,
            mapper = beatmap.Metadata.Author?.Username
        };
    }

    public static BeatmapResponse ProcessStableBeatmap(StableBeatmap beatmap)
    {
        if (beatmap == null) {
            return new BeatmapResponse { Found = false };
        }

        return new BeatmapResponse
        {
            Found = true,
            Title = beatmap.Title,
            Artist = beatmap.Artist,
            Difficulty = beatmap.Difficulty,
            BeatmapsetId = (int)beatmap.BeatmapsetId,
            DifficultyId = (int)beatmap.DifficultyId,
            Md5 = beatmap.Md5,
            Bpm = 0, // TODO
            Sr = 0, // TODO
            Ar = beatmap.Ar,
            Cs = beatmap.Cs,
            Hp = beatmap.Hp,
            Od = beatmap.Od,
            mapper = beatmap.Mapper
        };
    }
}

public class GetBeatmapsHandler : BaseMessageHandler<BeatmapsRequest, BeatmapsResponse>
{
    public override string MessageType => "get_beatmaps";
    public override async Task Handle(int id, bool send, BeatmapsRequest data) => await Task.CompletedTask;

    protected override Task<BeatmapsResponse?> ProcessRequest(BeatmapsRequest request)
    {
        var result = new BeatmapsResponse { Success = false, Beatmaps = [] };

        if (request?.Index == null) {
            return Task.FromResult<BeatmapsResponse?>(result);
        }

        if (request.Filtered == true && string.IsNullOrEmpty(request.Name)) {
            return Task.FromResult<BeatmapsResponse?>(result);
        }

        result = request.Filtered == true ? ProcessFilteredBeatmaps(request) : ProcessAllBeatmaps(request);
        return Task.FromResult<BeatmapsResponse?>(result);
    }

    private BeatmapsResponse ProcessFilteredBeatmaps(BeatmapsRequest request)
    {
        var result = new BeatmapsResponse { Success = false, Beatmaps = [] };
        
        if (string.IsNullOrEmpty(request.Name)) {
            return result;
        }

        var filtered_hashes = Manager.GetFilteredBeatmaps(request.Name);

        if (filtered_hashes == null || filtered_hashes.Count == 0) {
            return result;
        }

        int start_index = request.Index;
        int take_amount = request.Amount ?? 8;
        
        var selected_hashes = filtered_hashes.Skip(start_index).Take(take_amount);

        foreach (var hash in selected_hashes) {

            BeatmapResponse? beatmap_response;

            if (Manager.config.Lazer == true) {
                beatmap_response = GetBeatmapHandler.ProcessLazerBeatmap(hash);
            } else {
                beatmap_response = GetBeatmapHandler.ProcessStableBeatmap(hash);
            }

            if (beatmap_response != null && beatmap_response.Found) {
                result.Beatmaps.Add(beatmap_response);
            }
        }

        result.Success = true;
        return result;
    }

    private BeatmapsResponse ProcessAllBeatmaps(BeatmapsRequest request)
    {
        var result = new BeatmapsResponse { Success = false, Beatmaps = [] };

        if (Manager.config.Lazer == true) {

            var instance = RealmDB.GetInstance(Manager.config.LazerPath);
            var all_beatmaps = instance?.All<LazerBeatmap>();
            var total_count = all_beatmaps?.Count() ?? 0;

            if (all_beatmaps == null || total_count == 0) {
                return result;
            }

            int start_index = request.Index;
            int take_amount = request.Amount ?? 8;
            take_amount = Math.Min(take_amount, total_count - start_index);

            if (take_amount <= 0) {
                result.Success = true;
                return result;
            }

            var selected_beatmaps = all_beatmaps.ToList().Skip(start_index).Take(take_amount);
            
            foreach (var beatmap in selected_beatmaps) {
                result.Beatmaps.Add(GetBeatmapHandler.ProcessLazerBeatmap(beatmap));
            }
        } else {

            if (Manager.StableDatabase?.Beatmaps == null) {
                return result;
            }

            var all_beatmaps = Manager.StableDatabase.Beatmaps.Values.ToList();
            var total_count = all_beatmaps.Count;

            int start_index = request.Index;
            int take_amount = request.Amount ?? 8;
            take_amount = Math.Min(take_amount, total_count - start_index);

            if (take_amount <= 0) {
                result.Success = true;
                return result;
            }

            var selected_beatmaps = all_beatmaps.Skip(start_index).Take(take_amount);
            
            foreach (var beatmap in selected_beatmaps) {
                result.Beatmaps.Add(GetBeatmapHandler.ProcessStableBeatmap(beatmap));
            }
        }

        result.Success = true;
        return result;
    }
}

[MessagePackObject]
public class BeatmapsRequest 
{
    [Key("index")]
    public required int Index { get; set; }

    [Key("amount")]
    public int? Amount { get; set; }

    [Key("filtered")]
    public bool? Filtered { get; set; }

    [Key("name")]
    public string? Name { get; set; }
}

[MessagePackObject]
public class BeatmapsResponse 
{
    [Key("success")]
    public required bool Success { get; set; }

    [Key("beatmaps")]
    public List<BeatmapResponse>? Beatmaps { get; set; }
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

    [Key("local")]
    public bool Local = true; 
}