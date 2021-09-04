using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters;

namespace TAO3.Internal.Commands.Output
{
    internal class ConverterAdapter<TSettings, TCommandParameters> :
        IOutputConfigurableConverter<TSettings, TCommandParameters>,
        IConverterSerializer<TSettings>
    {
        public IConverter Converter { get; }
        private readonly IOutputConfigurableConverter<TSettings, TCommandParameters> _configurable;
        private readonly IConverterSerializer<TSettings> _serializer;

        public string Format => Converter.Format;
        public IReadOnlyList<string> Aliases => Converter.Aliases;
        public string MimeType => Converter.MimeType;
        public Dictionary<string, object> Properties => Converter.Properties;

        public ConverterAdapter(IConverter converter, IOutputConfigurableConverter<TSettings, TCommandParameters> configurable, IConverterSerializer<TSettings> serializer)
        {
            Converter = converter;
            _configurable = configurable;
            _serializer = serializer;
        }

        public TSettings BindParameters(TSettings settings, TCommandParameters args)
        {
            return _configurable.BindParameters(settings, args);
        }

        public void Configure(Command command)
        {
            _configurable.Configure(command);
        }

        public TSettings GetDefaultSettings()
        {
            return _configurable.GetDefaultSettings();
        }

        public string Serialize(object? value, TSettings? settings)
        {
            return _serializer.Serialize(value, settings);
        }
    }
}
