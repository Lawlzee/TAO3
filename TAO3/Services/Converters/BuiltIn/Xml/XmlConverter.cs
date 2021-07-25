using Microsoft.DotNet.Interactive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TAO3.CodeGeneration;
using Xamasoft.JsonClassGenerator;

namespace TAO3.Converters
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
        private readonly XmlWriterSettings _defaultSettings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "    ",
            NewLineChars = Environment.NewLine,
            NewLineHandling = NewLineHandling.Replace
        };

        public string Format => "xml";
        public string DefaultType => "dynamic";

        public object? Deserialize<T>(string text, XmlWriterSettings? settings)
        {
            XDocument document = XDocument.Parse(text);
            XElement rootElement = document.Root!;

            string jsonInput = JsonConvert.SerializeXNode(rootElement, Newtonsoft.Json.Formatting.None, omitRootObject: true);
            return new JsonConverter().Deserialize<T>(jsonInput, settings: null);
        }

        public string Serialize(object? value, XmlWriterSettings? settings)
        {
            string json = new JsonConverter().Serialize(value, settings: null);
            XmlDocument? doc = JsonConvert.DeserializeXmlNode(json);

            if (doc == null)
            {
                throw new ArgumentException(nameof(value));
            }

            StringBuilder sb = new();
            using XmlWriter writer = XmlWriter.Create(sb, settings ?? _defaultSettings);
            doc.Save(writer);
            return sb.ToString();
        }

        public void Configure(Command command)
        {
            command.Add(new Option(new[] { "-t", "--type" }, "The type that will be use to deserialize the input text"));
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
            XElement rootElement = document.Root!;

            string jsonInput = JsonConvert.SerializeXNode(rootElement, Newtonsoft.Json.Formatting.None, omitRootObject: true);

            string clipboardVariableName = await context.CreatePrivateVariableAsync(jsonInput, typeof(string));

            if (string.IsNullOrEmpty(args.Type))
            {
                string className = IdentifierUtils.ToCSharpIdentifier(args.Type!);
                string classDeclarations = JsonClassGenerator.GenerateClasses(jsonInput, className);

                await context.SubmitCodeAsync($@"{classDeclarations}{className} {args.Name} = JsonConvert.DeserializeObject<{className}>({clipboardVariableName});");
            }
            else
            {
                await context.SubmitCodeAsync($@"using Newtonsoft.Json;

{args.Type} {args.Name} = JsonConvert.DeserializeObject<{args.Type}>({clipboardVariableName});");
            }
        }
    }
}
