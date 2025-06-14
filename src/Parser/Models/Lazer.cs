using Realms;

namespace Main.Manager;

[MapTo("BeatmapDifficulty")]
public class LazerBeatmapDifficulty : EmbeddedObject
{
    public float DrainRate { get; set; }
    public float CircleSize { get; set; }
    public float OverallDifficulty { get; set; }
    public float ApproachRate { get; set; }
    public double SliderMultiplier { get; set; }
    public double SliderTickRate { get; set; }
}

[MapTo("BeatmapMetadata")]
public class LazerBeatmapMetadata : RealmObject
{
    public string? Title { get; set; }
    public string? TitleUnicode { get; set; }
    public string? Artist { get; set; }
    public string? ArtistUnicode { get; set; }
    public RealmUser? Author { get; set; }
    public string? Source { get; set; }
    public string? Tags { get; set; }
    public int PreviewTime { get; set; }
    public string? AudioFile { get; set; }
    public string? BackgroundFile { get; set; }
}

[MapTo("BeatmapUserSettings")]
public class LazerBeatmapUserSettings : EmbeddedObject
{
    public double Offset { get; set; }
}

public class RealmUser : EmbeddedObject
{
    public int OnlineID { get; set; }
    public string? Username { get; set; }
    public string? CountryCode { get; set; }
}

public class Ruleset : RealmObject
{
    [PrimaryKey]
    public string? ShortName { get; set; }
    
    [Indexed]
    public int OnlineID { get; set; } = -1;
    
    public string? Name { get; set; }
    public string? InstantiationInfo { get; set; }
    public int LastAppliedDifficultyVersion { get; set; }
    public bool Available { get; set; }
}

[MapTo("File")]
public class RealmFile : RealmObject
{
    [PrimaryKey]
    public string? Hash { get; set; }
}

public class RealmNamedFileUsage : EmbeddedObject
{
    public RealmFile? File { get; set; }
    public string? Filename { get; set; }
}

[MapTo("BeatmapCollection")]
public class LazerCollection : RealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; }
    
    public string? Name { get; set; }
    public IList<string> BeatmapMD5Hashes { get; } = null!;
    public DateTimeOffset LastModified { get; set; }
}

[MapTo("BeatmapSet")]
public class LazerBeatmapSet : RealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; }
    
    [Indexed]
    public int OnlineID { get; set; } = -1;
    
    public DateTimeOffset DateAdded { get; set; }
    public DateTimeOffset? DateSubmitted { get; set; }
    public DateTimeOffset? DateRanked { get; set; }
    public IList<LazerBeatmap> Beatmaps { get; } = null!;
    public IList<RealmNamedFileUsage> Files { get; } = null!;
    public int Status { get; set; } = 0;
    public bool DeletePending { get; set; } = false;
    public string? Hash { get; set; }
    public bool Protected { get; set; } = false;
}

[MapTo("Beatmap")]
public class LazerBeatmap : RealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; } 
    
    public string? DifficultyName { get; set; }
    public Ruleset Ruleset { get; set; } = null!;
    public LazerBeatmapDifficulty Difficulty { get; set; } = null!;
    public LazerBeatmapMetadata Metadata { get; set; } = null!;
    public LazerBeatmapUserSettings UserSettings { get; set; } = null!;
    public LazerBeatmapSet BeatmapSet { get; set; } = null!;
    
    [Indexed]
    public int OnlineID { get; set; } = -1;
    
    public double Length { get; set; } = 0;
    public double BPM { get; set; } = 0;
    public string? Hash { get; set; }
    public double StarRating { get; set; } = -1;
    public string? MD5Hash { get; set; }
    public string? OnlineMD5Hash { get; set; }
    public DateTimeOffset? LastLocalUpdate { get; set; }
    public DateTimeOffset? LastOnlineUpdate { get; set; }
    public int Status { get; set; } = 0;
    public bool Hidden { get; set; } = false;
    public int EndTimeObjectCount { get; set; } = -1;
    public int TotalObjectCount { get; set; } = -1;
    public DateTimeOffset? LastPlayed { get; set; }
    public int BeatDivisor { get; set; } = 4;
    public double? EditorTimestamp { get; set; }
}