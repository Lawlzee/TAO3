using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters;

namespace TAO3.Converters
{
    public interface IConverterEvent
    {
        IConverter Converter { get; }
    }

    public record ConverterRegisteredEvent(IConverter Converter) : IConverterEvent;
    public record ConverterUnregisteredEvent(IConverter Converter) : IConverterEvent;
}
