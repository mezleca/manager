using MessagePack;
using System.Collections.Concurrent;
using Main;

namespace IPC;

public class MessageBus
{
    private readonly ConcurrentDictionary<string, IMessageHandler> _handlers = new();
    private static int _message_counter = 0;

    public void RegisterHandler<T>(IMessageHandler<T> handler) where T : class => _handlers[handler.MessageType] = handler;
    
    public async Task HandleMessage(string raw_message)
    {
        try
        {
            var raw_data = Convert.FromBase64String(raw_message);
            var msg_data = MessagePackSerializer.Deserialize<MessageData>(raw_data);


            if (_handlers.TryGetValue(msg_data.Type, out var handler))
            {
                Console.WriteLine(msg_data.Type);
                await handler.Handle(msg_data.Id, msg_data.Send, msg_data.Data);
                await SendConfirmation(msg_data.Id, msg_data.Type);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"message handling error: {ex}");
        }
    }
    
    public static async Task Send<T>(string message_type, T data) where T : class
    {
        await SendInternal(Interlocked.Increment(ref _message_counter), message_type, data, false);
    }

    private static async Task SendConfirmation(int id, string message_type)
    {
        await SendInternal(id, message_type, new { }, false);
    }
    
    public static async Task SendResponse<T>(int response_id, string message_type, T data) where T : class
    {
        await SendInternal(response_id, message_type, data, false);
    }
    
    private static async Task SendInternal<T>(int id, string message_type, T data, bool expect_response) where T : class
    {
        try
        {
            var serialized = MessagePackSerializer.Serialize(data);
            var message_data = new MessageData
            {
                Id = id,
                Type = message_type,
                Data = serialized,
                Send = expect_response
            };

            var serialized_data = MessagePackSerializer.Serialize(message_data);
            var base64 = Convert.ToBase64String(serialized_data);

            Window.window?.SendWebMessage(base64);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"send error: {ex.Message}");
        }
    }

    public void ShowHandlers()
    {
        foreach (var handler in _handlers)
        {
            Console.WriteLine($"registered handler: {handler.Key}");
        }
    }
}

public interface IMessageHandler
{
    string MessageType { get; }
    Task Handle(int id, bool send, byte[] data);
}

public interface IMessageHandler<T> : IMessageHandler where T : class
{
    Task Handle(int id, bool send, T data);
    
    async Task IMessageHandler.Handle(int id, bool send, byte[] data)
    {
        var typed_data = MessagePackSerializer.Deserialize<T>(data);
        await Handle(id, send, typed_data);
    }
}

public abstract class BaseMessageHandler<TRequest, TResponse> : IMessageHandler<TRequest> where TRequest : class where TResponse : class
{
    public abstract string MessageType { get; }
    
    public async Task Handle(int id, bool send, byte[] data)
    {
        var request = MessagePackSerializer.Deserialize<TRequest>(data);
        var response = await ProcessRequest(request);
        
        if (send && response != null) {
            await MessageBus.SendResponse(id, MessageType, response);
        }
    }
    
    public abstract Task Handle(int id, bool send, TRequest data);
    protected abstract Task<TResponse?> ProcessRequest(TRequest request);
}

[MessagePackObject]
public class MessageData
{
    [Key("id")]
    public required int Id { get; set; }
    
    [Key("type")]
    public required string Type { get; set; }
    
    [Key("data")]
    public required byte[] Data { get; set; }
    
    [Key("send")]
    public bool Send { get; set; }
}