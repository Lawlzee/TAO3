using Microsoft.DotNet.Interactive;
using System.CommandLine;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive.Commands;
using TAO3.Internal.Extensions;
using TAO3.Converters;
using Microsoft.DotNet.Interactive.CSharp;
using TAO3.IO;
using System.Reactive.Linq;
using System.Reactive;
using TAO3.Internal.Types;
using TAO3.Converters.Default;
using Newtonsoft.Json;
using TAO3.Converters.Json;
using System.CommandLine.NamingConventionBinder;

namespace TAO3.Internal.Commands.Output;

internal class OutputCommand : Command
{
    private readonly CSharpKernel _cSharpKernel;
    private readonly DefaultConverter _defaultConverter;

    public OutputCommand(
        IDestinationService destinationService,
        IConverterService converterService,
        CSharpKernel cSharpKernel,
        ClipboardIO defaultDestination,
        DefaultConverter defaultConverter)
        : base("#!output", "Convert and copy returned value to destination")
    {
        _cSharpKernel = cSharpKernel;
        _defaultConverter = defaultConverter;

        AddAlias("#!out");

        CreateConverterCommand(
            defaultDestination,
            new ConverterAdapter<JsonSerializerSettings, Unit>(
                defaultConverter,
                defaultConverter),
            this);

        destinationService.Events.RegisterChildCommand<IDestinationEvent, DestinationAddedEvent, DestinationRemovedEvent>(
            this,
            x => x.Destination.Name,
            evnt =>
            {
                return TypeInferer.Invoke(
                    evnt.Destination,
                    typeof(IDestination<>),
                    () => CreateDestinationCommand<Unit>(evnt.Destination, converterService));
            });
        
    }

    private (Command, IDisposable) CreateDestinationCommand<TDestinationOptions>(
        IDestination destination, 
        IConverterService converterService)
    {
        Command command = new Command(destination.Name);
        command.AddAliases(destination.Aliases);

        if (destination is IConfigurableDestination configurableDestination)
        {
            configurableDestination.Configure(command);
        }

        IDestination<TDestinationOptions> typedDestination = (IDestination<TDestinationOptions>)destination;

        TypeInferer.Invoke(
            _defaultConverter,
            typeof(IOutputConfigurableConverter<,>),
            () => CreateConverterCommandOutputConfigurable<Unit, Unit, TDestinationOptions>(typedDestination, _defaultConverter, command));

        IDisposable formatSubscription = converterService.Events.RegisterChildCommand<IConverterEvent, ConverterRegisteredEvent, ConverterUnregisteredEvent>(
            command,
            x => x.Converter.Format,
            evnt =>
            {
                Command converterCommand = new Command(evnt.Converter.Format);
                if (evnt.Converter.GetType().IsAssignableToGenericType(typeof(IOutputConfigurableConverter<,>)))
                {
                    TypeInferer.Invoke(
                        evnt.Converter,
                        typeof(IOutputConfigurableConverter<,>),
                        () => CreateConverterCommandOutputConfigurable<Unit, Unit, TDestinationOptions>(typedDestination, evnt.Converter, converterCommand));
                }
                else if (evnt.Converter.GetType().IsAssignableToGenericType(typeof(IConverter<,>)))
                {
                    TypeInferer.Invoke(
                        evnt.Converter,
                        typeof(IConverter<,>),
                        () => CreateConverterCommandInputSettings<Unit, Unit, TDestinationOptions>(typedDestination, evnt.Converter, converterCommand));
                }
                else
                {
                    ConverterAdapter<Unit, Unit> converterAdapter = new(
                        evnt.Converter,
                        new DefaultOutputConfigurableConverter<Unit, Unit>());

                    CreateConverterCommand(
                        typedDestination,
                        converterAdapter,
                        converterCommand);
                }

                return converterCommand;
            });

        return (command, formatSubscription);
    }

    private void CreateConverterCommandOutputConfigurable<TSettings, TCommandParameters, TDestinationOptions>(
        IDestination<TDestinationOptions> destination,
        IConverter converter,
        Command command)
    {
        IOutputConfigurableConverter<TSettings, TCommandParameters> configurable = converter as IOutputConfigurableConverter<TSettings, TCommandParameters>
            ?? new DefaultOutputConfigurableConverter<TSettings, TCommandParameters>();

        ConverterAdapter<TSettings, TCommandParameters> converterAdapter = new(
            converter,
            configurable);

        CreateConverterCommand(
            destination,
            converterAdapter,
            command);
    }

    private void CreateConverterCommandInputSettings<T, TSettings, TDestinationOptions>(
        IDestination<TDestinationOptions> destination,
        IConverter converter,
        Command command)
    {
        IOutputConfigurableConverter<TSettings, Unit> configurable = new DefaultOutputConfigurableConverter<TSettings, Unit>();

        ConverterAdapter<TSettings, Unit> converterAdapter = new(
            converter,
            configurable);

        CreateConverterCommand(
            destination,
            converterAdapter,
            command);
    }

    private void CreateConverterCommand<TSettings, TCommandParameters, TDestinationOptions>(
        IDestination<TDestinationOptions> destination,
        ConverterAdapter<TSettings, TCommandParameters> converter,
        Command command)
    {
        command.AddAliases(converter.Converter.Aliases);

        converter.Configure(command);

        Argument<string?> variableArgument = new Argument<string?>("variable", description: "Name of the variable used as the output", getDefaultValue: () => null);
        variableArgument.AddCompletions(context =>
        {
            return _cSharpKernel
                .GetValueInfos()
                .Select(x => x.Name)
                .Where(x => context.WordToComplete?.Contains(x) ?? true);
        });
        command.Add(variableArgument);

        if (typeof(TSettings) != typeof(Unit))
        {
            command.Add(CommandFactory.CreateSettingsOption<TSettings>(_cSharpKernel));
        }

        Action<TDestinationOptions> converterBinder = ParameterBinder.Create<TDestinationOptions, IConverter>(converter.Converter);
        Action<TCommandParameters> destinationBinder = ParameterBinder.Create<TCommandParameters, IDestination>(destination);

        command.Handler = CommandHandler.Create((SettingsWrapper<TSettings> settingsWrapper, TDestinationOptions destinationOptions, TCommandParameters converterCommandParameters, string? variable, KernelInvocationContext context) =>
        {
            converterBinder.Invoke(destinationOptions);
            destinationBinder.Invoke(converterCommandParameters);

            TSettings bindedSettings = converter.BindParameters(settingsWrapper.Settings ?? converter.GetDefaultSettings(), converterCommandParameters);
            Handle(destination, destinationOptions, converter, bindedSettings, variable, context);
        });
    }


    private void Handle<TSettings, TCommandParameters, TDestinationOptions>(
        IDestination<TDestinationOptions> destination,
        TDestinationOptions destinationOptions,
        ConverterAdapter<TSettings, TCommandParameters> converter,
        TSettings settings,
        string? variableName,
        KernelInvocationContext context)
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
                            string resultText = converter.Converter.Serialize(valueProduced.Value, settings);
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
                                if (_cSharpKernel.TryGetValue(variableName, out object? variable))
                                {
                                    string resultText = converter.Converter.Serialize(variable, settings);
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
