using MessagePack;
namespace IPC.Handlers;

public class TestHandler
{
    [MessagePackObject]
    public class Request
    {
        [Key("content")]
        public required string? Content { get; set; }
    }

    [MessagePackObject]
    public class Response
    {
        [Key("content")]
        public required string Content { get; set; }
    }

    static public async Task Handle(int id, bool send, Request request)
    {
        await MessageHandler.Send(id, "test", new Response
        {
            Content = "Hello World"
        });
    }
}
