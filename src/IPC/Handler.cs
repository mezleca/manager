using MessagePack;
using System.Collections.Concurrent;
using Main;

namespace IPC;

public class MessageHandler
{
    private readonly ConcurrentDictionary<string, Func<int, byte[], Task>> _handlers = new();

    public void RegisterHandler<T>(string message_type, Func<int, T, Task> handler)
    {
        _handlers[message_type] = async (id, data) =>
        {
            var message = MessagePackSerializer.Deserialize<T>(data);
            await handler(id, message);
        };
    }

    public async Task HandleMessage(string raw)
    {
        try
        {
            var data = Convert.FromBase64String(raw);
            var msgdata = MessagePackSerializer.Deserialize<MessageData>(data);

            if (!msgdata.Send)
            {
                return;
            }

            if (_handlers.TryGetValue(msgdata.Type, out var handler))
            {
                await handler(msgdata.Id, msgdata.Data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public static async Task Send<T>(int id, string message_type, T data) where T : class
    {
        try
        {
            var serialized = await Task.Run(() => MessagePackSerializer.Serialize(data));
            var message_data = new MessageData
            {
                Id = id,
                Type = message_type,
                Data = serialized
            };

            var serialized_data = await Task.Run(() => MessagePackSerializer.Serialize(message_data));
            var base64 = Convert.ToBase64String(serialized_data);

            Window.window?.SendWebMessage(base64);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"failed to send message: {ex.Message}");
        }
    }
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
    public bool Send { get; set;}
}
