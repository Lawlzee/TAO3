using Microsoft.CodeAnalysis.CSharp;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TAO3.Converters;
using TAO3.IO;
using TAO3.Internal.Extensions;
using TAO3.Internal.Types;
using System.Reactive;
using TAO3.TypeProvider;

namespace TAO3.Internal.Commands.Input
{
    internal class InputCommand : Command
    {
        private readonly CSharpKernel _cSharpKernel;

        public InputCommand(
            ISourceService sourceService,
            IConverterService converterService,
            CSharpKernel cSharpKernel)
            : base("#!input", "Get a value from a source and convert it to a C# object")
        {
            _cSharpKernel = cSharpKernel;

            AddAlias("#!in");

            sourceService.Events.RegisterChildCommand<ISourceEvent, SourceAddedEvent, SourceRemovedEvent>(
                this,
                x => x.Source.Name,
                evnt =>
                {
                    if (evnt.Source.GetType().IsAssignableToGenericType(typeof(ITextSource<>)))
                    {
                        return TypeInferer.Invoke(
                            evnt.Source,
                            typeof(ITextSource<>),
                            () => CreateTextSourceCommand<Unit>(evnt.Source, converterService));
                    }

                    return TypeInferer.Invoke(
                        evnt.Source,
                        typeof(IIntermediateSource<>),
                        () => CreateIntermediateSourceCommand<Unit>(evnt.Source, converterService));
                });
        }

        private (Command, IDisposable) CreateTextSourceCommand<TSourceOptions>(
            ISource source,
            IConverterService converterService)
        {
            ITextSource<TSourceOptions> typedSource = (ITextSource<TSourceOptions>)source;
            TextSourceAdapter<TSourceOptions> sourceAdapter = new(typedSource);

            return CreateSourceCommand(
                sourceAdapter,
                converterService);
        }

        private (Command, IDisposable) CreateIntermediateSourceCommand<TSourceOptions>(
            ISource source,
            IConverterService converterService)
        {
            IIntermediateSource<TSourceOptions> typedSource = (IIntermediateSource<TSourceOptions>)source;
            IntermediateSourceAdapter<TSourceOptions> sourceAdapter = new(typedSource);

            return CreateSourceCommand(
                sourceAdapter,
                converterService);
        }

        private (Command, IDisposable) CreateSourceCommand<TSourceOptions>(
            ISourceAdapter<TSourceOptions> source,
            IConverterService converterService)
        {
            Command command = new Command(source.Name);
            command.AddAliases(source.Aliases);

            source.Configure(command);

            IDisposable formatSubscription = converterService.Events.RegisterChildCommand<IConverterEvent, ConverterRegisteredEvent, ConverterUnregisteredEvent>(
                command,
                x => x.Converter.Format,
                evnt =>
                {
                    if (evnt.Converter.GetType().IsAssignableToGenericType(typeof(IConverter<,>)))
                    {
                        return TypeInferer.Invoke(
                            evnt.Converter,
                            typeof(IConverter<,>),
                            () => CreateConverterCommand<Unit, Unit, TSourceOptions>(source, evnt.Converter));
                    }

                    return TypeInferer.Invoke(
                        evnt.Converter,
                        typeof(IConverterTypeProvider<,>),
                        () => CreateConverterTypeProviderCommand<Unit, Unit, TSourceOptions>(source, evnt.Converter));
                });

            return (command, formatSubscription);
        }

        private Command CreateConverterCommand<T, TSettings, TSourceOptions>(
            ISourceAdapter<TSourceOptions> source,
            IConverter converter)
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
                    () => CreateConfigurableConverterCommand<TSettings, Unit, T, TSourceOptions>(source, typedConverter));
            }

            DefaultConverterTypeProviderAdapter<T, TSettings, Unit> typeProvider = new(typedConverter);
            DefaultInputConfigurableConverter<TSettings> configurable = new();

            ConverterAdapter<TSettings, Unit> converterAdapter = new(
                typeProvider,
                configurable,
                converter);

            return DoCreateConverterCommand(
                source,
                converterAdapter);
        }

        public Command CreateConfigurableConverterCommand<TSettings, TCommandParameters, T, TSourceOptions>(
            ISourceAdapter<TSourceOptions> source,
            IConverter<T, TSettings> converter)
        {
            DefaultConverterTypeProviderAdapter<T, TSettings, TCommandParameters> typeProvider = new(converter);
            IInputConfigurableConverter<TSettings, TCommandParameters> configurableConverter = (IInputConfigurableConverter<TSettings, TCommandParameters>)converter;

            ConverterAdapter<TSettings, TCommandParameters> converterAdapter = new(
                typeProvider,
                configurableConverter,
                converter);

            return DoCreateConverterCommand(
                source,
                converterAdapter);
        }

        private Command CreateConverterTypeProviderCommand<TSettings, TCommandParameters, TSourceOptions>(
            ISourceAdapter<TSourceOptions> source,
            IConverter converter)
        {
            IConverterTypeProvider<TSettings, TCommandParameters> typedConverter = (IConverterTypeProvider<TSettings, TCommandParameters>)converter;
            ConverterTypeProviderAdapter<TSettings, TCommandParameters> typeProviderAdapter = new(typedConverter);

            ConverterAdapter<TSettings, TCommandParameters> converterAdapter = new ConverterAdapter<TSettings, TCommandParameters>(
                typeProviderAdapter,
                typedConverter,
                typedConverter);

            return DoCreateConverterCommand(
                source,
                converterAdapter);
        }

        private Command DoCreateConverterCommand<TSettings, TCommandParameters, TSourceOptions>(
            ISourceAdapter<TSourceOptions> source,
            ConverterAdapter<TSettings, TCommandParameters> converter)
        {
            Command command = new Command(converter.Format);
            command.AddAliases(converter.Aliases);

            converter.Configure(command);

            command.Add(new Argument<string>("name", "The name of the variable that will contain the deserialized clipboard content"));
            command.Add(new Option(new[] { "-v", "--verbose" }, "Print debugging information"));
            
            if (typeof(TSettings) != typeof(Unit))
            {
                command.Add(CommandFactory.CreateSettingsOption<TSettings>(_cSharpKernel));
            }

            Action<TSourceOptions> converterBinder = ParameterBinder.Create<TSourceOptions, IConverter>(converter.Converter);
            Action<TCommandParameters> sourceBinder = ParameterBinder.Create<TCommandParameters, ISource>(source);

            command.Handler = CommandHandler.Create(async (string name, bool verbose, TSourceOptions sourceOptions, TCommandParameters converterParameters, SettingsWrapper<TSettings> settingsWrapper) =>
            {
                converterBinder.Invoke(sourceOptions);
                sourceBinder.Invoke(converterParameters);

                TSettings realSettings = converter.BindParameters(settingsWrapper.Settings ?? converter.GetDefaultSettings(), converterParameters);

                await source.DeserializeAsync(
                    sourceOptions,
                    converter,
                    converterParameters,
                    variableName: name,
                    verbose,
                    realSettings,
                    _cSharpKernel);
            });

            return command;
        }
    }
}
