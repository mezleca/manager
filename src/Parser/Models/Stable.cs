public class StarRatingPair
{
    public uint Mod { get; set; }
    public float Difficulty { get; set; }
}

public class TimingPoint
{
    public double Length { get; set; }
    public double Offset { get; set; }
    public bool Inherited { get; set; }
}

public class StableBeatmap
{
    public uint Entry { get; set; }
    public required string Artist { get; set; }
    public required string ArtistUnicode { get; set; }
    public required string Title { get; set; }
    public required string TitleUnicode { get; set; }
    public required string Mapper { get; set; }
    public required string Difficulty { get; set; }
    public required string AudioFileName { get; set; }
    public required string Md5 { get; set; }
    public required string File { get; set; }
    public byte Status { get; set; }
    public ushort Hitcircle { get; set; }
    public ushort Sliders { get; set; }
    public ushort Spinners { get; set; }
    public ulong LastModification { get; set; }
    public required List<List<StarRatingPair>> StarRatingInfo { get; set; }
    public required List<TimingPoint> TimingPointsInfo { get; set; }
    public float Ar { get; set; }
    public float Cs { get; set; }
    public float Hp { get; set; }
    public float Od { get; set; }
    public double SliderVelocity { get; set; }
    public uint DrainTime { get; set; }
    public uint Length { get; set; }
    public uint AudioPreview { get; set; }
    public uint TimingPointsLength { get; set; }
    public uint DifficultyId { get; set; }
    public uint BeatmapsetId { get; set; }
    public uint ThreadId { get; set; }
    public byte GradeStandard { get; set; }
    public byte GradeTaiko { get; set; }
    public byte GradeCtb { get; set; }
    public byte GradeMania { get; set; }
    public ushort LocalOffset { get; set; }
    public float StackLeniency { get; set; }
    public byte Mode { get; set; }
    public required string Source { get; set; }
    public required string Tags { get; set; }
    public ushort OnlineOffset { get; set; }
    public required string Font { get; set; }
    public bool Unplayed { get; set; }
    public ulong LastPlayed { get; set; }
    public bool IsOsz2 { get; set; }
    public required string FolderName { get; set; }
    public ulong LastChecked { get; set; }
    public bool IgnoreSounds { get; set; }
    public bool IgnoreSkin { get; set; }
    public bool DisableStoryboard { get; set; }
    public bool DisableVideo { get; set; }
    public bool VisualOverride { get; set; }
    public uint LastModified { get; set; }
    public byte ManiaScrollSpeed { get; set; }
    public uint BeatmapEnd { get; set; }
    public bool Downloaded { get; set; }
    public bool Local { get; set; }
}

public class OsuStable
{
    public uint BeatmapOffsetStart { get; set; }
    public uint Version { get; set; }
    public uint Folders { get; set; }
    public bool AccountUnlocked { get; set; }
    public ulong LastUnlockedTime { get; set; }
    public required string Player { get; set; }
    public uint BeatmapsCount { get; set; }
    public required Dictionary<string, StableBeatmap> Beatmaps { get; set; }
    public uint PermissionId { get; set; }
    public uint BeatmapOffsetEnd { get; set; }
}

public class StableCollection
{
    public required string Name { get; set; }
    public uint Size { get; set; }
    public required List<string> Hashes { get; set; }
}

public class StableCollectionFile
{
    public uint Version { get; set; }
    public uint Size { get; set; }
    public required Dictionary<string, StableCollection> Collections { get; set; }
}