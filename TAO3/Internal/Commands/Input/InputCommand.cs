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

            if (converter.GetType().IsAssignableToGenericType(typeof(IInputTypeProvider<,>)))
            {
                TypeInferer.Invoke(
                    converter,
                    typeof(IInputTypeProvider<,>),
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
            IInputTypeProvider<TSettings, TCommandParameters> typeProvider = converter as IInputTypeProvider<TSettings, TCommandParameters> ?? new DefaultTypeProvider<TSettings, TCommandParameters>(converter);
            typeProvider.Configure(command);

            command.Add(new Argument<string>("name", "The name of the variable that will contain the deserialized clipboard content"));
            command.Add(new Option(new[] { "-v", "--verbose" }, "Print debugging information"));
            command.Add(CommandFactory.CreateSettingsOption<TSettings>(cSharpKernel));

            Action<TSourceOptions> converterBinder = ParameterBinder.Create<TSourceOptions, IConverter>(converter);
            Action<TCommandParameters> sourceBinder = ParameterBinder.Create<TCommandParameters, ISource>(source);

            command.Handler = CommandHandler.Create(async (string name, bool verbose, TSourceOptions sourceOptions, TCommandParameters converterParameters, SettingsWrapper<TSettings> settingsWrapper) =>
            {
                converterBinder.Invoke(sourceOptions);
                sourceBinder.Invoke(converterParameters);

                ConverterContext<TSettings> converterContext = new ConverterContext<TSettings>(name, settingsWrapper.Settings, verbose, cSharpKernel, () => source.GetTextAsync(sourceOptions));
                converterContext.Settings = typeProvider.BindParameters(settingsWrapper.Settings ?? typeProvider.GetDefaultSettings(), converterParameters);

                await DeserializeAsync(converterContext, converter, converterParameters, typeProvider);
            });
        }

        private async Task DeserializeAsync<TSettings, TCommandParameters>(
            ConverterContext<TSettings> context, 
            IConverter<TSettings> converter,
            TCommandParameters converterParameters,
            IInputTypeProvider<TSettings, TCommandParameters> typeProvider)
        {
            InferedType inferedType = await typeProvider.ProvideTypeAsync(context, converterParameters);
            SchemaSerialization schema = typeProvider.DomCompiler.Compile(inferedType.Type);

            string rootType = typeProvider.DomCompiler.Serializer.PrettyPrint(schema.Root);
            
            if (inferedType.ReturnTypeIsList && schema.RootElementType == null)
            {
                throw new Exception("Expected a collection as an infered return type");
            }
            
            string? elementType = schema.RootElementType != null
                ? typeProvider.DomCompiler.Serializer.PrettyPrint(schema.RootElementType)
                : null;

            string text = await context.GetTextAsync();

            string converterVariable = await context.CreatePrivateVariableAsync(converter, typeof(IConverter<TSettings>));
            string textVariable = await context.CreatePrivateVariableAsync(text, typeof(string));
            string settingsVariable = await context.CreatePrivateVariableAsync(context.Settings, typeof(TSettings));

            await context.SubmitCodeAsync($@"{schema.Code}

{rootType} {context.VariableName} = ({rootType}){converterVariable}.Deserialize<{(inferedType.ReturnTypeIsList ? elementType : rootType)}>({textVariable}, {settingsVariable});");
        }

        private class DefaultTypeProvider<TSettings, TCommandParameters> : IInputTypeProvider<TSettings, TCommandParameters>
        {
            private readonly IConverter _converter;

            public IDomCompiler DomCompiler { get; }

            public DefaultTypeProvider(IConverter converter)
            {
                _converter = converter;
                DomCompiler = new DomCompiler(
                    converter.Format,
                    IDomSchematizer.Default,
                    IDomSchemaSerializer.Default);
            }

            public Task<InferedType> ProvideTypeAsync(IConverterContext<TSettings> context, TCommandParameters args)
            {
                return Task.FromResult(new InferedType(new DomClassReference(_converter.DefaultType)));
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
