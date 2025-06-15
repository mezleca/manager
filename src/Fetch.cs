using System;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace App;

public class Fetch {

    private static readonly HttpClient _client = new HttpClient {
        Timeout = TimeSpan.FromSeconds(30)
    };

    public static async Task<T?> Get<T>(string url, Dictionary<string, string> headers)
    {
        CancellationToken cancellationToken = new();
        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        foreach (var kv in headers) {
            request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
        }

        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // annoying trim warning
        var context = (JsonSerializerContext?)options.TypeInfoResolver?.GetType().GetProperty("Context")?.GetValue(options.TypeInfoResolver) 
            ?? throw new InvalidOperationException("A JsonSerializerContext is required for trimming-safe deserialization.");

        if (context.GetType().GetProperty("Default")?.GetValue(context) is not JsonTypeInfo<T> typeInfo) {
            throw new InvalidOperationException("Could not obtain JsonTypeInfo for type " + typeof(T).FullName);
        }

        T? result = await JsonSerializer.DeserializeAsync<T>(stream, typeInfo, cancellationToken);
        return result;
    }

    public static async Task<(int status, Stream buffer)> Download(string url, Dictionary<string, string> headers)
    {
        CancellationToken cancellationToken = new();
        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        foreach (var kv in headers) {   
            request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
        }

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return ((int)response.StatusCode, stream);
    }
}