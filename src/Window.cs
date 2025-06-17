using System.Runtime.InteropServices;
using Ipc.Handlers;
using IPC;
using IPC.Handlers;
using Main.Manager;
using Photino.NET;

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
            .SetSize(1366, 800)
            .SetMinSize(1024, 700)
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

        Console.WriteLine("initializing handlers");

        // collections
        bus.RegisterHandler(new GetCollectionHandler());
        bus.RegisterHandler(new GetCollectionsHandler());
        bus.RegisterHandler(new SetCollectionHandler());
        bus.RegisterHandler(new ReloadCollectionHandler());

        // beatmaps
        bus.RegisterHandler(new GetBeatmapsHandler());

        // osu / files
        bus.RegisterHandler(new LoadOsuDataHandler());
        bus.RegisterHandler(new UpdateConfigHandler());
        bus.RegisterHandler(new ShowDialogHandler());

        // extra
        bus.RegisterHandler(new OpenHandler());
        bus.RegisterHandler(new GetBeatmapHandler());

        // downloader
        bus.RegisterHandler(new AddDownloadHandler());
        bus.RegisterHandler(new DownloadHandler());
        bus.RegisterHandler(new UpdateDownloadHandler());
        bus.RegisterHandler(new GetDownloadsHandler());
        bus.RegisterHandler(new AddTokenHandler());
        bus.RegisterHandler(new AddMirrorHandler());
        bus.RegisterHandler(new RemoveMirrorHandler());

        window?.RegisterWebMessageReceivedHandler((sender, message) => Task.Run(() => bus.HandleMessage(message)));
    }
}