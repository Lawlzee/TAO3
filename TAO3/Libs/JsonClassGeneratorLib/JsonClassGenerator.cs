// Copyright © 2010 Xamasoft

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Globalization;
using Xamasoft.JsonClassGenerator.CodeWriters;
using TAO3.Internal.CodeGeneration;

namespace Xamasoft.JsonClassGenerator
{
    internal static class JsonClassGenerator
    {
        internal static string GenerateClasses(string jsonText, string mainClassName)
        {
            JObject[] examples;
            using (StringReader sr = new StringReader(jsonText))
            {
                using (JsonTextReader reader = new JsonTextReader(sr))
                {
                    JToken? json = JToken.ReadFrom(reader);
                    if (json is JArray)
                    {
                        examples = ((JArray)json).Cast<JObject>().ToArray();
                    }
                    else if (json is JObject)
                    {
                        examples = new[] { (JObject)json };
                    }
                    else
                    {
                        throw new Exception("Sample JSON must be either a JSON array, or a JSON object.");
                    }
                }
            }


            HashSet<string> usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                mainClassName
            };

            JsonType rootType = new(examples[0])
            {
                IsRoot = true
            };
            rootType.AssignName(mainClassName);

            
            List<JsonType> types = GenerateClass(examples, rootType, usedNames);

            return WriteClasses(types);
        }

        internal static string WriteClasses(List<JsonType> types)
        {
            CSharpCodeWriter codeWriter = new CSharpCodeWriter();
            using (StringWriter stringWriter = new StringWriter())
            {
                codeWriter.WriteFileStart(stringWriter);
                foreach (JsonType? type in types)
                {
                    codeWriter.WriteClass(stringWriter, type);
                }

                codeWriter.WriteFileEnd(stringWriter);

                return stringWriter.ToString();
            }
        }

        private static List<JsonType> GenerateClass(JObject[] examples, JsonType type, HashSet<string> usedNames)
        {
            List<JsonType> types = new List<JsonType>();

            Dictionary<string, JsonType>? jsonFields = new Dictionary<string, JsonType>();
            Dictionary<string, IList<object>>? fieldExamples = new Dictionary<string, IList<object>>();

            bool first = true;

            foreach (JObject obj in examples)
            {
                foreach (JProperty prop in obj.Properties())
                {
                    JsonType fieldType;
                    JsonType? currentType = new JsonType(prop.Value);
                    string? propName = prop.Name;
                    if (jsonFields.TryGetValue(propName, out fieldType))
                    {
                        JsonType? commonType = fieldType.GetCommonType(currentType);
                        jsonFields[propName] = commonType;
                    }
                    else
                    {
                        JsonType? commonType = currentType;
                        if (!first)
                        {
                            commonType = commonType.GetCommonType(JsonType.GetNull());
                        }

                        jsonFields.Add(propName, commonType);
                        fieldExamples[propName] = new List<object>();
                    }
                    IList<object>? fe = fieldExamples[propName];
                    JToken? val = prop.Value;
                    if (val.Type == JTokenType.Null || val.Type == JTokenType.Undefined)
                    {
                        if (!fe.Contains(null))
                        {
                            fe.Insert(0, null);
                        }
                    }
                    else
                    {
                        object? v = val.Type == JTokenType.Array || val.Type == JTokenType.Object ? val : val.Value<object>();
                        if (!fe.Any(x => v.Equals(x)))
                        {
                            fe.Add(v);
                        }
                    }
                }
                first = false;
            }

            foreach (KeyValuePair<string, JsonType> field in jsonFields)
            {
                JsonType? fieldType = field.Value;
                if (fieldType.Type == JsonTypeEnum.Object)
                {
                    List<JObject>? subexamples = new List<JObject>(examples.Length);
                    foreach (JObject? obj in examples)
                    {
                        JToken value;
                        if (obj.TryGetValue(field.Key, out value))
                        {
                            if (value.Type == JTokenType.Object)
                            {
                                subexamples.Add((JObject)value);
                            }
                        }
                    }

                    fieldType.AssignName(CreateUniqueClassName(field.Key, usedNames));
                    types.AddRange(GenerateClass(subexamples.ToArray(), fieldType, usedNames));
                }

                if (fieldType.InternalType != null && fieldType.InternalType.Type == JsonTypeEnum.Object)
                {
                    List<JObject>? subexamples = new List<JObject>(examples.Length);
                    foreach (JObject? obj in examples)
                    {
                        JToken value;
                        if (obj.TryGetValue(field.Key, out value))
                        {
                            if (value.Type == JTokenType.Array)
                            {
                                foreach (JToken? item in (JArray)value)
                                {
                                    if (!(item is JObject))
                                    {
                                        throw new NotSupportedException("Arrays of non-objects are not supported yet.");
                                    }

                                    subexamples.Add((JObject)item);
                                }

                            }
                            else if (value.Type == JTokenType.Object)
                            {
                                foreach (KeyValuePair<string, JToken> item in (JObject)value)
                                {
                                    if (!(item.Value is JObject))
                                    {
                                        throw new NotSupportedException("Arrays of non-objects are not supported yet.");
                                    }

                                    subexamples.Add((JObject)item.Value);
                                }
                            }
                        }
                    }


                    field.Value.InternalType.AssignName(CreateUniqueClassName(field.Key, usedNames));
                    types.AddRange(GenerateClass(subexamples.ToArray(), field.Value.InternalType, usedNames));
                }
            }

            type.Fields = jsonFields.Select(x => new FieldInfo(x.Key, x.Value)).ToArray();

            types.Add(type);
            return types;
        }

        private static string CreateUniqueClassName(string name, HashSet<string> usedNames)
        {
            name = IdentifierUtils.ToPascalCase(name);

            string? finalName = name;
            int i = 2;
            while (usedNames.Contains(finalName))
            {
                finalName = name + i.ToString();
                i++;
            }

            usedNames.Add(finalName);
            return finalName;
        }
    }
}
