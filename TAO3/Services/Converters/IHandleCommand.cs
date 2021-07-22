using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace TAO3.Converters
{
    public interface IHandleCommand
    {
        ICommandHandler CreateHandler(ConvertionContextProvider contextProvider);
    }

    public interface IHandleCommand<TSettings, TCommandParameters> : IHandleCommand
        where TCommandParameters : ConverterCommandParameters, new()
    {
        Task HandleCommandAsync(IConverterContext<TSettings> context, TCommandParameters args);

        ICommandHandler IHandleCommand.CreateHandler(ConvertionContextProvider contextProvider)
        {
            return CommandHandler.Create(async (TCommandParameters args) =>
            {
                IConverterContext<TSettings> converterContext = contextProvider.Invoke<TSettings>(args.Name!, args.Settings!, args.Verbose, args.Context!);
                await HandleCommandAsync(converterContext, args);
            });
        }
    }
}
