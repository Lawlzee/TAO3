using System.CommandLine;
using System.Net.Http;
using TAO3.Converters;

namespace TAO3.IO;

public enum HttpVerb
{
    Get,
    Delete,
    Head,
    Options,
    Patch,
    Post,
    Put,
    Trace
}

internal record HttpSourceOptions
{
    public string Uri { get; init; } = null!;
    public HttpVerb? Verb { get; init; }
}

internal record HttpDestinationOptions
{
    public string Uri { get; init; } = null!;
    public HttpVerb? Verb { get; init; }
    public string? MediaType { get; init; }
    public IConverter Converter { get; init; } = null!;
}

internal class HttpIO : 
    ITextSource<HttpSourceOptions>,
    IDestination<HttpDestinationOptions>,
    IConfigurableSource,
    IConfigurableDestination
{
    private readonly HttpClient _httpClient;

    public string Name => "http";
    public IReadOnlyList<string> Aliases => Array.Empty<string>();

    public HttpIO(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    void IConfigurableSource.Configure(Command command)
    {
        command.Add(new Argument<string>("uri"));
        command.Add(new Option<HttpVerb>("--verb"));
    }

    public async Task<string> GetTextAsync(HttpSourceOptions options)
    {
        HttpMethod method = GetMethod(options.Verb ?? HttpVerb.Get);

        HttpResponseMessage response = await _httpClient.SendAsync(new HttpRequestMessage(method, options.Uri));
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    void IConfigurableDestination.Configure(Command command)
    {
        command.Add(new Argument<string>("uri"));
        command.Add(new Option<HttpVerb>("--verb"));
        command.Add(new Option<string>("--mediaType"));
    }

    public async Task SetTextAsync(string text, HttpDestinationOptions options)
    {
        HttpMethod method = GetMethod(options.Verb ?? HttpVerb.Post);

        HttpResponseMessage response = await _httpClient.SendAsync(new HttpRequestMessage(method, options.Uri)
        {
            Content = new StringContent(text, Encoding.UTF8, options.MediaType ?? options.Converter.MimeType)
        });
        response.EnsureSuccessStatusCode();
    }

    private HttpMethod GetMethod(HttpVerb verb)
    {
        switch (verb)
        {
            case HttpVerb.Get: return HttpMethod.Get;
            case HttpVerb.Delete: return HttpMethod.Delete;
            case HttpVerb.Head: return HttpMethod.Head;
            case HttpVerb.Options: return HttpMethod.Options;
            case HttpVerb.Patch: return HttpMethod.Patch;
            case HttpVerb.Post: return HttpMethod.Post;
            case HttpVerb.Put: return HttpMethod.Put;
            case HttpVerb.Trace: return HttpMethod.Trace;

        }

        throw new ArgumentException(nameof(verb));
    }
}
