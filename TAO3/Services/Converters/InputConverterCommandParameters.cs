using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Converters
{
    public class InputConverterCommandParameters
    {
        public string? Name { get; set; }
        public string? Settings { get; set; }
        public bool Verbose { get; set; }
        public KernelInvocationContext? Context { get; set; }
    }

    public interface IInputConfigurableConverterCommand : IConfigurableConverterCommand
    {

    }

    public interface IInputConfigurableConverterCommand<TSettings, TCommandParameters> : IConfigurableConverterCommand<TSettings, TCommandParameters>, IInputConfigurableConverterCommand
        where TCommandParameters : InputConverterCommandParameters,  new()
    {

    }
}
