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

namespace TAO3.Internal.Commands.Input
{
    internal class InputCommand : Command
    {
        public InputCommand(
            ISourceService sourceService,
            IFormatConverterService formatConverterService,
            CSharpKernel cSharpKernel)
            : base("#!input", "Get a value from a source and convert it to a C# object")
        {
            AddAlias("#!in");

            sourceService.Events.RegisterChildCommand<ISourceEvent, SourceAddedEvent, SourceRemovedEvent>(
                this,
                x => x.Source.Name,
                evnt =>
                {
                    return TypeInferer.Invoke(
                        evnt.Source,
                        typeof(ISource<>),
                        () => CreateSourceCommand<Unit>(evnt.Source, formatConverterService, cSharpKernel));
                });
        }

        private (Command, IDisposable) CreateSourceCommand<TSourceOptions>(
            ISource source,
            IFormatConverterService formatConverterService,
            CSharpKernel cSharpKernel)
        {
            Command command = new Command(source.Name);
            command.AddAliases(source.Aliases);

            if (source is IConfigurableSource configurableSource)
            {
                configurableSource.Configure(command);
            }

            IDisposable formatSubscription = formatConverterService.Events.RegisterChildCommand<IConverterEvent, ConverterRegisteredEvent, ConverterUnregisteredEvent>(
                command,
                x => x.Converter.Format,
                evnt =>
                {
                    return TypeInferer.Invoke(
                        evnt.Converter,
                        typeof(IConverter<>),
                        () => CreateConverterCommand<Unit, TSourceOptions>(source, evnt.Converter, cSharpKernel));
                });

            return (command, formatSubscription);
        }

        private Command CreateConverterCommand<TSettings, TSourceOptions>(
            ISource source,
            IConverter converter,
            CSharpKernel cSharpKernel)
        {
            return CreateConverterCommand(
                (ISource<TSourceOptions>)source,
                (IConverter<TSettings>)converter,
                cSharpKernel);
        }

        private Command CreateConverterCommand<TSettings, TSourceOptions>(
            ISource<TSourceOptions> source,
            IConverter<TSettings> converter,
            CSharpKernel cSharpKernel)
        {
            Command command = new Command(converter.Format);
            command.AddAliases(converter.Aliases);

            if (converter.GetType().IsAssignableToGenericType(typeof(IHandleInputCommand<,>)))
            {
                TypeInferer.Invoke(
                    converter,
                    typeof(IHandleInputCommand<,>),
                    () => CreateCommandHandler<TSettings, Unit, TSourceOptions>(source, converter, command, cSharpKernel));
            }
            else if (converter.GetType().IsAssignableToGenericType(typeof(IInputConfigurableConverterCommand<,>)))
            {
                TypeInferer.Invoke(
                    converter,
                    typeof(IInputConfigurableConverterCommand<,>),
                    () => CreateCommandHandler<TSettings, Unit, TSourceOptions>(source, converter, command, cSharpKernel));
            }
            else
            {
                CreateCommandHandler<TSettings, Unit, TSourceOptions>(source, converter, command, cSharpKernel);
            }

            return command;
        }

        private void CreateCommandHandler<TSettings, TCommandParameters, TSourceOptions>(
            ISource<TSourceOptions> source,
            IConverter<TSettings> converter,
            Command command,
            CSharpKernel cSharpKernel)
        {
            IHandleInputCommand<TSettings, TCommandParameters> handler = converter as IHandleInputCommand<TSettings, TCommandParameters> ?? new DefaultInputCommandHandler<TSettings, TCommandParameters>();
            handler.Configure(command);

            command.Add(new Argument<string>("name", "The name of the variable that will contain the deserialized clipboard content"));
            command.Add(new Option(new[] { "-v", "--verbose" }, "Print debugging information"));
            command.Add(CommandFactory.CreateSettingsOption<TSettings>(cSharpKernel));

            Action<TSourceOptions> converterBinder = ParameterBinder.Create<TSourceOptions, IConverter>(converter);
            Action<TCommandParameters> sourceBinder = ParameterBinder.Create<TCommandParameters, ISource>(source);

            command.Handler = CommandHandler.Create(async (string name, bool verbose, TSourceOptions sourceOptions, TCommandParameters converterParameters, SettingsWrapper<TSettings> settingsWrapper) =>
            {
                converterBinder.Invoke(sourceOptions);
                sourceBinder.Invoke(converterParameters);

                ConverterContext<TSettings> converterContext = new ConverterContext<TSettings>(converter, name, settingsWrapper.Settings, verbose, cSharpKernel, () => source.GetTextAsync(sourceOptions));
                converterContext.Settings = handler.BindParameters(settingsWrapper.Settings ?? handler.GetDefaultSettings(), converterParameters);
                await handler.HandleCommandAsync(converterContext, converterParameters);
            });
        }

        private class DefaultInputCommandHandler<TSettings, TCommandParameters> : IHandleInputCommand<TSettings, TCommandParameters>
        {
            public Task HandleCommandAsync(IConverterContext<TSettings> context, TCommandParameters args)
            {
                return context.DefaultHandleCommandAsync();
            }

            public TSettings BindParameters(TSettings settings, TCommandParameters args)
            {
                return settings;
            }

            public void Configure(Command command)
            {
                
            }

            public TSettings GetDefaultSettings()
            {
                return default!;
            }
        }
    }
}
