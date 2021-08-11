using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;

namespace TAO3.Converters
{
    public interface IHandleInputCommand
    {
        ICommandHandler CreateHandler(ConvertionContextProvider contextProvider);
    }

    public interface IHandleInputCommand<TSettings, TCommandParameters> : IHandleInputCommand, IInputConfigurableConverterCommand<TSettings, TCommandParameters>
        where TCommandParameters : InputConverterCommandParameters, new()
    {
        Task HandleCommandAsync(IConverterContext<TSettings> context, TCommandParameters args);

        ICommandHandler IHandleInputCommand.CreateHandler(ConvertionContextProvider contextProvider)
        {
            return CommandHandler.Create(async (TCommandParameters args) =>
            {
                try
                {
                    IConverterContext<TSettings> converterContext = contextProvider.Invoke<TSettings>(args.Name!, args.Settings!, args.Verbose, args.Context!);
                    
                    TSettings settings = converterContext.Settings ?? GetDefaultSettings();
                    converterContext.Settings = BindParameters(settings, args);
                    
                    await HandleCommandAsync(converterContext, args);
                }
                catch (Exception ex)
                {
                    ex.Display();
                }
            });
        }
    }
}
