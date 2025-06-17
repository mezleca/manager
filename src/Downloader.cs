using System.Text.Json.Serialization;
using Main;
using Main.Manager;

namespace App;

public class Downloader 
{
    private static List<Download> downloads = new();
    private static List<Mirror> mirrors = [];
    private static List<int> rate_values = new() { 429, 503, 504 };
    private static Dictionary<string, BeatmapApiResponse> beatmap_cache = [];
    private static string token = "";

    private static bool processing = true;
    private static Dictionary<string, int> download_progress = [];
    private static readonly object lock_object = new object();
    
    private static readonly SemaphoreSlim download_semaphore = new(3);

    public static async Task Initialize() 
    {
        processing = true;

        while (processing) 
        {
            if (string.IsNullOrEmpty(token)) {
                throw new Exception("null or empty token");
            }

            // wait until new download
            if (downloads.Count == 0 || mirrors.Count == 0) {
                await Task.Delay(100);
                continue;
            }

            // process next download
            var next = downloads.First();
            await ProcessParallel(next);

            // only continue if we're not "paused"
            if (!processing) {
                break;
            }

            // remove the current download from queue
            lock (lock_object) {
                if (downloads.Count > 0) {
                    downloads.RemoveAt(0);
                }
                download_progress.Remove(next.Name);
            }
        }

        Console.WriteLine("finished");
    }

    public static async Task<BeatmapApiResponse?> GetBeatmapInformation(string hash) 
    {
        // check if beatmap is cached
        if (beatmap_cache.TryGetValue(hash, out var cached_beatmap)) {
            return cached_beatmap;
        }

        var headers = new Dictionary<string, string>{    
            { "Authorization", $"Bearer {token}" }
        };

        var result = await Fetch.Get<BeatmapApiResponse>($"https://osu.ppy.sh/api/v2/beatmaps/lookup?checksum={hash}", headers);

        if (result == null) {
            return null;
        }

        beatmap_cache[hash] = result;
        return result;
    }

    private static async Task<Stream?> GetOsz(int id) {
        
        for (int i = 0; i < mirrors.Count; i++) {

            var mirror = mirrors[i];
            var url = mirror.Url;

            // check if we are on a cooldown
            if (mirror.Cooldown != null) {
                // only remove the cooldown after 5 min
                if ((DateTime.Now - mirror.Cooldown.Value).TotalMinutes < 5) {
                    continue;
                }

                mirror.Cooldown = null;
                mirrors[i] = mirror;

                Console.WriteLine($"removed rate limit from {mirror.Name}");
            }

            // prevent double "/"
            if (url.EndsWith('/')) {
                url = url.Substring(0, url.Length - 1);
            } 

            var (status, buffer) = await Fetch.Download($"{url}/{id}", []);

            // add small cooldown if we get rate limited
            if (rate_values.Contains(status)) {
                Console.WriteLine($"added rate limit to {mirror.Name}");
                mirror.Cooldown = DateTime.Now;
                mirrors[i] = mirror;
                continue;
            }

            if (buffer != null) {
                return buffer;
            }
        }

        return null;
    }

    // nova funcao para processar beatmaps em paralelo
    public static async Task ProcessParallel(Download download) 
    {
        if (download.Beatmaps?.Count == 0) {
            return;
        }

        // get current progress or start from 0
        var start_index = download_progress.TryGetValue(download.Name, out var progress) ? progress : 0;
        var total_count = download.Beatmaps?.Count ?? 0;
        var tasks = new List<Task>();
        
        for (int i = start_index; i < total_count; i++) {
            // ensure the download is not "paused"
            if (!processing) {
                Console.WriteLine($"not processing {i} due to processing == false");
                break;
            }

            if (download.Beatmaps == null) {
                break;
            }

            var beatmap = download.Beatmaps[i];
            
            var task = ProcessBeatmaps(download, beatmap, i);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

    private static async Task ProcessBeatmaps(Download download, DownloadBeatmap beatmap, int index) 
    {
        // wait for a new "slot"
        await download_semaphore.WaitAsync();

        // check if the download is still on the queue
        lock (lock_object) {
            if (!downloads.Contains(download)) {
                Console.WriteLine($"cancelling {download.Name}");
                download_semaphore.Release();
                return;
            }
        }
        
        try {
            await ProcessBeatmap(download, beatmap, index);
        }
        catch(Exception ex) {
            Console.WriteLine(ex);
        }

        download_semaphore.Release();
    }

    private static async Task<bool> ProcessBeatmap(Download? download, DownloadBeatmap beatmap, int index) 
    {
        if (!processing) {
            return false;
        }

        var data = await GetBeatmapInformation(beatmap.Md5);

        if (data == null) {
            Console.WriteLine($"received null for {beatmap.Md5}");
            return false;
        }

        var result = await GetOsz(data.Id);

        if (result == null) {
            Console.WriteLine($"failed to download beatmap from {data.Id}");
            // @TODO: tell something to the frontend
            if (download != null) { }
            return false;
        }

        var save_path = GetPath();

        // if we dont have a save location, stop the current download
        if (save_path == null) {
            // @TODO: tell something to the frontend
            if (download != null) { }
            processing = false;
            return false;
        }

        _ = Utils.SaveFile(result, save_path);

        // save the current index (next beatmap to download)
        if (download != null) { 
            lock (lock_object) {
                if (download_progress.TryGetValue(download.Name, out int value)) {
                    download_progress[download.Name] = Math.Max(value, index + 1);
                } else {
                    download_progress[download.Name] = index + 1;
                }
            }
        }

        return true;
    }

    public static void ResumeDownload() 
    {
        if (downloads.Count > 0) {
            processing = true;
            Console.WriteLine($"resuming download for: {downloads[0].Name}");
        }
    }

    public static void StopDownload() 
    {
        if (downloads.Count > 0) {
            Console.WriteLine($"stopping download for: {downloads[0].Name}");
        }

        processing = false;
    }

    public static string? GetPath() 
    {
        if (Manager.config.Lazer == true) {
            return Manager.config?.ExportPath;
        }

        return Manager.config.StableSongsPath;    
    }

    public static void SetToken(string _token) {
        token = _token;
    }

    public static void AddMirror(Mirror new_mirror) {
        lock (lock_object) {
            mirrors.Add(new_mirror);
        }
    }

    public static void AddDownload(Download new_download) {
        lock (lock_object) {
            downloads.Add(new_download);
        }
    }

    public static void RemoveMirror(string name) 
    {
        lock (lock_object) {
            var index = mirrors.FindIndex((m) => m.Name == name);
            if (index != -1) {
                mirrors.RemoveAt(index);
            }
        }
    }

    public static void RemoveDownload(string name) 
    {
        lock (lock_object) {
            var index = downloads.FindIndex((m) => m.Name == name);
            if (index != -1) {
                downloads.RemoveAt(index);
            }
        }
    }

    public static List<Download> Downloads() => downloads; 
    public static int Size() => downloads.Count;
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
    public required string Name { get; set; }
    public required List<DownloadBeatmap> Beatmaps { get; set; }
}

public class Mirror {
    public required string Url { get; set; }
    public required string Name { get; set; }
    public DateTime? Cooldown { get; set; }
}