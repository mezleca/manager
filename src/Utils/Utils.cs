using System.Diagnostics;
using System.Text;

namespace Main;

public static class Utils
{
    public static (string stdout, int exitCode) Exec(string command)
    {
        var process_info = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = process_info };
        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                stdout.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                stderr.AppendLine(e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        return (stdout.ToString().Trim(), process.ExitCode);
    }
    
    public static string GetLinuxPath()
    {
        string home_dir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string default_wine_path = Path.Combine(home_dir, ".local", "share", "osu-wine", "osu");
        string wine_custom_path = Path.Combine(home_dir, ".local", "share", "osuconfig", "osu_path");

        if (Directory.Exists(default_wine_path)) {
            return default_wine_path;
        }

        string command = $"[ -e \"{wine_custom_path}\" ] && echo 1";
        var (_, exit_code) = Exec(command);
        
        if (exit_code == 0) {
            try {
                string data = File.ReadAllText(wine_custom_path);
                return data.Trim();
            }
            catch {
                
            }
        }

        return default_wine_path;
    }

    public static string GetWinPath()
    {
        string local_app = Environment.GetEnvironmentVariable("LOCALAPPDATA") ?? "";
        
        if (string.IsNullOrEmpty(local_app)) {

            string profile = Environment.GetEnvironmentVariable("USERPROFILE") ?? "";
            
            if (!string.IsNullOrEmpty(profile)) {
                local_app = Path.Combine(profile, "AppData", "Local");
            }
        }

        string osu_path = Path.Combine(local_app ?? "", "osu!");
        
        if (Directory.Exists(osu_path)) {
            return osu_path;
        }

        return osu_path;
    }
}