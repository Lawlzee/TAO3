﻿using CsvHelper.Configuration;
using Microsoft.DotNet.Interactive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TAO3.Clipboard;
using TAO3.Converters;
using TAO3.Excel;
using TAO3.InputSources;
using TAO3.Keyboard;
using TAO3.Notepad;
using TAO3.OutputDestinations;
using TAO3.Services;
using TAO3.Toast;
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
