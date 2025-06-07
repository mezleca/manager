namespace Main.Parser;

public unsafe class Reader
{
    public static byte[]? ReadFile(string path)
    {
        bool exists = File.Exists(path);

        if (!exists) {
            return null;
        }

        return File.ReadAllBytes(path);
    }

    public static StableCollectionFile ReadCollection(byte[] buffer)
    {
        var collections = new StableCollectionFile{
            Collections = []
        };
        
        var offset = 0;

        fixed (byte* ptr = &buffer[0])
        {
            collections.Version = Binary.Read<uint>(ptr, &offset);
            collections.Size = Binary.Read<uint>(ptr, &offset);
            collections.Collections = [];

            for (int i = 0; i < collections.Size; i++)
            {
                var collection = new StableCollection
                {
                    Hashes = [],
                    Name = Binary.ReadString(ptr, &offset),
                    Size = Binary.Read<uint>(ptr, &offset)
                };

                for (int j = 0; j < collection.Size; j++)
                {
                    var hash = Binary.ReadString(ptr, &offset);
                    collection.Hashes.Add(hash);
                }

                collections.Collections.Add(collection.Name, collection);
            }
        }

        return collections;
    }

    public static StableDatabase? ReadStableDB(byte[] buffer) 
    {
        var offset = 0;
        var file = new StableDatabase { Beatmaps = new Dictionary<string, StableBeatmap>() };

        fixed (byte* ptr = &buffer[0]) {
            
            var starRatingData = new List<List<StarRatingPair>>(4);
            file.Version = Binary.Read<uint>(ptr, &offset);

            if (file.Version < 20140609) {
                throw new Exception("old ass file");
            }

            file.Folders = Binary.Read<uint>(ptr, &offset);
            file.AccountUnlocked = Binary.Read<bool>(ptr, &offset);
            file.LastUnlockedTime = Binary.Read<ulong>(ptr, &offset);
            file.Player = Binary.ReadString(ptr, &offset);
            file.BeatmapsCount = Binary.Read<uint>(ptr, &offset);

            var beatmaps = new Dictionary<string, StableBeatmap>((int)file.BeatmapsCount);

            for (uint i = 0; i < file.BeatmapsCount; i++) {
                StableBeatmap beatmap = new()
                {
                    BeatmapStart = offset,
                    Entry = file.Version < 20191106 ? Binary.Read<uint>(ptr, &offset) : 0,
                    Artist = Binary.ReadString(ptr, &offset),
                    ArtistUnicode = Binary.ReadString(ptr, &offset),
                    Title = Binary.ReadString(ptr, &offset),
                    TitleUnicode = Binary.ReadString(ptr, &offset),
                    Mapper = Binary.ReadString(ptr, &offset),
                    Difficulty = Binary.ReadString(ptr, &offset),
                    AudioFileName = Binary.ReadString(ptr, &offset),
                    Md5 = Binary.ReadString(ptr, &offset),
                    File = Binary.ReadString(ptr, &offset),
                    Status = Binary.Read<byte>(ptr, &offset),
                    Hitcircle = Binary.Read<ushort>(ptr, &offset),
                    Sliders = Binary.Read<ushort>(ptr, &offset),
                    Spinners = Binary.Read<ushort>(ptr, &offset),
                    LastModification = Binary.Read<ulong>(ptr, &offset),
                    Ar = Binary.Read<float>(ptr, &offset),
                    Cs = Binary.Read<float>(ptr, &offset),
                    Hp = Binary.Read<float>(ptr, &offset),
                    Od = Binary.Read<float>(ptr, &offset),
                    SliderVelocity = Binary.Read<double>(ptr, &offset)
                };

                for (int j = 0; j < 4; j++) {
                    uint length = Binary.Read<uint>(ptr, &offset);
                    List<StarRatingPair> pair = [];
                    for (uint k = 0; k < length; k++) {
                        var comb = new StarRatingPair();
                        if (file.Version < 20250107) {
                            Binary.Read<byte>(ptr, &offset);
                            comb.Mod = Binary.Read<uint>(ptr, &offset);
                            Binary.Read<byte>(ptr, &offset);
                            comb.Difficulty = Binary.Read<double>(ptr, &offset);
                        } else {
                            if (k > 0) {
                                // only read the first mod combination (nomod)
                                // might break stuff idk
                                offset += 10 * ((int)length - 1);
                                break;
                            }
                            Binary.Read<byte>(ptr, &offset);
                            comb.Mod = Binary.Read<uint>(ptr, &offset);
                            Binary.Read<byte>(ptr, &offset);
                            comb.Difficulty = Binary.Read<float>(ptr, &offset);
                        }
                        pair.Add(comb);
                    }
                    starRatingData.Add(pair);
                }

                beatmap.DrainTime = Binary.Read<uint>(ptr, &offset);
                beatmap.Length = Binary.Read<uint>(ptr, &offset);
                beatmap.AudioPreview = Binary.Read<uint>(ptr, &offset);
                beatmap.TimingPointsLength = Binary.Read<uint>(ptr, &offset);

                var timingPoints = new List<TimingPoint>((int)beatmap.TimingPointsLength);

                for (uint k = 0; k < beatmap.TimingPointsLength; k++) {
                    timingPoints.Add(new TimingPoint{
                        Length = Binary.Read<double>(ptr, &offset),
                        Offset = Binary.Read<double>(ptr, &offset),
                        Inherited = Binary.Read<bool>(ptr, &offset)
                    });
                }

                beatmap.TimingPointsInfo = timingPoints;
                beatmap.DifficultyId = Binary.Read<uint>(ptr, &offset);
                beatmap.BeatmapsetId = Binary.Read<uint>(ptr, &offset);
                beatmap.ThreadId = Binary.Read<uint>(ptr, &offset);
                beatmap.GradeStandard = Binary.Read<byte>(ptr, &offset);
                beatmap.GradeTaiko = Binary.Read<byte>(ptr, &offset);
                beatmap.GradeCtb = Binary.Read<byte>(ptr, &offset);
                beatmap.GradeMania = Binary.Read<byte>(ptr, &offset);
                beatmap.LocalOffset = Binary.Read<ushort>(ptr, &offset);
                beatmap.StackLeniency = Binary.Read<float>(ptr, &offset);
                beatmap.Mode = Binary.Read<byte>(ptr, &offset);
                beatmap.Source = Binary.ReadString(ptr, &offset);
                beatmap.Tags = Binary.ReadString(ptr, &offset);
                beatmap.OnlineOffset = Binary.Read<ushort>(ptr, &offset);
                beatmap.Font = Binary.ReadString(ptr, &offset);
                beatmap.Unplayed = Binary.Read<bool>(ptr, &offset);
                beatmap.LastPlayed = Binary.Read<ulong>(ptr, &offset);
                beatmap.IsOsz2 = Binary.Read<bool>(ptr, &offset);
                beatmap.FolderName = Binary.ReadString(ptr, &offset);
                beatmap.LastChecked = Binary.Read<ulong>(ptr, &offset);
                beatmap.IgnoreSounds = Binary.Read<bool>(ptr, &offset);
                beatmap.IgnoreSkin = Binary.Read<bool>(ptr, &offset);
                beatmap.DisableStoryboard = Binary.Read<bool>(ptr, &offset);
                beatmap.DisableVideo = Binary.Read<bool>(ptr, &offset);
                beatmap.VisualOverride = Binary.Read<bool>(ptr, &offset);

                beatmap.LastModified = Binary.Read<uint>(ptr, &offset);
                beatmap.ManiaScrollSpeed = Binary.Read<byte>(ptr, &offset);
                beatmap.BeatmapEnd = offset;

                beatmap.Downloaded = true;
                beatmap.StarRatingInfo = starRatingData;
                beatmaps[beatmap.Md5] = beatmap;
            }

            file.Beatmaps = beatmaps;
            file.ExtraStart = offset;
            file.PermissionId = Binary.Read<uint>(ptr, &offset);         
        }

        return file;
    }
}