﻿using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters
{
    public interface IConverter
    {
        string Format { get; }
        IReadOnlyList<string> Aliases { get; }

        string DefaultType { get; }
        Type SettingsType => typeof(object);

        string Serialize(object? value);
        object? Deserialize<T>(string text);

        string Serialize(object? value, object? settings)
        {
            return Serialize(value);
        }

        object? Deserialize<T>(string text, object? settings)
        {
            return Deserialize<T>(text);
        }
    }

    public interface IConverter<TSettings> : IConverter, IConfigurableConverter
    {
        string Serialize(object? value, TSettings? settings);
        object? Deserialize<T>(string text, TSettings? settings);

        Type IConverter.SettingsType => typeof(TSettings);

        string IConverter.Serialize(object? value)
        {
            return Serialize(value, default);
        }

        object? IConverter.Deserialize<T>(string text)
        {
            return Deserialize<T>(text, default);
        }

        string IConverter.Serialize(object? value, object? settings)
        {
            return Serialize(value, settings is TSettings ? (TSettings)settings : default);
        }

        object? IConverter.Deserialize<T>(string text, object? settings)
        {
            return Deserialize<T>(text, settings is TSettings ? (TSettings)settings : default);
        }
    }
}
