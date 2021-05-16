using Microsoft.DotNet.Interactive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.CodeGeneration;
using Xamasoft.JsonClassGenerator;

namespace TAO3.Converters
{
    public class JsonConverter : IConverter<JsonSerializerSettings>, IConfigurableConverter
    {
        private static readonly JsonSerializerSettings _defaultSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public string Format => "json";
        public string DefaultType => "dynamic";

        public object? Deserialize<T>(string text, JsonSerializerSettings? settings)
        {
            return JsonConvert.DeserializeObject<T>(text, settings ?? _defaultSettings); 
        }

        public string Serialize(object? value, JsonSerializerSettings? settings)
        {
            return JsonConvert.SerializeObject(value, settings ?? _defaultSettings);
        }

        public void ConfigureCommand(Command command, ConvertionContextProvider contextProvider)
        {
            command.Add(new Option(new[] { "-t", "--type" }, "The type that will be use to deserialize the input text"));

            command.Handler = CommandHandler.Create(async (string name, string settings, bool verbose, string type, KernelInvocationContext context) =>
            {
                IConverterContext<JsonSerializerSettings> converterContext = contextProvider.Invoke<JsonSerializerSettings>(name, settings, verbose, context);

                converterContext.Settings ??= _defaultSettings;

                if (type == "dynamic")
                {
                    await converterContext.DefaultHandle();
                    return;
                }

                string text = await converterContext.GetTextAsync();

                string clipboardVariableName = await converterContext.CreatePrivateVariable(text, typeof(string));
                string settingsVariableName = await converterContext.CreatePrivateVariable(converterContext.Settings, typeof(JsonSerializerSettings));
                if (string.IsNullOrEmpty(type))
                {
                    string className = IdentifierUtils.ToPascalCase(name);
                    string classDeclarations = JsonClassGenerator.GenerateClasses(text, className);

                    await converterContext.SubmitCodeAsync($@"{classDeclarations}{className} {name} = JsonConvert.DeserializeObject<{className}>({clipboardVariableName}, {settingsVariableName});");
                }
                else
                {
                    await converterContext.SubmitCodeAsync($@"using Newtonsoft.Json;

{type} {name} = JsonConvert.DeserializeObject<{type}>({clipboardVariableName}, {settingsVariableName});");
                }
            });
        }
    }
}
