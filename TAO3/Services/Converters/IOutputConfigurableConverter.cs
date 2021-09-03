using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Reactive;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;

namespace TAO3.Converters
{
    public interface IOutputConfigurableConverter<TSettings, TCommandParameters>
    {
        void Configure(Command command);
        TSettings GetDefaultSettings();
        TSettings BindParameters(TSettings settings, TCommandParameters args) => settings;
    }
}
