using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Reactive;
using System.Reactive.Disposables;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.Parsing;
using TAO3.Converters;
using TAO3.Internal.Commands;
using TAO3.Internal.Commands.Input;
using TAO3.Internal.Extensions;
using TAO3.Internal.Types;
using TAO3.TypeProvider;

namespace TAO3.Internal.Kernels.Variable;

internal class VariableKernel
    : Kernel,
    IKernelCommandHandler<SubmitCode>
{
    public override ChooseKernelDirective ChooseKernelDirective { get; }
    private readonly Dictionary<Command, Func<SubmitCode, ParseResult, Task>> _handleSubmitCodeByCommand;
    private readonly CSharpKernel _cSharpKernel;

    public VariableKernel(
        IConverterService converterService,
        CSharpKernel cSharpKernel)
        : base("variable")
    {
        _cSharpKernel = cSharpKernel;
        _handleSubmitCodeByCommand = new Dictionary<Command, Func<SubmitCode, ParseResult, Task>>();
        ChooseKernelDirective = new ChooseKernelDirective(this);

        converterService.Events.RegisterChildCommand<IConverterEvent, ConverterRegisteredEvent, ConverterUnregisteredEvent>(
            ChooseKernelDirective,
            x => x.Converter.Format,
            evnt =>
            {
                Command command = new Command(evnt.Converter.Format);

                Func<SubmitCode, ParseResult, Task> handleSubmitCode;

                if (evnt.Converter.GetType().IsAssignableToGenericType(typeof(IConverter<,>)))
                {
                    handleSubmitCode = TypeInferer.Invoke(
                        evnt.Converter,
                        typeof(IConverter<,>),
                        () => CreateConverterCommand<Unit, Unit>(evnt.Converter, command));
                }
                else
                {
                    handleSubmitCode = TypeInferer.Invoke(
                        evnt.Converter,
                        typeof(IConverterTypeProvider<,>),
                        () => CreateConverterTypeProviderCommand<Unit, Unit>(evnt.Converter, command));
                }

                _handleSubmitCodeByCommand[command] = handleSubmitCode;

                return (command, Disposable.Create(() => _handleSubmitCodeByCommand.Remove(command)));
            });

    }

    private Func<SubmitCode, ParseResult, Task> CreateConverterCommand<T, TSettings>(
        IConverter converter,
        Command command)
    {
        IConverter<T, TSettings> typedConverter = (IConverter<T, TSettings>)converter;

        bool isConfigurable = converter.GetType()
            .GetInterfaces()
            .Where(x => x.IsGenericType)
            .Where(x => x.GetGenericTypeDefinition() == typeof(IInputConfigurableConverter<,>))
            .Where(x => x.GetGenericArguments()[0] == typeof(TSettings))
            .Any();

        if (isConfigurable)
        {
            return TypeInferer.Invoke(
                converter,
                typeof(IInputConfigurableConverter<,>),
                () => CreateConfigurableConverterCommand<TSettings, Unit, T>(typedConverter, command));
        }

        DefaultConverterTypeProviderAdapter<T, TSettings, Unit> typeProvider = new(typedConverter);
        DefaultInputConfigurableConverter<TSettings> configurable = new();

        ConverterAdapter<TSettings, Unit> converterAdapter = new(
            typeProvider,
            configurable,
            converter);

        return DoCreateConverterCommand(
            converterAdapter,
            command);
    }

    public Func<SubmitCode, ParseResult, Task> CreateConfigurableConverterCommand<TSettings, TCommandParameters, T>(
        IConverter<T, TSettings> converter,
        Command command)
    {
        DefaultConverterTypeProviderAdapter<T, TSettings, TCommandParameters> typeProvider = new(converter);
        IInputConfigurableConverter<TSettings, TCommandParameters> configurableConverter = (IInputConfigurableConverter<TSettings, TCommandParameters>)converter;

        ConverterAdapter<TSettings, TCommandParameters> converterAdapter = new(
            typeProvider,
            configurableConverter,
            converter);

        return DoCreateConverterCommand(
            converterAdapter,
            command);
    }

    private Func<SubmitCode, ParseResult, Task> CreateConverterTypeProviderCommand<TSettings, TCommandParameters>(
        IConverter converter,
        Command command)
    {
        IConverterTypeProvider<TSettings, TCommandParameters> typedConverter = (IConverterTypeProvider<TSettings, TCommandParameters>)converter;
        ConverterTypeProviderAdapter<TSettings, TCommandParameters> typeProviderAdapter = new(typedConverter);

        ConverterAdapter<TSettings, TCommandParameters> converterAdapter = new ConverterAdapter<TSettings, TCommandParameters>(
            typeProviderAdapter,
            typedConverter,
            typedConverter);

        return DoCreateConverterCommand(
            converterAdapter,
            command);
    }

    private Func<SubmitCode, ParseResult, Task> DoCreateConverterCommand<TSettings, TCommandParameters>(
        ConverterAdapter<TSettings, TCommandParameters> converter,
        Command command)
    {
        command.AddAliases(converter.Aliases);

        converter.Configure(command);

        command.Add(new Argument<string>("name", "The name of the variable that will contain the deserialized source content"));
        command.Add(new Option(new[] { "-v", "--verbose" }, "Print debugging information"));

        if (typeof(TSettings) != typeof(Unit))
        {
            command.Add(CommandFactory.CreateSettingsOption<TSettings>(_cSharpKernel));
        }

        Func<SubmitCode, ParseResult, Task> currentHandler = (_, _) => Task.CompletedTask;

        command.Handler = CommandHandler.Create((string name, bool verbose, TCommandParameters converterParameters, SettingsWrapper<TSettings> settingsWrapper) =>
        {
            TSettings realSettings = converter.BindParameters(settingsWrapper.Settings ?? converter.GetDefaultSettings(), converterParameters);

            currentHandler = async (submitCode, parseResult) =>
            {
                try
                {
                    ConverterContext<TSettings> context = new ConverterContext<TSettings>(name, realSettings, () => Task.FromResult(submitCode.Code));

                    IDomType inferedType = await converter.ProvideTypeAsync(context, converterParameters);
                    SchemaSerialization schema = converter.DomCompiler.Compile(inferedType);

                    string type = converter.DomCompiler.Serializer.PrettyPrint(schema.Root);

                    string text = await context.GetTextAsync();

                    string converterVariable = await _cSharpKernel.CreatePrivateVariableAsync(converter, typeof(IConverterDeserializerAdapter<TSettings>));
                    string textVariable = await _cSharpKernel.CreatePrivateVariableAsync(text, typeof(string));
                    string settingsVariable = await _cSharpKernel.CreatePrivateVariableAsync(realSettings, typeof(TSettings));

                    await _cSharpKernel.SubmitCodeAsync($@"{schema.Code}

{type} {context.VariableName} = {converterVariable}.Deserialize<{type}>({textVariable}, {settingsVariable});", verbose);

                }
                finally
                {
                    currentHandler = (_, _) => Task.CompletedTask;
                }
            };
        });

        return (submitCode, parseResult) =>
        {
            return currentHandler(submitCode, parseResult);
        };
    }

    public async Task HandleAsync(SubmitCode command, KernelInvocationContext context)
    {
        KernelNameDirectiveNode node = command.GetKernelNameDirectiveNode();
        if (node == null)
        {
            return;
        }

        ParseResult parseResult = node.GetDirectiveParseResult();
        if (_handleSubmitCodeByCommand.TryGetValue(parseResult.CommandResult.Command, out var handler))
        {
            await handler(command, parseResult);
        }
    }
}
