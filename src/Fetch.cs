using System;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        
        T? result = await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);
        return result;
    }

    public static async Task Download(string url, string path, Dictionary<string, string> headers)
    {
        CancellationToken cancellationToken = new();
        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        foreach (var kv in headers) { 
            request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
        }

        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        // 100mbs?
        await using var source = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var destination = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 102400, true);

        await source.CopyToAsync(destination, 102400, cancellationToken);
    }
}