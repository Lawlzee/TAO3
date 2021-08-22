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
        private readonly CSharpKernel _cSharpKernel;

        public OutputCommand(
            IDestinationService destinationService,
            IFormatConverterService formatConverterService,
            CSharpKernel cSharpKernel)
            : base("#!output", "Convert and copy returned value to destination")
        {
            _cSharpKernel = cSharpKernel;
            AddAlias("#!out");

            destinationService.Events.RegisterChildCommand<IDestinationEvent, DestinationAddedEvent, DestinationRemovedEvent>(
                this,
                x => x.Destination.Name,
                evnt =>
                {
                    return TypeInferer.Invoke(
                        evnt.Destination,
                        typeof(IDestination<>),
                        () => CreateDestinationCommand<Unit>(evnt.Destination, formatConverterService));
                });
        }

        private (Command, IDisposable) CreateDestinationCommand<TDestinationOptions>(
            IDestination destination, 
            IFormatConverterService formatConverterService)
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
                        () => CreateConverterCommand<Unit, TDestinationOptions>(destination, evnt.Converter));
                });

            return (command, formatSubscription);
        }

        private Command CreateConverterCommand<TSettings, TDestinationOptions>(
            IDestination destination,
            IConverter converter)
        {
            return CreateConverterCommand(
                (IDestination<TDestinationOptions>)destination,
                (IConverter<TSettings>)converter);
        }

        private Command CreateConverterCommand<TSettings, TDestinationOptions>(
            IDestination<TDestinationOptions> destination, 
            IConverter<TSettings> converter)
        {
            Command command = new Command(converter.Format)
            {
                CommandFactory.CreateSettingsOption<TSettings>(_cSharpKernel)
            };

            Argument<string?> variableArgument = new Argument<string?>("variable", description: "Variable to output", getDefaultValue: () => null);
            variableArgument.AddSuggestions((_, text) =>
            {
                return _cSharpKernel
                    .GetVariableNames()
                    .Where(x => text?.Contains(x) ?? true);
            });
            
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
                    () => CreateConfigurableConverterHandler<TSettings, Unit, TDestinationOptions>(destination, converter, command, variableArgument));
                return command;
            }
            else
            {
                CreateConfigurableConverterHandler<TSettings, Unit, TDestinationOptions>(destination, converter, command, variableArgument);
            }

            return command;
        }

        private void CreateConfigurableConverterHandler<TSettings, TCommandParameters, TDestinationOptions>(
            IDestination<TDestinationOptions> destination,
            IConverter<TSettings> converter,
            Command command,
            Argument<string?> variableArgument)
        {
            var configurableConverter = converter as IOutputConfigurableConverterCommand<TSettings, TCommandParameters> ?? new DefaultOutputConfigurableConverterCommand<TSettings, TCommandParameters>();
            
            configurableConverter.Configure(command);
            command.Add(variableArgument);

            Action<TDestinationOptions> converterBinder = ParameterBinder.Create<TDestinationOptions, IConverter>(converter);
            Action<TCommandParameters> destinationBinder = ParameterBinder.Create<TCommandParameters, IDestination>(destination);

            command.Handler = CommandHandler.Create((TSettings settings, TDestinationOptions destinationOptions, TCommandParameters converterCommandParameters, string? variable, KernelInvocationContext context) =>
            {
                converterBinder.Invoke(destinationOptions);
                destinationBinder.Invoke(converterCommandParameters);

                TSettings bindedSettings = configurableConverter.BindParameters(settings ?? configurableConverter.GetDefaultSettings(), converterCommandParameters);
                Handle(destination, converter, context, destinationOptions, bindedSettings, variable);
            });
        }

        private class DefaultOutputConfigurableConverterCommand<TSettings, TCommandParameters> : IOutputConfigurableConverterCommand<TSettings, TCommandParameters>
        {
            public void Configure(Command command)
            {

            }

            public TSettings GetDefaultSettings()
            {
                return default!;
            }
        }

        private void Handle<TConverterOptions, TDestinationOptions>(
            IDestination<TDestinationOptions> destination,
            IConverter<TConverterOptions> converter,
            KernelInvocationContext context,
            TDestinationOptions destinationOptions,
            TConverterOptions settings,
            string? variableName)
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
                        if (variableName == null && e is ReturnValueProduced valueProduced)
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
                            try
                            {
                                if (variableName != null)
                                {
                                    if (_cSharpKernel.TryGetVariable(variableName, out object? variable))
                                    {
                                        string resultText = converter.Serialize(variable, settings);
                                        await destination.SetTextAsync(resultText, destinationOptions);
                                    }
                                }
                            }
                            finally
                            {
                                disposable.Dispose();
                            }
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
