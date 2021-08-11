using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;

namespace TAO3.Converters
{
    public class OutputConverterCommandParameters
    {
        public string? Settings { get; set; }
        public KernelInvocationContext Context { get; set; } = null!;
    }

    public interface IOutputConfigurableConverterCommand : IConfigurableConverterCommand
{
        ICommandHandler CreateHandler(Action<KernelInvocationContext, object?> handler);
    }

    public interface IOutputConfigurableConverterCommand<TSettings, TCommandParameters> : IConfigurableConverterCommand<TSettings, TCommandParameters>, IOutputConfigurableConverterCommand
        where TCommandParameters : OutputConverterCommandParameters, new()
    {
        ICommandHandler IOutputConfigurableConverterCommand.CreateHandler(Action<KernelInvocationContext, object?> handler)
        {
            return CommandHandler.Create((TCommandParameters args) =>
            {
                object? settingsInstance = null;

                CSharpKernel cSharpKernel = args.Context.GetCSharpKernel();
                if (!string.IsNullOrEmpty(args.Settings) && !cSharpKernel.TryGetVariable(args.Settings, out settingsInstance))
                {
                    args.Context.Fail(new ArgumentException(), $"The variable '{args.Settings}' was not found");
                    return;
                }

                settingsInstance = BindParameters((TSettings)settingsInstance! ?? GetDefaultSettings(), args);
                handler.Invoke(args.Context, settingsInstance);
            });
        }
    }

    public interface IOutputConfigurableConverterCommand<TSettings> : IOutputConfigurableConverterCommand<TSettings, OutputConverterCommandParameters>
    {

    }
}
