using System.Runtime.InteropServices;
using System;
namespace Main;

public class Entry
{
    [DllImport("libc.so.6", SetLastError = true)]
    private static extern int setenv(string name, string value, int overwrite);

    [STAThread]
    static void Main(string[] args)
    {
        if (Window.is_linux) {
            
            var LIBVA_DRIVER_NAME = Environment.GetEnvironmentVariable("LIBVA_DRIVER_NAME");
            var XDG_SESSION_TYPE = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");

            // force wayland
            if (XDG_SESSION_TYPE == "wayland") {      
                _ = setenv("GDK_BACKEND", "wayland", 1);
            }
      
            // fix webview not loading due to some stupid ass nvidia bug
            if (LIBVA_DRIVER_NAME == "nvidia") {
                _ = setenv("WEBKIT_DISABLE_COMPOSITING_MODE", "1", 1);
            }

            // check if we have webkit2gtk installed
            if (!File.Exists("/usr/lib/libwebkit2gtk-4.1.so") && !Directory.Exists("/usr/lib/webkit2gtk-4.1")) {
                var command = "notify-send --app-name='manager' -h 'string:sound-name:message-new-instant' 'missing webkit2gtk-4.1' 'please install webkit2gtk-4.1 bro'";
                Utils.Exec(command);
                Environment.Exit(1);
            }
        }

        Window.Initialize();
    }
}

