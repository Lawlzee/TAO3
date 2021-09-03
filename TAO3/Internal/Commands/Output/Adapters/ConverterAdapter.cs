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
        IConverter,
        IOutputConfigurableConverter<TSettings, TCommandParameters>,
        IConverterSerializer<TSettings>
    {
        private readonly IConverter _converter;
        private readonly IOutputConfigurableConverter<TSettings, TCommandParameters> _configurable;
        private readonly IConverterSerializer<TSettings> _serializer;

        public string Format => _converter.Format;
        public IReadOnlyList<string> Aliases => _converter.Aliases;
        public string MimeType => _converter.MimeType;
        public Dictionary<string, object> Properties => _converter.Properties;

        public ConverterAdapter(IConverter converter, IOutputConfigurableConverter<TSettings, TCommandParameters> configurable, IConverterSerializer<TSettings> serializer)
        {
            _converter = converter;
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
