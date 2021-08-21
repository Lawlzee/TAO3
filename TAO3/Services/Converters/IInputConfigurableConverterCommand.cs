using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters
{
    public interface IInputConfigurableConverterCommand<TSettings, TCommandParameters>
    {
        void Configure(Command command);
        TSettings GetDefaultSettings();
        TSettings BindParameters(TSettings settings, TCommandParameters args);
    }
}
