using Microsoft.DotNet.Interactive;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace TAO3.Converters
{
    public interface IHandleInputCommand<TSettings, TCommandParameters> : IInputConfigurableConverterCommand<TSettings, TCommandParameters>
    {
        Task HandleCommandAsync(IConverterContext<TSettings> context, TCommandParameters args);
    }
}
