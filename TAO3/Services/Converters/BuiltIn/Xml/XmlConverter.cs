using Newtonsoft.Json;
using System.CommandLine;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using TAO3.Converters.Json;
using TAO3.Internal.Utils;
using TAO3.TypeProvider;

namespace TAO3.Converters.Xml;

public record XmlInputConverterParameters
{
    public string? Type { get; init; }
}

public record XmlOutputConverterParameters
{
    public string? Type { get; init; }
}

//We can't use XmlSerializer because when we declare a class in a dot net interactive notebook,
//the name of class the class if prefixed with "Submission#0+" and the XmlSerializer doesn't like that.
//We can probably do a lot better then this, but it works for now
public class XmlConverter :
    IConverterTypeProvider<XmlWriterSettings, XmlInputConverterParameters>,
    IOutputConfigurableConverter<XmlWriterSettings, XmlOutputConverterParameters>
{
    private readonly Json.JsonConverter _jsonConverter;
    private readonly ITypeProvider<JsonSource> _typeProvider;

    public string Format => "xml";
    public IReadOnlyList<string> Aliases => Array.Empty<string>();
    public string MimeType => "application/xml";
    public IReadOnlyList<string> FileExtensions => new[] { ".xml" };
    public Dictionary<string, object> Properties { get; }
    public IDomCompiler DomCompiler => _typeProvider;

    public XmlConverter(Json.JsonConverter jsonConverter, ITypeProvider<JsonSource> typeProvider)
    {
        _jsonConverter = jsonConverter;
        _typeProvider = typeProvider;
        Properties = new Dictionary<string, object>();
    }

    public T Deserialize<T>(string text, XmlWriterSettings? settings)
    {
        XDocument document = XDocument.Parse(text);

        string jsonInput = JsonConvert.SerializeXNode(document, Newtonsoft.Json.Formatting.None);
        return _jsonConverter.Deserialize<T>(jsonInput, settings: null);
    }

    public string Serialize(object? value, XmlWriterSettings? settings)
    {
        JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = IgnoreEmptyCollectionContractResolver.Instance,
            NullValueHandling = NullValueHandling.Ignore
        };

        string json = _jsonConverter.Serialize(value, jsonSettings);

        //Todo: do something cleaner
        XmlDocument? doc;
        try
        {
            doc = JsonConvert.DeserializeXmlNode(json, value?.GetType()?.Name ?? "Root");
        }
        catch
        {
            doc = JsonConvert.DeserializeXmlNode(json);
        }

        if (doc == null)
        {
            throw new ArgumentException(nameof(value));
        }

        settings ??= GetDefaultSettings();
        using StringWriter stringWriter = new StringWriterWithEncoding(GetEncoding(doc) ?? Encoding.Unicode);
        using XmlWriter writer = XmlWriter.Create(stringWriter, settings);
        doc.Save(writer);
        return stringWriter.ToString();

        Encoding? GetEncoding(XmlDocument xmlDocument)
        {
            if (xmlDocument.FirstChild is XmlDeclaration xmlDeclaration)
            {
                return Encoding.GetEncoding(xmlDeclaration.Encoding);
            }

            return null;
        }
    }

    void IInputConfigurableConverter<XmlWriterSettings, XmlInputConverterParameters>.Configure(Command command)
    {
        Configure(command);
    }

    void IOutputConfigurableConverter<XmlWriterSettings, XmlOutputConverterParameters>.Configure(Command command)
    {
        Configure(command);
    }

    private static void Configure(Command command)
    {
        command.Add(new Option<string>(new[] { "-t", "--type" }, "The type that will be use to deserialize the input text"));
    }

    public XmlWriterSettings GetDefaultSettings()
    {
        return new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "    ",
            NewLineChars = Environment.NewLine,
            NewLineHandling = NewLineHandling.Replace
        };
    }

    XmlWriterSettings IInputConfigurableConverter<XmlWriterSettings, XmlInputConverterParameters>.BindParameters(XmlWriterSettings settings, XmlInputConverterParameters args)
    {
        return settings;
    }

    async Task<IDomType> IConverterTypeProvider<XmlWriterSettings, XmlInputConverterParameters>.ProvideTypeAsync(IConverterContext<XmlWriterSettings> context, XmlInputConverterParameters args)
    {
        if (args.Type != null)
        {
            return new DomClassReference(args.Type);
        }

        string text = await context.GetTextAsync();
        XDocument document = XDocument.Parse(text);
        string jsonInput = JsonConvert.SerializeXNode(document, Newtonsoft.Json.Formatting.None);

        return _typeProvider.DomParser.Parse(new JsonSource(context.VariableName, jsonInput));
    }
}
