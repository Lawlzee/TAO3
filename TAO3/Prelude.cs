using CsvHelper.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAO3.Converters;
using JsonConverter = TAO3.Converters.JsonConverter;

namespace TAO3
{
    public static class Prelude
    {
        public static string ToJson(object? value, JsonSerializerSettings? settings = null) => new JsonConverter().Serialize(value, settings);
        public static string ToXml(object? value, XmlWriterSettings? settings = null) => new XmlConverter().Serialize(value, settings);
        public static string ToCsv(object? value, CsvConfiguration? settings = null) => new CsvConverter(hasHeader: false).Serialize(value, settings);
        public static string ToCsvh(object? value, CsvConfiguration? settings = null) => new CsvConverter(hasHeader: true).Serialize(value, settings);
        public static string ToLine(object? value) => new LineConverter().Serialize(value, settings: null);

        public static T FromJson<T>(string text, JsonSerializerSettings? settings = null) => (T)new JsonConverter().Deserialize<T>(text, settings)!;
        public static T FromXml<T>(string text, XmlWriterSettings? settings = null) => (T)new XmlConverter().Deserialize<T>(text, settings)!;
        public static T[] FromCsv<T>(string text, CsvConfiguration? settings = null) => (T[])new CsvConverter(hasHeader: false).Deserialize<T>(text, settings)!;
        public static T[] FromCsvh<T>(string text, CsvConfiguration? settings = null) => (T[])new CsvConverter(hasHeader: true).Deserialize<T>(text, settings)!;

        public static dynamic FromJson(string text, JsonSerializerSettings? settings = null) => new JsonConverter().Deserialize<ExpandoObject>(text, settings)!;
        public static dynamic FromXml(string text, XmlWriterSettings? settings = null) => new XmlConverter().Deserialize<ExpandoObject>(text, settings)!;
        public static string[] FromCsv(string text, CsvConfiguration? settings = null) => (string[])new CsvConverter(hasHeader: false).Deserialize<ExpandoObject>(text, settings)!;
        public static string[] FromCsvh(string text, CsvConfiguration? settings = null) => (string[])new CsvConverter(hasHeader: true).Deserialize<ExpandoObject>(text, settings)!;
        public static string[] FromLine(string text) => (string[])new LineConverter().Deserialize<string>(text, settings: null)!;
    }
}
