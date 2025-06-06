using System.Runtime.InteropServices;
namespace Main;

public class Entry
{
    [DllImport("libc.so.6", SetLastError = true)]
    private static extern int setenv(string name, string value, int overwrite);

    [STAThread]
    static void Main(string[] args)
    {
        // classic nvidia moment
        if (Window.is_linux) {
            
            var LIBVA_DRIVER_NAME = Environment.GetEnvironmentVariable("LIBVA_DRIVER_NAME");
      
            // fix webview not loading due to some stupid ass nvidia bug
            if (LIBVA_DRIVER_NAME == "nvidia") {
                _ = setenv("WEBKIT_DISABLE_COMPOSITING_MODE", "1", 1);
                _ = setenv("WEBKIT_DMABUF_RENDERER_DISABLE_GBM", "1", 1);
            }
        }

        Window.Initialize();
    }
}

