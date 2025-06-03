using Photino.NET;
using System.Drawing;
using IPC;

namespace Main
{
    public class Window
    {

        public static PhotinoWindow? window;

        [STAThread]
        public static void Initialize()
        {
            window = new PhotinoWindow()
                .SetTitle("osu manager")
                .SetUseOsDefaultSize(false)
                .SetSize(new Size(1024, 800))
                .Center()
                .SetIconFile("frontend/icon.txt")
                .SetResizable(true)
                .Load("frontend/index.html");

            SetupHandlers();
            window.WaitForClose();
        }

        private static void SetupHandlers()
        {
            var messageHandler = new MessageHandler();

            messageHandler.RegisterHandler<TestRequest>("test", async (id, request) =>
            {
                await MessageHandler.Send<TestResponse>(id, "test", new TestResponse
                {
                    Success = true,
                    Content = "Hello World"
                });
            });

            window?.RegisterWebMessageReceivedHandler((sender, message) =>
            {
                _ = Task.Run(() => messageHandler.HandleMessage(message));
            });
        }
    }
}
