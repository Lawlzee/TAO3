using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters;

namespace TAO3.Internal.Commands.Output
{
    internal interface IConverterSerializer<TSettings>
    {
        string Serialize(object? value, TSettings? settings);
    }

    internal class ConverterSerializer<T, TSettings> : IConverterSerializer<TSettings>
    {
        private readonly IConverter<T, TSettings> _converter;

        public ConverterSerializer(IConverter<T, TSettings> converter)
        {
            _converter = converter;
        }

        public string Serialize(object? value, TSettings? settings)
        {
            return _converter.Serialize(value, settings);
        }
    }

    internal class TypeProviderConverterSerializer<TSettings, TCommandParameters> : IConverterSerializer<TSettings>
    {
        private readonly IConverterTypeProvider<TSettings, TCommandParameters> _converter;

        public TypeProviderConverterSerializer(IConverterTypeProvider<TSettings, TCommandParameters> converter)
        {
            _converter = converter;
        }

        public string Serialize(object? value, TSettings? settings)
        {
            return _converter.Serialize(value, settings);
        }
    }
}
