using CsvHelper.Configuration;
using Microsoft.AspNetCore.Html;
using Microsoft.DotNet.Interactive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAO3.Cell;
using TAO3.Clipboard;
using TAO3.Converters;
using TAO3.Converters.CSharp;
using TAO3.Excel;
using TAO3.InputSources;
using TAO3.Keyboard;
using TAO3.Notepad;
using TAO3.OutputDestinations;
using TAO3.Services;
using TAO3.Toast;
using TAO3.Translation;

namespace TAO3
{
    public static class Prelude
    {
        public static Kernel Kernel { get; internal set; } = null!;
        public static TAO3Services Services { get; internal set; } = null!;
        public static IExcelService Excel => Services.Excel;
        public static INotepadService Notepad => Services.Notepad;
        public static IKeyboardService Keyboard => Services.Keyboard;
        public static IClipboardService Clipboard => Services.Clipboard;
        public static IToastService Toast => Services.Toast;
        public static IFormatConverterService FormatConverter => Services.FormatConverter;
        public static IInputSourceService InputSource => Services.InputSource;
        public static IOutputDestinationService OutputDestination => Services.OutputDestination;
        public static ICellService Cells => Services.Cells;
        public static HttpClient HttpClient => Services.HttpClient;
        public static ITranslationService Translation => Services.Translation;
        public static TAO3Converters Converters => Services.Converters;

        public static string ToJson(object? value, JsonSerializerSettings? settings = null) => Converters.Json.Serialize(value, settings);
        public static string ToXml(object? value, XmlWriterSettings? settings = null) => Converters.Xml.Serialize(value, settings);
        public static string ToCsv(object? value, CsvConfiguration? settings = null) => Converters.Csv.Serialize(value, settings);
        public static string ToCsvh(object? value, CsvConfiguration? settings = null) => Converters.Csvh.Serialize(value, settings);
        public static string ToLine(object? value) => Converters.Line.Serialize(value);
        public static string ToHmtl(object? value) => Converters.Html.Serialize(value);
        public static string ToCSharp(object? value) => Converters.CSharp.Serialize(value);
        public static string ToSql(object? value) => Converters.Sql.Serialize(value);

        public static T FromJson<T>(string text, JsonSerializerSettings? settings = null) => (T)Converters.Json.Deserialize<T>(text, settings)!;
        public static T FromXml<T>(string text, XmlWriterSettings? settings = null) => (T)Converters.Xml.Deserialize<T>(text, settings)!;
        public static T[] FromCsv<T>(string text, CsvConfiguration? settings = null) => (T[])Converters.Csv.Deserialize<T>(text, settings)!;
        public static T[] FromCsvh<T>(string text, CsvConfiguration? settings = null) => (T[])Converters.Csvh.Deserialize<T>(text, settings)!;

        public static dynamic FromJson(string text, JsonSerializerSettings? settings = null) => Converters.Json.Deserialize<ExpandoObject>(text, settings)!;
        public static dynamic FromXml(string text, XmlWriterSettings? settings = null) => Converters.Xml.Deserialize<ExpandoObject>(text, settings)!;
        public static string[] FromCsv(string text, CsvConfiguration? settings = null) => (string[])Converters.Csv.Deserialize<ExpandoObject>(text, settings)!;
        public static string[] FromCsvh(string text, CsvConfiguration? settings = null) => (string[])Converters.Csvh.Deserialize<ExpandoObject>(text, settings)!;
        public static string[] FromLine(string text) => (string[])Converters.Line.Deserialize<string>(text)!;
        public static HtmlString FromHmtl(string text) => (HtmlString)Converters.Html.Deserialize<object>(text)!;
        public static CSharpCompilationUnit FromCSharp(string text) => (CSharpCompilationUnit)Converters.CSharp.Deserialize<CSharpCompilationUnit>(text)!;

        public static void ConfigureTranslator(string url, string? apiKey = null)
        {
            Translation.Configure(url, apiKey);
        }

        public static Translator CreateTranslator(string sourceLanguage, string targetLanguage)
        {
            return Translation.CreateTranslator(sourceLanguage, targetLanguage);
        }

        public static Translator CreateTranslator(Language sourceLanguage, Language targetLanguage)
        {
            return Translation.CreateTranslator(sourceLanguage, targetLanguage);
        }

        public static Task<string?> TranslateAsync(string sourceLanguage, string targetLanguage, string text)
        {
            return Translation.TranslateAsync(sourceLanguage, targetLanguage, text);
        }

        public static Task<string?> TranslateAsync(Language sourceLanguage, Language targetLanguage, string text)
        {
            return Translation.TranslateAsync(sourceLanguage, targetLanguage, text);
        }

        public static Task<string?[]> TranslateAsync(string sourceLanguage, string targetLanguage, params string[] texts)
        {
            return Translation.TranslateAsync(sourceLanguage, targetLanguage, texts);
        }

        public static Task<string?[]> TranslateAsync(Language sourceLanguage, Language targetLanguage, params string[] texts)
        {
            return Translation.TranslateAsync(sourceLanguage, targetLanguage, texts);
        }
    }
}
