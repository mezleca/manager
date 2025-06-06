using System.Runtime.InteropServices;
namespace Main;

public class Entry
{
    [DllImport("libc.so.6", SetLastError = true)]
    private static extern int setenv(string name, string value, int overwrite);

    [STAThread]
    static void Main(string[] args)
    {
        // classic nvidia + wayland moment
        if (Window.is_linux) {
            
            var wayland_display = Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
            var is_wayland = !string.IsNullOrEmpty(wayland_display);
            
            // fix webview not loading due to some stupid nvidia bug on wayland
            if (is_wayland) {
                _ = setenv("WEBKIT_DISABLE_COMPOSITING_MODE", "1", 1);
                _ = setenv("WEBKIT_DMABUF_RENDERER_DISABLE_GBM", "1", 1);
            }
        }

        Window.Initialize();
    }
}

