using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.DotNet.Interactive;
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
using TAO3.TypeProvider;

namespace TAO3.Converters.Csv
{
    public class CsvConverterParameters : ConverterCommandParameters
    {
        public string? Separator { get; set; }
        public string? Type { get; set; }
    }

    public class CsvConverter : 
        IConverter<CsvConfiguration>,  
        IHandleCommand<CsvConfiguration, CsvConverterParameters>
    {
        private readonly CsvConfiguration _defaultSettings;

        private readonly ITypeProvider<CsvSource> _typeProvider;
        private readonly bool _hasHeader;

        public string Format => _hasHeader ? "csvh" : "csv";

        public string DefaultType => "List<string[]>";

        public CsvConverter(ITypeProvider<CsvSource> typeProvider, bool hasHeader)
        {
            _typeProvider = typeProvider;
            _hasHeader = hasHeader;
            _defaultSettings = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = _hasHeader
            };
        }

        public object? Deserialize<T>(string text, CsvConfiguration? settings)
        {
            bool isDynamic = typeof(T) == typeof(ExpandoObject);

            using StringReader reader = new StringReader(text);
            using CsvReader csvReader = new CsvReader(reader, settings ?? _defaultSettings);
            return isDynamic
                ? GetRows(csvReader).ToList()
                : csvReader.GetRecords<T>().ToList();
        }

        private IEnumerable<string[]> GetRows(CsvReader csvReader)
        {
            while (csvReader.Read())
            {
                yield return csvReader.Context.Record;
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
            using CsvWriter csvWriter = new CsvWriter(textWriter, settings ?? _defaultSettings);
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
            command.Add(new Option(new[] { "-t", "--type" }, "The type that will be use to deserialize the input text"));
        }

        public async Task HandleCommandAsync(IConverterContext<CsvConfiguration> context, CsvConverterParameters args)
        {
            context.Settings ??= _defaultSettings;

            if (!string.IsNullOrEmpty(args.Separator))
            {
                context.Settings.Delimiter = Regex.Unescape(args.Separator);
            }

            if (args.Type == "dynamic")
            {
                await context.DefaultHandleCommandAsync();
                return;
            }

            string sourceVariableName = await context.CreatePrivateVariableAsync(await context.GetTextAsync(), typeof(string));
            string converterVariableName = await context.CreatePrivateVariableAsync(context.Converter, typeof(CsvConverter));
            string settingsVariableName = await context.CreatePrivateVariableAsync(context.Settings, typeof(CsvConfiguration));

            if (string.IsNullOrEmpty(args.Type))
            {
                string csv = await context.GetTextAsync();
                SchemaSerialization? schema = _typeProvider.ProvideTypes(new CsvSource(args.Name!, csv, context.Settings));

                string code = $@"{schema.Code}

{schema.RootType} {context.VariableName} = ({schema.RootType}){converterVariableName}.Deserialize<{schema.ElementType}>({sourceVariableName}, {settingsVariableName});";

                await context.SubmitCodeAsync(code);
            }
            else
            {
                string code = $"List<{args.Type}> {args.Name} = (List<{args.Type}>){converterVariableName}.Deserialize<{args.Type}>({sourceVariableName}, {settingsVariableName});";
                await context.SubmitCodeAsync(code);
            }
        }
    }
}
