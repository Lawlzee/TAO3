using System;
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
    }

    public interface IConverter<TSettings> : IConverter
    {
        string Serialize(object? value, TSettings? settings);
        object? Deserialize<T>(string text, TSettings? settings);
    }
}
