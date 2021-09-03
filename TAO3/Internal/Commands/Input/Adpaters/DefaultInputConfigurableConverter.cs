using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters;

namespace TAO3.Internal.Commands.Input
{
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
}
