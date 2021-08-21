using Microsoft.DotNet.Interactive;
using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive.Commands;
using TAO3.Internal.Extensions;
using TAO3.Converters;
using Microsoft.DotNet.Interactive.CSharp;
using TAO3.IO;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Reactive;
using System.Linq.Expressions;
using TAO3.Internal.Types;

namespace TAO3.Internal.Commands.Output
{
    internal class OutputCommand : Command
    {
        public OutputCommand(
            IDestinationService destinationService,
            IFormatConverterService formatConverterService,
            CSharpKernel cSharpKernel)
            : base("#!output", "Convert and copy returned value to destination")
        {
            AddAlias("#!out");

            destinationService.Events.RegisterChildCommand<IDestinationEvent, DestinationAddedEvent, DestinationRemovedEvent>(
                this,
                x => x.Destination.Name,
                evnt =>
                {
                    return TypeInferer.Invoke(
                        evnt.Destination,
                        typeof(IDestination<>),
                        () => CreateDestinationCommand<Unit>(evnt.Destination, formatConverterService, cSharpKernel));
                });
        }

        private (Command, IDisposable) CreateDestinationCommand<TDestinationOptions>(
            IDestination destination, 
            IFormatConverterService formatConverterService,
            CSharpKernel cSharpKernel)
        {
            Command command = new Command(destination.Name);
            command.AddAliases(destination.Aliases);

            IDisposable formatSubscription = formatConverterService.Events.RegisterChildCommand<IConverterEvent, ConverterRegisteredEvent, ConverterUnregisteredEvent>(
                command,
                x => x.Converter.Format,
                evnt => 
                {
                    return TypeInferer.Invoke(
                        evnt.Converter,
                        typeof(IConverter<>),
                        () => CreateConverterCommand<Unit, TDestinationOptions>(destination, evnt.Converter, cSharpKernel));
                });

            return (command, formatSubscription);
        }

        private Command CreateConverterCommand<TSettings, TDestinationOptions>(
            IDestination destination,
            IConverter converter,
            CSharpKernel cSharpKernel)
        {
            return CreateConverterCommand(
                (IDestination<TDestinationOptions>)destination,
                (IConverter<TSettings>)converter,
                cSharpKernel);
        }

        private Command CreateConverterCommand<TSettings, TDestinationOptions>(
            IDestination<TDestinationOptions> destination, 
            IConverter<TSettings> converter,
            CSharpKernel cSharpKernel)
        {
            Command command = new Command(converter.Format)
            {
                CommandFactory.CreateSettingsOption(cSharpKernel, typeof(TSettings))
            };

            command.AddAliases(converter.Aliases);

            if (destination is IConfigurableDestination configurableDestination)
            {
                configurableDestination.Configure(command);
            }

            if (converter.GetType().IsAssignableToGenericType(typeof(IOutputConfigurableConverterCommand<,>)))
            {
                TypeInferer.Invoke(
                    converter,
                    typeof(IOutputConfigurableConverterCommand<,>),
                    () => CreateConfigurableConverterHandler<TSettings, Unit, TDestinationOptions>(destination, converter, command));
                return command;
            }

            command.Handler = CommandHandler.Create((TSettings settings, TDestinationOptions destinationOptions, KernelInvocationContext context) =>
            {
                Handle(destination, converter, context, destinationOptions, settings);
            });

            return command;
        }

        private void CreateConfigurableConverterHandler<TSettings, TConverterCommandParameters, TDestinationOptions>(
            IDestination<TDestinationOptions> destination,
            IConverter<TSettings> converter,
            Command command)
        {
            IOutputConfigurableConverterCommand<TSettings, TConverterCommandParameters> configurableConverter = (IOutputConfigurableConverterCommand<TSettings, TConverterCommandParameters>)converter;
            configurableConverter.Configure(command);

            command.Handler = CommandHandler.Create((TSettings settings, TDestinationOptions destinationOptions, TConverterCommandParameters converterCommandParameters, KernelInvocationContext context) =>
            {
                TSettings bindedSettings = configurableConverter.BindParameters(settings ?? configurableConverter.GetDefaultSettings(), converterCommandParameters);
                Handle(destination, converter, context, destinationOptions, bindedSettings);
            });
        }

        private void Handle<TConverterOptions, TDestinationOptions>(
            IDestination<TDestinationOptions> destination,
            IConverter<TConverterOptions> converter,
            KernelInvocationContext context,
            TDestinationOptions destinationOptions,
            TConverterOptions settings)
        {
            Kernel rootKernel = context.HandlingKernel.ParentKernel;
            KernelCommand submitCodeCommand = context.Command.GetRootCommand();

            IDisposable disposable = null!;
            disposable = rootKernel.KernelEvents
                .SelectMany(async e =>
                {
                    KernelCommand rootCommand = e.Command.GetRootCommand();

                    if (rootCommand == submitCodeCommand)
                    {
                        if (e is ReturnValueProduced valueProduced)
                        {
                            try
                            {
                                string resultText = converter.Serialize(valueProduced.Value, settings);
                                await destination.SetTextAsync(resultText, destinationOptions);
                            }
                            catch (Exception ex)
                            {
                                ex.Display();
                            }
                            finally
                            {
                                disposable.Dispose();
                            }
                        }

                        if (e is CommandSucceeded commandSucceeded && commandSucceeded.Command == submitCodeCommand)
                        {
                            disposable.Dispose();
                        }

                        if (e is CommandFailed commandFailed)
                        {
                            disposable.Dispose();
                        }
                    }

                    return Unit.Default;
                })
                .Subscribe();
        }
    }
}
