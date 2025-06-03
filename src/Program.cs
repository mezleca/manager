using System.Runtime.InteropServices;

namespace Main
{
    public class Manager
    {

        [DllImport("libc.so.6", SetLastError = true)]
        private static extern int setenv(string name, string value, int overwrite);
        [STAThread]
        static void Main(string[] args)
        {
            // classic nvidia + wayland moment
            if (Window.is_linux)
            {
                Console.WriteLine("enabling extra env variables");
                _ = setenv("GDK_BACKEND", "x11", 1);
                _ = setenv("WEBKIT_DISABLE_COMPOSITING_MODE", "1", 1);
                _ = setenv("WEBKIT_DISABLE_DMABUF_RENDERER", "1", 1);
            }

            Window.Initialize();
        }
    }
}
