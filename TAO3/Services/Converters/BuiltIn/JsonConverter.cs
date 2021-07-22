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
    public class JsonConverterParameters : ConverterCommandParameters
    {
        public string? Type { get; set; }
    }

    public class JsonConverter : 
        IConverter<JsonSerializerSettings>, 
        IHandleCommand<JsonSerializerSettings, JsonConverterParameters>
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

        public void Configure(Command command)
        {
            command.Add(new Option(new[] { "-t", "--type" }, "The type that will be use to deserialize the input text"));
        }

        public async Task HandleCommandAsync(IConverterContext<JsonSerializerSettings> context, JsonConverterParameters args)
        {
            context.Settings ??= _defaultSettings;

            if (args.Type == "dynamic")
            {
                await context.DefaultHandleCommandAsync();
                return;
            }

            string text = await context.GetTextAsync();

            string clipboardVariableName = await context.CreatePrivateVariableAsync(text, typeof(string));
            string settingsVariableName = await context.CreatePrivateVariableAsync(context.Settings, typeof(JsonSerializerSettings));
            if (string.IsNullOrEmpty(args.Type))
            {
                string className = IdentifierUtils.ToCSharpIdentifier(args.Name!);
                string classDeclarations = JsonClassGenerator.GenerateClasses(text, className);

                await context.SubmitCodeAsync($@"{classDeclarations}{className} {args.Name} = JsonConvert.DeserializeObject<{className}>({clipboardVariableName}, {settingsVariableName});");
            }
            else
            {
                await context.SubmitCodeAsync($@"using Newtonsoft.Json;

{args.Type} {args.Name} = JsonConvert.DeserializeObject<{args.Type}>({clipboardVariableName}, {settingsVariableName});");
            }
        }
    }
}
