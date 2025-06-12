using Photino.NET;
using System.Drawing;
using IPC;
using System.Runtime.InteropServices;
using IPC.Handlers;
using Main.Manager;

namespace Main;

public class Window
{
    public static PhotinoWindow? window;
    public static bool is_linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    [STAThread]
    public static void Initialize()
    {
        // @TODO: figure out a way to use Chromeless mode and move the window on header drag
        window = new PhotinoWindow()
            .SetTitle("osu manager")
            .SetUseOsDefaultSize(false)
            .SetSize(1024, 800)
            .SetMinSize(800, 700)
            .Center()
            .SetResizable(true)
            .SetLogVerbosity(0)
            .Load("frontend/index.html");

        SetupHandlers();
        window.WaitForClose();
    }

    private static void SetupHandlers()
    {
        var bus = new MessageBus();

        bus.RegisterHandler(new GetCollectionHandler());
        bus.RegisterHandler(new LoadOsuDataHandler());
        bus.RegisterHandler(new ReloadCollectionHandler());
        bus.RegisterHandler(new UpdateConfigHandler());
        bus.RegisterHandler(new ShowDialogHandler());
        bus.RegisterHandler(new OpenHandler());
        bus.RegisterHandler(new GetBeatmapHandler());
        bus.RegisterHandler(new GetCollectionsHandler());
        bus.RegisterHandler(new GetBeatmapsHandler());
        bus.RegisterHandler(new SetCollectionHandler());

        bus.ShowHandlers();

        window?.RegisterWebMessageReceivedHandler((sender, message) => Task.Run(() => bus.HandleMessage(message)));
    }
}