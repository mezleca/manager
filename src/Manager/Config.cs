namespace Main;

public class Config
{
    // @TODO: maybe using indexedDB is a bad idea
    // in the future it will be saved on app folder 
    public string? Id { get; set; }
    public string? Secret { get; set; }
    public string? Token { get; set; }
    public string? StablePath { get; set; }
    public string? LazerPath { get; set; }
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
        if (other.StableSongsPath != null) StableSongsPath = other.StableSongsPath;
        if (other.AccessToken != null) AccessToken = other.AccessToken;
        if (other.Lazer != null) Lazer = other.Lazer;
        if (other.Local != null) Local = other.Local;

        Console.WriteLine("---- UPDATING CONFIG ----");
        Console.WriteLine($"StablePath: {StablePath}");
        Console.WriteLine($"LazerPath: {LazerPath}");
        Console.WriteLine($"StableSongs: {StableSongsPath}");
        Console.WriteLine($"Token: {AccessToken}");
        Console.WriteLine($"Lazer: {Lazer}");
        Console.WriteLine($"Local: {Local}");
        Console.WriteLine($"OsuId: {Id}");
        Console.WriteLine($"OsuSecret: {Secret}");
        Console.WriteLine("-------------------------");
    }
}
