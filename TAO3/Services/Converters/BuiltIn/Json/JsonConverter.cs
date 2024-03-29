﻿using Newtonsoft.Json;
using System.CommandLine;
using System.Reactive;
using TAO3.TypeProvider;

namespace TAO3.Converters.Json;

public record JsonConverterInputParameters
{
    public string? Type { get; init; }
}

public class JsonConverter :
    IConverterTypeProvider<JsonSerializerSettings, JsonConverterInputParameters>,
    IOutputConfigurableConverter<JsonSerializerSettings, Unit>
{
    private readonly ITypeProvider<JsonSource> _typeProvider;

    public string Format => "json";
    public string MimeType => "application/json";
    public IReadOnlyList<string> Aliases => Array.Empty<string>();
    public IReadOnlyList<string> FileExtensions => new[] { ".json" };
    public Dictionary<string, object> Properties { get; }
    public IDomCompiler DomCompiler => _typeProvider;

    public JsonConverter(ITypeProvider<JsonSource> typeProvider)
    {
        _typeProvider = typeProvider;
        Properties = new Dictionary<string, object>();
    }

    public T Deserialize<T>(string text, JsonSerializerSettings? settings)
    {
        return JsonConvert.DeserializeObject<T>(text, settings ?? GetDefaultSettings())!;
    }

    public string Serialize(object? value, JsonSerializerSettings? settings)
    {
        return JsonConvert.SerializeObject(value, settings ?? GetDefaultSettings());
    }

    void IInputConfigurableConverter<JsonSerializerSettings, JsonConverterInputParameters>.Configure(Command command)
    {
        Configure(command);
    }

    void IOutputConfigurableConverter<JsonSerializerSettings, Unit>.Configure(Command command)
    {
        Configure(command);
    }

    private static void Configure(Command command)
    {
        command.Add(new Option<string>(new[] { "-t", "--type" }, "The type that will be use to deserialize the input text"));
    }

    public JsonSerializerSettings GetDefaultSettings()
    {
        return new JsonSerializerSettings
        {
            Formatting = Newtonsoft.Json.Formatting.Indented
        };
    }

    JsonSerializerSettings IInputConfigurableConverter<JsonSerializerSettings, JsonConverterInputParameters>.BindParameters(JsonSerializerSettings settings, JsonConverterInputParameters args)
    {
        return settings;
    }

    async Task<IDomType> IConverterTypeProvider<JsonSerializerSettings, JsonConverterInputParameters>.ProvideTypeAsync(IConverterContext<JsonSerializerSettings> context, JsonConverterInputParameters args)
    {
        if (args.Type != null)
        {
            return new DomClassReference(args.Type);
        }

        string json = await context.GetTextAsync();
        return _typeProvider.DomParser.Parse(new JsonSource(context.VariableName, json));
    }
}
