using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters
{
    public interface IConfigurableConverterCommand
    {
        void Configure(Command command);
    }

    public interface IConfigurableConverterCommand<TSettings, TCommandParameters> : IConfigurableConverterCommand
        where TCommandParameters : new()
    {
        TSettings GetDefaultSettings();
        TSettings BindParameters(TSettings settings, TCommandParameters args) => settings;
    }
}
