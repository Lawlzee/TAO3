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
using TAO3.TextSerializer.CSharp;
using TAO3.Toast;
using TAO3.Translation;
using JsonConverter = TAO3.Converters.JsonConverter;

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
        public static ICSharpObjectSerializer CSharpGenerator => Services.CSharpSerializer;
        public static HttpClient HttpClient => Services.HttpClient;
        public static ITranslationService Translation => Services.Translation;

        public static string ToJson(object? value, JsonSerializerSettings? settings = null) => new JsonConverter().Serialize(value, settings);
        public static string ToXml(object? value, XmlWriterSettings? settings = null) => new XmlConverter().Serialize(value, settings);
        public static string ToCsv(object? value, CsvConfiguration? settings = null) => new CsvConverter(hasHeader: false).Serialize(value, settings);
        public static string ToCsvh(object? value, CsvConfiguration? settings = null) => new CsvConverter(hasHeader: true).Serialize(value, settings);
        public static string ToLine(object? value) => new LineConverter().Serialize(value, settings: null);
        public static string ToHmtl(object? value) => new HtmlConverter().Serialize(value, settings: null);
        public static string ToCSharp(object? value) => new CSharpConverter(CSharpGenerator).Serialize(value, settings: null);

        public static T FromJson<T>(string text, JsonSerializerSettings? settings = null) => (T)new JsonConverter().Deserialize<T>(text, settings)!;
        public static T FromXml<T>(string text, XmlWriterSettings? settings = null) => (T)new XmlConverter().Deserialize<T>(text, settings)!;
        public static T[] FromCsv<T>(string text, CsvConfiguration? settings = null) => (T[])new CsvConverter(hasHeader: false).Deserialize<T>(text, settings)!;
        public static T[] FromCsvh<T>(string text, CsvConfiguration? settings = null) => (T[])new CsvConverter(hasHeader: true).Deserialize<T>(text, settings)!;

        public static dynamic FromJson(string text, JsonSerializerSettings? settings = null) => new JsonConverter().Deserialize<ExpandoObject>(text, settings)!;
        public static dynamic FromXml(string text, XmlWriterSettings? settings = null) => new XmlConverter().Deserialize<ExpandoObject>(text, settings)!;
        public static string[] FromCsv(string text, CsvConfiguration? settings = null) => (string[])new CsvConverter(hasHeader: false).Deserialize<ExpandoObject>(text, settings)!;
        public static string[] FromCsvh(string text, CsvConfiguration? settings = null) => (string[])new CsvConverter(hasHeader: true).Deserialize<ExpandoObject>(text, settings)!;
        public static string[] FromLine(string text) => (string[])new LineConverter().Deserialize<string>(text, settings: null)!;
        public static HtmlString FromHmtl(string text) => (HtmlString)new HtmlConverter().Deserialize<object>(text, settings: null)!;
        public static CSharpCompilationUnit FromCSharp(string text) => (CSharpCompilationUnit)new CSharpConverter(CSharpGenerator).Deserialize<CSharpCompilationUnit>(text, settings: null)!;

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
    }
}
