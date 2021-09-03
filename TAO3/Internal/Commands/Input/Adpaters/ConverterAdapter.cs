using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters;
using TAO3.TypeProvider;

namespace TAO3.Internal.Commands.Input
{
    internal class ConverterAdapter<TSettings, TCommandParameters> : 
        IConverter, 
        IConverterTypeProviderAdapter<TSettings, TCommandParameters>,
        IInputConfigurableConverter<TSettings, TCommandParameters>
    {
        private readonly IConverterTypeProviderAdapter<TSettings, TCommandParameters> _typeProvider;
        private readonly IInputConfigurableConverter<TSettings, TCommandParameters> _configurable;
        public IConverter Converter { get; }
        public string Format => Converter.Format;
        public IReadOnlyList<string> Aliases => Converter.Aliases;
        public string MimeType => Converter.MimeType;
        public Dictionary<string, object> Properties => Converter.Properties;

        public IDomCompiler DomCompiler => _typeProvider.DomCompiler;

        public ConverterAdapter(
            IConverterTypeProviderAdapter<TSettings, TCommandParameters> typeProvider, 
            IInputConfigurableConverter<TSettings, TCommandParameters> configurable, 
            IConverter converter)
        {
            _typeProvider = typeProvider;
            _configurable = configurable;
            Converter = converter;
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

        public Task<IDomType> ProvideTypeAsync(IConverterContext<TSettings> context, TCommandParameters args)
        {
            return _typeProvider.ProvideTypeAsync(context, args);
        }

        public T Deserialize<T>(string text, TSettings? settings)
        {
            return _typeProvider.Deserialize<T>(text, settings);
        }
    }
}
