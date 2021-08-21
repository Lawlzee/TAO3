using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TAO3.Internal.CodeGeneration;
using TAO3.TypeProvider;

namespace TAO3.Converters.Csv
{
    public class CsvConverterInputParameters
    {
        public string? Separator { get; set; }
        public string? Type { get; set; }
    }

    public class CsvConverterOutputParameters
    {
        public string? Separator { get; set; }
    }

    public class CsvConverter : 
        IConverter<CsvConfiguration>,  
        IHandleInputCommand<CsvConfiguration, CsvConverterInputParameters>,
        IOutputConfigurableConverterCommand<CsvConfiguration, CsvConverterOutputParameters>

    {
        private readonly ITypeProvider<CsvSource> _typeProvider;
        private readonly bool _hasHeader;

        public string Format => _hasHeader ? "csvh" : "csv";

        public string DefaultType => "var";

        public IReadOnlyList<string> Aliases => new[]
        {
            _hasHeader ? "CSVH" : "CSV"
        };

        public CsvConverter(ITypeProvider<CsvSource> typeProvider, bool hasHeader)
        {
            _typeProvider = typeProvider;
            _hasHeader = hasHeader;
        }

        public CsvConfiguration GetDefaultSettings()
        {
            return new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = _hasHeader
            };
        }

        public object? Deserialize<T>(string text, CsvConfiguration? settings)
        {
            bool isDynamic = typeof(T) == typeof(ExpandoObject);
            bool isStringArray = typeof(T) == typeof(string[]);

            CsvConfiguration config = settings ?? GetDefaultSettings();
            
            //Ugly fix for https://github.com/JoshClose/CsvHelper/issues/1262
            if (!isDynamic && !isStringArray && !config.HasHeaderRecord)
            {
                List<string> propertyNames = typeof(T).GetProperties()
                    .Select(x => x.GetCustomAttributes(typeof(NameAttribute), inherit: true)
                        .OfType<NameAttribute>()
                        .SelectMany(x => x.Names)
                        .FirstOrDefault() ?? x.Name)
                    .ToList();

                string header = string.Join(
                    config.Delimiter,
                    propertyNames
                        .Select(x => "\"" + x.Replace("\"", "\"\"") + "\""));
                
                config.HasHeaderRecord = true;
                text = header + config.NewLineString + text;
            }

            using StringReader reader = new StringReader(text);
            using CsvReader csvReader = new CsvReader(reader, config);
            return isStringArray
                ? GetStringRows().ToList()
                : isDynamic
                    ? config.HasHeaderRecord
                        ? csvReader.GetRecords<dynamic>().ToList()
                        : GetDynamicRows().ToList()
                    : csvReader.GetRecords<T>().ToList();

            IEnumerable<string[]> GetStringRows()
            {
                if (config.HasHeaderRecord)
                {
                    csvReader.Read();
                }

                while (csvReader.Read())
                {
                    yield return csvReader.Context.Record;
                }
            }

            IEnumerable<dynamic> GetDynamicRows()
            {
                while (csvReader.Read())
                {
                    IDictionary<string, object?> obj = new ExpandoObject();
                    for (int i = 0; i < csvReader.Context.Record.Length; i++)
                    {
                        obj[ExcelIdentifierUtils.GetExcelColumnName(i + 1)] = csvReader.Context.Record[i];
                    }
                    yield return obj;
                }
            }
        }

        public string Serialize(object? value, CsvConfiguration? settings)
        {
            if (value == null)
            {
                return string.Empty;
            }

            Type? enumerableType = GetGenericTypeUsage(value.GetType(), typeof(IEnumerable<>));
            IEnumerable<object> values = enumerableType != null
                ? (IEnumerable<object>)value!
                : new object[] { value! };

            using StringWriter textWriter = new StringWriter();
            using CsvWriter csvWriter = new CsvWriter(textWriter, settings ?? GetDefaultSettings());
            csvWriter.WriteRecords(values);

            return textWriter.ToString();
        }

        //https://stackoverflow.com/questions/5461295/using-isassignablefrom-with-open-generic-types
        private static Type? GetGenericTypeUsage(Type givenType, Type genericType)
        {
            Type[] interfaceTypes = givenType.GetInterfaces();

            foreach (Type it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    return it;
                }
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return givenType;
            }

            Type? baseType = givenType.BaseType;
            if (baseType == null)
            {
                return null;
            }

            return GetGenericTypeUsage(baseType, genericType);
        }

        public void Configure(Command command)
        {
            command.Add(new Option<string>(new[] { "-s", "--separator" }, "Value separator"));
            command.Add(new Option<string>(new[] { "-t", "--type" }, "The type that will be use to deserialize the input text"));
        }

        public CsvConfiguration BindParameters(CsvConfiguration settings, CsvConverterInputParameters args)
        {
            if (!string.IsNullOrEmpty(args.Separator))
            {
                settings.Delimiter = Regex.Unescape(args.Separator);
            }
            return settings;
        }

        public CsvConfiguration BindParameters(CsvConfiguration settings, CsvConverterOutputParameters args)
        {
            if (!string.IsNullOrEmpty(args.Separator))
            {
                settings.Delimiter = Regex.Unescape(args.Separator);
            }
            return settings;
        }

        public async Task HandleCommandAsync(IConverterContext<CsvConfiguration> context, CsvConverterInputParameters args)
        {
            if (args.Type == "dynamic" || args.Type == "string[]")
            {
                string text = await context.GetTextAsync();
                object? result = args.Type == "dynamic"
                    ? Deserialize<ExpandoObject>(text, context.Settings)
                    : Deserialize<string[]>(text, context.Settings);

                await context.SubmitCodeAsync($"List<{args.Type}> {context.VariableName} = null;");
                context.CSharpKernel.ScriptState.GetVariable(context.VariableName).Value = result;
                return;
            }

            string sourceVariableName = await context.CreatePrivateVariableAsync(await context.GetTextAsync(), typeof(string));
            string converterVariableName = await context.CreatePrivateVariableAsync(this, typeof(CsvConverter));
            string settingsVariableName = await context.CreatePrivateVariableAsync(context.Settings, typeof(CsvConfiguration));

            if (string.IsNullOrEmpty(args.Type))
            {
                string csv = await context.GetTextAsync();
                SchemaSerialization? schema = _typeProvider.ProvideTypes(new CsvSource(context.VariableName, csv, context.Settings!));

                string code = $@"{schema.Code}

{schema.RootType} {context.VariableName} = ({schema.RootType}){converterVariableName}.Deserialize<{schema.ElementType}>({sourceVariableName}, {settingsVariableName});";

                await context.SubmitCodeAsync(code);
            }
            else
            {
                string code = $"List<{args.Type}> {context.VariableName} = (List<{args.Type}>){converterVariableName}.Deserialize<{args.Type}>({sourceVariableName}, {settingsVariableName});";
                await context.SubmitCodeAsync(code);
            }
        }
    }
}
