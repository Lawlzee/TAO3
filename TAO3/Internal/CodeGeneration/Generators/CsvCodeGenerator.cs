using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters;
using Xamasoft.JsonClassGenerator;

namespace TAO3.Internal.CodeGeneration.Generators
{
    internal class CsvCodeGenerator
    {
        public async Task<string> GenerateSourceCodeAsync(IConverterContext<CsvConfiguration> context)
        {
            ClassInferer inferedClass = InferClass(await context.GetTextAsync(), context.Settings!);
            string className = IdentifierUtils.ToCSharpIdentifier(context.VariableName);
            JsonType typeDefinition = inferedClass.CreateJsonType(className);

            string classDefinition = JsonClassGenerator.WriteClasses(new List<JsonType> { typeDefinition });

            string clipboardVariableName = await context.CreatePrivateVariableAsync(await context.GetTextAsync(), typeof(string));
            string converterVariableName = await context.CreatePrivateVariableAsync(context.Converter, typeof(CsvConverter));
            string settingsVariableName = await context.CreatePrivateVariableAsync(context.Settings, typeof(CsvConfiguration));

            string code = $@"using System.Globalization;
using System.IO;
using System.Linq;
{classDefinition}{className}[] {context.VariableName} = ({className}[]){converterVariableName}.Deserialize<{className}>({clipboardVariableName}, {settingsVariableName});";

            return code;
        }

        private ClassInferer InferClass(string text, CsvConfiguration settings)
        {
            bool hasHeader = settings.HasHeaderRecord;
            try
            {
                settings.HasHeaderRecord = false;

                using StringReader reader = new StringReader(text);
                using CsvReader csvReader = new CsvReader(reader, settings);

                List<string[]> rows = new List<string[]>();

                ClassInferer? inferer = null;

                while (csvReader.Read())
                {
                    string[] row = csvReader.Context.Record;

                    if (inferer == null)
                    {
                        if (hasHeader)
                        {
                            inferer = new ClassInferer(csvReader.Context.Record);
                            continue;
                        }

                        string[] anonymousHeaders = Enumerable.Range(1, row.Length)
                            .Select(ExcelIdentifierUtils.GetExcelColumnName)
                            .ToArray();

                        inferer = new ClassInferer(anonymousHeaders);
                    }

                    bool done = inferer.UpdateClassInferance(row);
                    if (done)
                    {
                        break;
                    }
                }

                return inferer ?? new ClassInferer(Array.Empty<string>());
            }
            finally
            {
                settings.HasHeaderRecord = hasHeader;
            }
            
        }

        private class ClassInferer
        {
            private readonly TypeInferer[] _typeInferers;

            public ClassInferer(string[] headers)
            {
                _typeInferers = headers
                    .Select(x => new TypeInferer(x))
                    .ToArray();
            }

            public bool UpdateClassInferance(string[] row)
            {
                bool done = true;
                for (int i = 0; i < _typeInferers.Length; i++)
                {
                    if (!_typeInferers[i].UpdateLegalTypeConvertions(row[i]))
                    {
                        done = false;
                    }
                }

                return done;
            }

            public JsonType CreateJsonType(string className)
            {
                return new JsonType(
                    JsonTypeEnum.Object,
                    internalType: null,
                    assignedName: className,
                    _typeInferers
                        .Select(x => new FieldInfo(x.Header, new JsonType(x.GetJsonType())))
                        .ToList(),
                    isRoot: true);
            }
        }

        private class TypeInferer
        {
            private readonly List<TypeConvertion> _legalTypeConvertions;
            private bool _isNullable;
            public bool IsDone => _legalTypeConvertions.Count == 0;
            public string Header { get; }

            public TypeInferer(string header)
            {
                Header = header;
                _legalTypeConvertions = GetAllTypeConveters();
            }

            public JsonTypeEnum GetJsonType()
            {
                if (IsDone)
                {
                    return JsonTypeEnum.String;
                }

                if (_isNullable)
                {
                    return JsonTypeEnum.Nullable | _legalTypeConvertions[0].Type;
                }

                return _legalTypeConvertions[0].Type;
            }

            public bool UpdateLegalTypeConvertions(string value)
            {
                if (IsDone)
                {
                    return true;
                }

                if (string.IsNullOrEmpty(value) || value.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    _isNullable = true;
                    return false;
                }

                _legalTypeConvertions.RemoveAll(converter => !converter.isCompatible(value));
                return false;
            }

            private List<TypeConvertion> GetAllTypeConveters()
            {
                return new List<TypeConvertion>
                {
                    new TypeConvertion(JsonTypeEnum.Integer, str => int.TryParse(str, out _)),
                    new TypeConvertion(JsonTypeEnum.Long, str => long.TryParse(str, out _)),
                    new TypeConvertion(JsonTypeEnum.Float, str => double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out _)),
                    new TypeConvertion(JsonTypeEnum.Boolean, str => bool.TryParse(str, out _)),
                    new TypeConvertion(JsonTypeEnum.Date, str => DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                };
            }
        }

        private record TypeConvertion(
            JsonTypeEnum Type,
            Func<string, bool> isCompatible);
    }
}
