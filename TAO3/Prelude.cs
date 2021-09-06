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
using TAO3.IO;
using TAO3.Keyboard;
using TAO3.Notepad;
using TAO3.Services;
using TAO3.Toast;
using TAO3.Translation;
using TAO3.TypeProvider;
using TAO3.Formatting;
using TAO3.VsCode;
using TAO3.Avalonia;

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
        public static IConverterService Converter => Services.Converter;
        public static ISourceService SourceService => Services.SourceService;
        public static IDestinationService OutputDestination => Services.DestinationService;
        public static ICellService Cells => Services.Cells;
        public static HttpClient HttpClient => Services.HttpClient;
        public static ITranslationService Translation => Services.Translation;
        public static TAO3Converters BuiltInConverters => Services.BuiltInConverters;
        public static ITypeProviders TypeProviders => Services.TypeProviders;
        public static TAO3Formatters Formatters => Services.Formatters;
        public static IVsCodeService VsCode => Services.VsCode;
        public static IAvaloniaService AvaloniaService => Services.Avalonia;

        public static string ToJson(object? value, JsonSerializerSettings? settings = null) => BuiltInConverters.Json.Serialize(value, settings);
        public static string ToXml(object? value, XmlWriterSettings? settings = null) => BuiltInConverters.Xml.Serialize(value, settings);
        public static string ToCsv(object? value, CsvConfiguration? settings = null) => BuiltInConverters.Csv.Serialize(value, settings);
        public static string ToCsvh(object? value, CsvConfiguration? settings = null) => BuiltInConverters.Csvh.Serialize(value, settings);
        public static string ToLine(object? value) => BuiltInConverters.Line.Serialize(value);
        public static string ToHtml(object? value) => BuiltInConverters.Html.Serialize(value);
        public static string ToCSharp(object? value) => BuiltInConverters.CSharp.Serialize(value);
        public static string ToSql(object? value) => BuiltInConverters.Sql.Serialize(value);

        public static T FromJson<T>(string text, JsonSerializerSettings? settings = null) => BuiltInConverters.Json.Deserialize<T>(text, settings);
        public static T FromXml<T>(string text, XmlWriterSettings? settings = null) => BuiltInConverters.Xml.Deserialize<T>(text, settings);
        public static List<T> FromCsv<T>(string text, CsvConfiguration? settings = null) => BuiltInConverters.Csv.Deserialize<T>(text, settings);
        public static List<T> FromCsvh<T>(string text, CsvConfiguration? settings = null) => BuiltInConverters.Csvh.Deserialize<T>(text, settings);
        public static List<T> FromSql<T>(string text)
            where T : new()
        {
            return BuiltInConverters.Sql.Deserialize<T>(text);
        }

        public static dynamic FromJson(string text, JsonSerializerSettings? settings = null) => BuiltInConverters.Json.Deserialize<ExpandoObject>(text, settings);
        public static dynamic FromXml(string text, XmlWriterSettings? settings = null) => BuiltInConverters.Xml.Deserialize<ExpandoObject>(text, settings);
        public static List<dynamic> FromCsv(string text, CsvConfiguration? settings = null) => BuiltInConverters.Csv.Deserialize<dynamic>(text, settings);
        public static List<dynamic> FromCsvh(string text, CsvConfiguration? settings = null) => BuiltInConverters.Csvh.Deserialize<dynamic>(text, settings);
        public static List<string> FromLine(string text) => BuiltInConverters.Line.Deserialize(text);
        public static HtmlString FromHtml(string text) => BuiltInConverters.Html.Deserialize(text);
        public static CSharpCompilationUnit FromCSharp(string text) => BuiltInConverters.CSharp.Deserialize(text);

        public static List<string[]> FromCsvArray(string text, CsvConfiguration? settings = null) => BuiltInConverters.Csv.Deserialize<string[]>(text, settings);
        public static List<string[]> FromCsvhArray(string text, CsvConfiguration? settings = null) => BuiltInConverters.Csvh.Deserialize<string[]>(text, settings);

        public static DisplayedValue DisplayAsJson(string json, bool format = true) => (format ? FormatJson(json) : json).DisplayAs("application/json");
        public static DisplayedValue DisplayAsHtml(string html) => html.DisplayAs("text/html");
        public static DisplayedValue DisplayAsText(string text) => text.DisplayAs("text/plain");
        public static DisplayedValue DisplayAsMarkdown(string markdown) => markdown.DisplayAs("text/markdown");
        public static DisplayedValue DisplayAsCSharp(string csharp, bool format = true) => (format ? FormatCSharp(csharp) : csharp).DisplayAs("text/x-csharp");
        public static DisplayedValue DisplayAsJavascript(string javascript) => javascript.DisplayAs("text/x-javascript");
        public static DisplayedValue DisplayAsFSharp(string fsharp) => fsharp.DisplayAs("text/x-fsharp");
        public static DisplayedValue DisplayAsSql(string sql, bool format = true) => (format ? FormatSql(sql) : sql).DisplayAs("text/x-sql");
        public static DisplayedValue DisplayAsPowershell(string powershell) => powershell.DisplayAs("text/x-powershell");
        public static DisplayedValue DisplayAsCss(string css) => css.DisplayAs("text/x-css");

        public static DisplayedValue DisplayAsXml(string xml, bool format = true) => (format ? FormatXml(xml) : xml).DisplayAs("text/plain");
        public static DisplayedValue DisplayAsCsv(string csv) => csv.DisplayAs("text/plain");
        public static DisplayedValue DisplayAsCsvh(string csvh) => csvh.DisplayAs("text/plain");
        public static DisplayedValue DisplayAsLine(string line) => line.DisplayAs("text/plain");

        public static string FormatCSharp(string csharp) => Formatters.CSharp.Format(csharp);
        public static string FormatJson(string json) => Formatters.Json.Format(json);
        public static string FormatSql(string sql) => Formatters.Sql.Format(sql);
        public static string FormatXml(string xml) => Formatters.Xml.Format(xml);

        public static DisplayedValue DisplayToJson(object? value, bool format = true, JsonSerializerSettings? settings = null) => DisplayAsJson(ToJson(value, settings), format);
        public static DisplayedValue DisplayToXml(object? value, bool format = true, XmlWriterSettings? settings = null) => DisplayAsXml(ToXml(value, settings), format);
        public static DisplayedValue DisplayToCsv(object? value, CsvConfiguration? settings = null) => DisplayAsCsv(ToCsv(value, settings));
        public static DisplayedValue DisplayToCsvh(object? value, CsvConfiguration? settings = null) => DisplayAsCsvh(ToCsvh(value, settings));
        public static DisplayedValue DisplayToLine(object? value) => DisplayAsCsvh(ToLine(value));
        public static DisplayedValue DisplayToHtml(object? value) => DisplayAsHtml(ToHtml(value));
        public static DisplayedValue DisplayToCSharp(object? value, bool format = true) => DisplayAsCSharp(ToCSharp(value), format);
        public static DisplayedValue DisplayToSql(object? value, bool format = true) => DisplayAsSql(ToSql(value), format);

        public static void ConfigureTranslator(string url, string? apiKey = null)
        {
            Translation.Configure(url, apiKey);
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
