namespace Main;

public class Config
{
    public string? Id { get; set; }
    public string? Secret { get; set; }
    public string? Token { get; set; }
    public string? StablePath { get; set; }
    public string? LazerPath { get; set; }
    public string? ExportPath { get; set; }
    public string? StableSongsPath { get; set; }
    public string? AccessToken { get; set; }
    public bool? Lazer { get; set; }
    public bool? Local {get; set; }

    public void UpdateFrom(Config other)
    {
        if (other.Id != null) Id = other.Id;
        if (other.Secret != null) Secret = other.Secret;
        if (other.StablePath != null) StablePath = other.StablePath;
        if (other.LazerPath != null) LazerPath = other.LazerPath;
        if (other.ExportPath != null) ExportPath = other.ExportPath;
        if (other.StableSongsPath != null) StableSongsPath = other.StableSongsPath;
        if (other.AccessToken != null) AccessToken = other.AccessToken;
        if (other.Lazer != null) Lazer = other.Lazer;
        if (other.Local != null) Local = other.Local;
    }
}
