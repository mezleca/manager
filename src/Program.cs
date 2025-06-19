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
      
            if (LIBVA_DRIVER_NAME == "nvidia") {
                // prevent webview not loading due to some stupid ass nvidia bug
                _ = setenv("WEBKIT_DISABLE_COMPOSITING_MODE", "1", 1);
                _ = setenv("WEBKIT_DMABUF_RENDERER_DISABLE_GBM", "1", 1);
            }

            // @TODO: chjeck if whatever webview used on windows is present
            // also use showmessagebox or something

            // @NOTE: may vary depending on the distro? idk
            if (!File.Exists("/usr/lib/libwebkit2gtk-4.0.so") && !Directory.Exists("/usr/lib/webkit2gtk-4.0")) {
                var command = "notify-send --app-name='manager' -h 'string:sound-name:message-new-instant' 'missing libwebkit2gtk' 'please install webkit2gtk on your distro vro'";
                Utils.Exec(command);
                Environment.Exit(1);
            }
        }

        Window.Initialize();
    }
}

