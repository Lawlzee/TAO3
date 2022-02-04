using System.CommandLine;
using TAO3.Converters;

namespace TAO3.Internal.Commands.Output;

internal class ConverterAdapter<TSettings, TCommandParameters> :
    IOutputConfigurableConverter<TSettings, TCommandParameters>
{
    public IConverter Converter { get; }
    private readonly IOutputConfigurableConverter<TSettings, TCommandParameters> _configurable;

    public ConverterAdapter(IConverter converter, IOutputConfigurableConverter<TSettings, TCommandParameters> configurable)
    {
        Converter = converter;
        _configurable = configurable;
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
}
