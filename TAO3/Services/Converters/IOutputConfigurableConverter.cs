using System.CommandLine;

namespace TAO3.Converters;

public interface IOutputConfigurableConverter<TSettings, TCommandParameters>
{
    void Configure(Command command);
    TSettings GetDefaultSettings();
    TSettings BindParameters(TSettings settings, TCommandParameters args) => settings;
}
