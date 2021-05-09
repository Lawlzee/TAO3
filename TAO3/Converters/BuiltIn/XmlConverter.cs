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
using TAO3.Internal.CodeGeneration;
using Xamasoft.JsonClassGenerator;

namespace TAO3.Converters
{
    //We can't use XmlSerializer because when we declare a class in a dot net interactive notebook,
    //the name of class the class if prefixed with "Submission#0+" and the XmlSerializer doesn't like that.
    //We can probably do a lot better then this, but it works for now
    public class XmlConverter : IConverter<XmlWriterSettings>, IConfigurableConverter
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
            XmlDocument doc = JsonConvert.DeserializeXmlNode(json);

            StringBuilder sb = new();
            using XmlWriter writer = XmlWriter.Create(sb, settings ?? _defaultSettings);
            doc.Save(writer);
            return sb.ToString();
        }

        public void ConfigureCommand(Command command, ConvertionContextProvider contextProvider)
        {
            command.Add(new Option(new[] { "-t", "--type" }, "The type that will be use to deserialize the input text"));

            command.Handler = CommandHandler.Create(async (string source, string name, string settings, bool verbose, string type, KernelInvocationContext context) =>
            {
                IConverterContext<XmlWriterSettings> converterContext = contextProvider.Invoke<XmlWriterSettings>(source, name, settings, verbose, context);

                converterContext.Settings ??= _defaultSettings;

                if (type == "dynamic")
                {
                    await converterContext.DefaultHandle();
                    return;
                }

                string text = await converterContext.GetTextAsync();

                XDocument document = XDocument.Parse(text);
                XElement rootElement = document.Root!;

                string jsonInput = JsonConvert.SerializeXNode(rootElement, Newtonsoft.Json.Formatting.None, omitRootObject: true);

                string clipboardVariableName = await converterContext.CreatePrivateVariable(jsonInput, typeof(string));

                if (string.IsNullOrEmpty(type))
                {
                    string className = IdentifierUtils.ToPascalCase(name);
                    string classDeclarations = JsonClassGenerator.GenerateClasses(jsonInput, className);

                    await converterContext.SubmitCodeAsync($@"{classDeclarations}{className} {name} = JsonConvert.DeserializeObject<{className}>({clipboardVariableName});");
                }
                else
                {
                    await converterContext.SubmitCodeAsync($@"using Newtonsoft.Json;

{type} {name} = JsonConvert.DeserializeObject<{type}>({clipboardVariableName});");
                }
            });
        }
    }
}
