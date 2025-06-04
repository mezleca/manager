namespace Main;

public static class Config
{
    public static string? StablePath { get; set; }
    public static string? StableSongsPath { get; set; }
    public static string? AccessToken { get; set; }

    public static void UpdateFrom(ConfigData other)
    {
        if (other.StablePath != null)
            StablePath = other.StablePath;

        if (other.StableSongsPath != null)
            StableSongsPath = other.StableSongsPath;

        if (other.AccessToken != null)
            AccessToken = other.AccessToken;
    }
}

public class ConfigData
{
    public string? StablePath { get; set; }
    public string? StableSongsPath { get; set; }
    public string? AccessToken { get; set; }
    public bool? Lazer { get; set; }
}