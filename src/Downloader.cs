using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace App;

public class Downloader 
{
    private static List<Download> downloads = new();
    private static List<Mirror> mirrors = [];
    private static Dictionary<string, BeatmapApiResponse> beatmap_cache = [];
    private static string token = ""; 
    private static bool processing = true;

    public static async Task Initialize() 
    {
        while (processing) 
        {
            // ensure we have the access_token for the osu! api (in case no id is passed)
            if (string.IsNullOrEmpty(token)) {
                throw new Exception("null or empty token");
            }

            // wait until new download
            if (downloads.Count == 0 || mirrors.Count == 0) {
                await Task.Delay(100);
                continue;
            }

            // process next download
            await Process(downloads.First());

            // remove the current download from download queue
            downloads.RemoveAt(0);
        }

        Console.WriteLine("finished");
    }

    public static async Task<BeatmapApiResponse?> GetBeatmapInformation(string hash) 
    {
        var cached_beatmap = beatmap_cache[hash];

        // return cached beatmap if possible
        if (cached_beatmap != null) {
            return cached_beatmap;
        }

        var headers = new Dictionary<string, string>{    
            { "Authorization", $"Bearer {token}" }
        };

        var result = await Fetch.Get<BeatmapApiResponse>($"https://osu.ppy.sh/api/v2/beatmaps/lookup?checksum={hash}", headers);

        if (result == null) {
            return null;
        }

        beatmap_cache.Add(hash, result);
        return result;
    }

    public static async Task Process(Download download) 
    {
        Console.WriteLine($"starting new download {download.Name}");

        if (download.Beatmaps?.Count == 0) {
            return;
        }

        for (int i = 0; i < download.Beatmaps?.Count; i++) {
            
            var beatmap = download.Beatmaps[i];
            var beatmap_data = await GetBeatmapInformation(beatmap.Md5);

            if (beatmap == null) {
                continue;
            }

            Console.WriteLine(beatmap_data?.Id);
        }
    }

    public static void SetToken(string _token) {
        token = _token;
    }

    public static void AddMirror(Mirror new_mirror) {
        mirrors.Add(new_mirror);
    }

    public static void AddDownload(Download new_download) {
        downloads.Add(new_download);
    }

    public static void RemoveMirror(string name) 
    {
        var index = mirrors.FindIndex((m) => m.Name == name);
        if (index != -1) {
            mirrors.RemoveAt(index);
        }
    }

    public static void RemoveDownload(string Id) 
    {
        var index = downloads.FindIndex((m) => m.Id == Id);
        if (index != -1) {
            downloads.RemoveAt(index);
        }
    }

    public static void Finish() {
        processing = false;
    }
}

public class BeatmapApiResponse {
    [JsonPropertyName("beatmapset_id")]
    public required int BeatmapSetId { get; set; }

    [JsonPropertyName("difficulty_rating")]
    public required double Difficulty { get; set; }

    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("mode")]
    public required string Mode { get; set; }

    [JsonPropertyName("status")]
    public required string Status { get; set; }

    [JsonPropertyName("total_length")]
    public required int Length { get; set; }

    [JsonPropertyName("user_id")]
    public required int UserId { get; set; }

    [JsonPropertyName("version")]
    public required string Version { get; set; } 
}

public class DownloadBeatmap {
    public required string Md5 { get; set; }
    public int? Id { get; set; }
}

public class Download {
    public required string Id { get; set; }
    public string? Name { get; set; }
    public List<DownloadBeatmap>? Beatmaps { get; set; }
}

public class Mirror {
    public required string Url { get; set; }
    public required string Name { get; set; }
    public long? Cooldown { get; set; }
}