using Microsoft.DotNet.Interactive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TAO3.CodeGeneration;
using TAO3.Converters.Json;
using TAO3.Internal.Utils;
using TAO3.TypeProvider;

namespace TAO3.Converters.Xml
{
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
        IConverter<XmlWriterSettings>,
        IInputTypeProvider<XmlWriterSettings, XmlInputConverterParameters>,
        IOutputConfigurableConverterCommand<XmlWriterSettings, XmlOutputConverterParameters>
    {
        private readonly TAO3.Converters.Json.JsonConverter _jsonConverter;
        private readonly ITypeProvider<JsonSource> _typeProvider;

        public string Format => "xml";
        public IReadOnlyList<string> Aliases => Array.Empty<string>();
        public string MimeType => "application/xml";
        public string DefaultType => "dynamic";
        public Dictionary<string, object> Properties { get; }
        public IDomCompiler DomCompiler => _typeProvider;

        public XmlConverter(TAO3.Converters.Json.JsonConverter jsonConverter, ITypeProvider<JsonSource> typeProvider)
        {
            _jsonConverter = jsonConverter;
            _typeProvider = typeProvider;
            Properties = new Dictionary<string, object>();
        }

        public object? Deserialize<T>(string text, XmlWriterSettings? settings)
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

        public void Configure(Command command)
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

        public XmlWriterSettings BindParameters(XmlWriterSettings settings, XmlInputConverterParameters args)
        {
            return settings;
        }

        public async Task<InferedType> ProvideTypeAsync(IConverterContext<XmlWriterSettings> context, XmlInputConverterParameters args)
        {
            if (args.Type == "dynamic")
            {
                return new InferedType(new DomClassReference(typeof(ExpandoObject).FullName!));
            }

            if (args.Type != null)
            {
                return new InferedType(new DomClassReference(args.Type));
            }

            string text = await context.GetTextAsync();
            XDocument document = XDocument.Parse(text);
            string jsonInput = JsonConvert.SerializeXNode(document, Newtonsoft.Json.Formatting.None);

            IDomType domType = _typeProvider.DomParser.Parse(new JsonSource(context.VariableName, jsonInput));

            return new InferedType(domType);
        }
    }
}
