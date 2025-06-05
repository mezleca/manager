namespace Main;

public class Config
{
    public string? StablePath { get; set; }
    public string? LazerPath { get; set; }
    public string? StableSongsPath { get; set; }
    public string? AccessToken { get; set; }
    public bool? Lazer { get; set; }

    public void UpdateFrom(Config other)
    {
        if (other.StablePath != null) StablePath = other.StablePath;
        if (other.LazerPath != null) LazerPath = other.LazerPath;
        if (other.StableSongsPath != null) StableSongsPath = other.StableSongsPath;
        if (other.AccessToken != null) AccessToken = other.AccessToken;
        if (other.Lazer != null) Lazer = other.Lazer;
    }
}
