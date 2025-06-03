using MessagePack;
namespace IPC.Handlers;

public class Test
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
        [Key("success")]
        public bool Success { get; set; }
        [Key("content")]
        public required string Content { get; set; }
    }

    static public async Task Handle(int id, Request request)
    {
        await MessageHandler.Send(id, "test", new Response
        {
            Success = true,
            Content = "Hello World"
        });
    }
}
