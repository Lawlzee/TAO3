using Newtonsoft.Json;
using System.CommandLine;
using System.Reactive;
using TAO3.Converters.Json;
using TAO3.TypeProvider;
using JsonConverter = TAO3.Converters.Json.JsonConverter;

namespace TAO3.Converters.Default;

internal class DefaultConverter :
    IConverterTypeProvider<JsonSerializerSettings, JsonConverterInputParameters>,
    IOutputConfigurableConverter<JsonSerializerSettings, Unit>
{
    private readonly JsonConverter _jsonConverter;

    public IDomCompiler DomCompiler => _jsonConverter.DomCompiler;
    public string Format => "default";
    public IReadOnlyList<string> Aliases => new string[0];
    public string MimeType => _jsonConverter.MimeType;
    public IReadOnlyList<string> FileExtensions => new string[0];
    public Dictionary<string, object> Properties { get; }

    public DefaultConverter(JsonConverter jsonConverter)
    {
        _jsonConverter = jsonConverter;
        Properties = new Dictionary<string, object>();
    }

    public T Deserialize<T>(string text, JsonSerializerSettings? settings)
    {
        if (typeof(T) == typeof(string))
        {
            return (T)(object)text;
        }

        return _jsonConverter.Deserialize<T>(text, settings);
    }

    public string Serialize(object? value, JsonSerializerSettings? settings)
    {
        if (value is string str)
        {
            return str;
        }

        if (value is IEnumerable<string> enumeration)
        {
            return string.Join(Environment.NewLine, enumeration);
        }

        return _jsonConverter.Serialize(value, settings);
    }


    public void Configure(Command command)
    {
        ((IInputConfigurableConverter<JsonSerializerSettings, JsonConverterInputParameters>)_jsonConverter).Configure(command);
    }

    public JsonSerializerSettings BindParameters(JsonSerializerSettings settings, JsonConverterInputParameters args)
    {
        return ((IInputConfigurableConverter<JsonSerializerSettings, JsonConverterInputParameters>)_jsonConverter).BindParameters(settings, args);
    }

    
    public JsonSerializerSettings GetDefaultSettings()
    {
        return _jsonConverter.GetDefaultSettings();
    }

    public async Task<IDomType> ProvideTypeAsync(IConverterContext<JsonSerializerSettings> context, JsonConverterInputParameters args)
    {
        try
        {
            return await ((IConverterTypeProvider<JsonSerializerSettings, JsonConverterInputParameters>)_jsonConverter).ProvideTypeAsync(context, args);
        }
        catch
        {
            return new DomLiteral(typeof(string));
        }
    }
}
