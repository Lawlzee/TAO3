using System.CommandLine;
using System.Reactive;
using TAO3.Converters;

namespace TAO3.Internal.Commands.Input;

internal class DefaultInputConfigurableConverter<TSettings>
    : IInputConfigurableConverter<TSettings, Unit>
{
    public TSettings BindParameters(TSettings settings, Unit args)
    {
        return settings;
    }

    public void Configure(Command command)
    {
    }

    public TSettings GetDefaultSettings()
    {
        return default!;
    }
}
