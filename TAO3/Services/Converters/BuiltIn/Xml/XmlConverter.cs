using Microsoft.DotNet.Interactive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
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
    public class XmlConverterParameters : ConverterCommandParameters
    {
        public string? Type { get; set; }
    }

    //We can't use XmlSerializer because when we declare a class in a dot net interactive notebook,
    //the name of class the class if prefixed with "Submission#0+" and the XmlSerializer doesn't like that.
    //We can probably do a lot better then this, but it works for now
    public class XmlConverter : 
        IConverter<XmlWriterSettings>, 
        IHandleCommand<XmlWriterSettings, XmlConverterParameters>
    {
        private static readonly XmlWriterSettings _defaultSettings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "    ",
            NewLineChars = Environment.NewLine,
            NewLineHandling = NewLineHandling.Replace
        };

        private readonly TAO3.Converters.Json.JsonConverter _jsonConverter;
        private readonly ITypeProvider<JsonSource> _typeProvider;

        public string Format => "xml";
        public string DefaultType => "dynamic";

        public IReadOnlyList<string> Aliases => new[] { "XML" };

        public XmlConverter(TAO3.Converters.Json.JsonConverter jsonConverter, ITypeProvider<JsonSource> typeProvider)
        {
            _jsonConverter = jsonConverter;
            _typeProvider = typeProvider;
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
            XmlDocument? doc = JsonConvert.DeserializeXmlNode(json);

            if (doc == null)
            {
                throw new ArgumentException(nameof(value));
            }

            settings ??= _defaultSettings;
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

        public async Task HandleCommandAsync(IConverterContext<XmlWriterSettings> context, XmlConverterParameters args)
        {
            context.Settings ??= _defaultSettings;

            if (args.Type == "dynamic")
            {
                await context.DefaultHandleCommandAsync();
                return;
            }

            string text = await context.GetTextAsync();

            XDocument document = XDocument.Parse(text);

            string jsonInput = JsonConvert.SerializeXNode(document, Newtonsoft.Json.Formatting.None);

            string textVariableName = await context.CreatePrivateVariableAsync(jsonInput, typeof(string));

            if (string.IsNullOrEmpty(args.Type))
            {
                SchemaSerialization schema = _typeProvider.ProvideTypes(new JsonSource(args.Name!, jsonInput));
                await context.SubmitCodeAsync($@"{schema.Code}

{schema.RootType} {args.Name} = JsonConvert.DeserializeObject<{schema.RootType}>({textVariableName});");
            }
            else
            {
                await context.SubmitCodeAsync($@"using Newtonsoft.Json;

{args.Type} {args.Name} = JsonConvert.DeserializeObject<{args.Type}>({textVariableName});");
            }
        }
    }
}
