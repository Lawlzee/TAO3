using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
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
using System.Numerics;
using System.Reactive;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TAO3.Internal.CodeGeneration;
using TAO3.Internal.Types;
using TAO3.TypeProvider;

namespace TAO3.Converters.Csv
{
    public record CsvConverterInputParameters
    {
        public string? Separator { get; init; }
        public string? Type { get; init; }
    }

    public record CsvConverterOutputParameters
    {
        public string? Separator { get; init; }
    }

    public class CsvConverter :
        IConverterTypeProvider<CsvConfiguration, CsvConverterInputParameters>,  
        IOutputConfigurableConverter<CsvConfiguration, CsvConverterOutputParameters>

    {
        private readonly ITypeProvider<CsvSource> _typeProvider;
        private readonly bool _hasHeader;

        public string Format => _hasHeader ? "csvh" : "csv";
        public string MimeType => "text/csv";
        public IReadOnlyList<string> Aliases => Array.Empty<string>();
        public IReadOnlyList<string> FileExtensions => new[] { ".csv" };
        public Dictionary<string, object> Properties { get; }
        public IDomCompiler DomCompiler => _typeProvider;

        public CsvConverter(ITypeProvider<CsvSource> typeProvider, bool hasHeader)
        {
            _typeProvider = typeProvider;
            _hasHeader = hasHeader;
            Properties = new Dictionary<string, object>();
        }

        public CsvConfiguration GetDefaultSettings()
        {
            CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = _hasHeader
            };

            config.TypeConverterOptionsCache.AddOptions<BigInteger>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<bool>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<byte>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<byte[]>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<char>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<DateTime>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<DateTimeOffset>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<decimal>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<double>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<float>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<Guid>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<short>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<int>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<long>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<sbyte>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<string>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<TimeSpan>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<Type>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<ushort>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<uint>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<ulong>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<Uri>(CreateConverter());

            config.TypeConverterOptionsCache.AddOptions<BigInteger?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<bool?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<byte?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<char?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<DateTime?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<DateTimeOffset?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<decimal?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<double?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<float?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<Guid?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<short?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<int?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<long?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<sbyte?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<string?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<TimeSpan?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<ushort?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<uint?>(CreateConverter());
            config.TypeConverterOptionsCache.AddOptions<ulong?>(CreateConverter());

            return config;

            TypeConverterOptions CreateConverter()
            {
                TypeConverterOptions options = new TypeConverterOptions();
                options.NullValues.AddRange(new List<string> { "NULL", "null", "" });
                return options;
            }
        }

        T IConverterTypeProvider<CsvConfiguration, CsvConverterInputParameters>.Deserialize<T>(string text, CsvConfiguration? settings)
        {
            return (T)TypeInferer.Invoke<object>(
                typeof(T),
                typeof(List<>),
                () => Deserialize<Unit>(text, settings));
        }

        public List<T> Deserialize<T>(string text, CsvConfiguration? settings)
        {
            bool isDynamic = typeof(T) == typeof(object);
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

            if (isStringArray)
            {
                return (List<T>)(object)GetStringRows().ToList();
            }

            if (isDynamic)
            {
                if (config.HasHeaderRecord)
                {
                    return (List<T>)(object)csvReader.GetRecords<dynamic>().ToList();
                }

                return (List<T>)(object)GetDynamicRows().ToList();
            }

            return csvReader.GetRecords<T>().ToList();

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

        public async Task<IDomType> ProvideTypeAsync(IConverterContext<CsvConfiguration> context, CsvConverterInputParameters args)
        {
            if (args.Type != null)
            {
                return new DomCollection(new List<IDomType>
                {
                    new DomClassReference(args.Type)
                });
            }

            string csv = await context.GetTextAsync();
            return _typeProvider.DomParser.Parse(new CsvSource(context.VariableName, csv, context.Settings!));
        }
    }
}
