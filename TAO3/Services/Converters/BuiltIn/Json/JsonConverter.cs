using Microsoft.DotNet.Interactive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TAO3.CodeGeneration;
using TAO3.Converters.Json;
using TAO3.TypeProvider;

namespace TAO3.Converters.Json
{
    public record JsonConverterInputParameters
    {
        public string? Type { get; init; }
    }

    public class JsonConverter :
        IConverter<JsonSerializerSettings>,
        IHandleInputCommand<JsonSerializerSettings, JsonConverterInputParameters>,
        IOutputConfigurableConverterCommand<JsonSerializerSettings, Unit>
    {
        private readonly ITypeProvider<JsonSource> _typeProvider;

        public string Format => "json";
        public string DefaultType => "dynamic";
        public string MimeType => "application/json";
        public IReadOnlyList<string> Aliases => Array.Empty<string>();
        public Dictionary<string, object> Properties { get; }

        public JsonConverter(ITypeProvider<JsonSource> typeProvider)
        {
            _typeProvider = typeProvider;
            Properties = new Dictionary<string, object>();
        }

        public object? Deserialize<T>(string text, JsonSerializerSettings? settings)
        {
            return JsonConvert.DeserializeObject<T>(text, settings ?? GetDefaultSettings());
        }

        public string Serialize(object? value, JsonSerializerSettings? settings)
        {
            return JsonConvert.SerializeObject(value, settings ?? GetDefaultSettings());
        }

        public void Configure(Command command)
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

        public JsonSerializerSettings BindParameters(JsonSerializerSettings settings, JsonConverterInputParameters args)
        {
            return settings;
        }

        public async Task HandleCommandAsync(IConverterContext<JsonSerializerSettings> context, JsonConverterInputParameters args)
        {
            if (args.Type == "dynamic")
            {
                await context.DefaultHandleCommandAsync();
                return;
            }

            string json = await context.GetTextAsync();

            string clipboardVariableName = await context.CreatePrivateVariableAsync(json, typeof(string));
            string settingsVariableName = await context.CreatePrivateVariableAsync(context.Settings, typeof(JsonSerializerSettings));
            if (string.IsNullOrEmpty(args.Type))
            {
                SchemaSerialization schema = _typeProvider.ProvideTypes(new JsonSource(context.VariableName, json));
                await context.SubmitCodeAsync($@"{schema.Code}

{schema.RootType} {context.VariableName} = JsonConvert.DeserializeObject<{schema.RootType}>({clipboardVariableName}, {settingsVariableName});");
            }
            else
            {
                await context.SubmitCodeAsync($@"using Newtonsoft.Json;

{args.Type} {context.VariableName} = JsonConvert.DeserializeObject<{args.Type}>({clipboardVariableName}, {settingsVariableName});");
            }
        }
    }
}
