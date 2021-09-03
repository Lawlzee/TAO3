using Microsoft.DotNet.Interactive.CSharp;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Converters;
using TAO3.Internal.Extensions;
using TAO3.IO;
using TAO3.TypeProvider;

namespace TAO3.Internal.Commands.Input
{
    internal interface ISourceAdapter<TSourceOptions> : ISource, IConfigurableSource
    {
        Task DeserializeAsync<TSettings, TCommandParameters>(
            TSourceOptions sourceOptions,
            ConverterAdapter<TSettings, TCommandParameters> converter,
            TCommandParameters converterParameters,
            string variableName,
            bool verbose,
            TSettings settings,
            CSharpKernel cSharpKernel);
    }

    internal class TextSourceAdapter<TSourceOptions>: ISourceAdapter<TSourceOptions>
    {
        private readonly ITextSource<TSourceOptions> _source;

        public string Name => _source.Name;
        public IReadOnlyList<string> Aliases => _source.Aliases;

        public TextSourceAdapter(ITextSource<TSourceOptions> source)
        {
            _source = source;
        }

        public void Configure(Command command)
        {
            if (_source is IConfigurableSource configurableSource)
            {
                configurableSource.Configure(command);
            }
        }

        public async Task DeserializeAsync<TSettings, TCommandParameters>(
            TSourceOptions sourceOptions,
            ConverterAdapter<TSettings, TCommandParameters> converter,
            TCommandParameters converterParameters,
            string variableName,
            bool verbose,
            TSettings settings,
            CSharpKernel cSharpKernel)
        {
            ConverterContext<TSettings> context = new ConverterContext<TSettings>(variableName, settings, () => _source.GetTextAsync(sourceOptions));

            IDomType inferedType = await converter.ProvideTypeAsync(context, converterParameters);
            SchemaSerialization schema = converter.DomCompiler.Compile(inferedType);

            string type = converter.DomCompiler.Serializer.PrettyPrint(schema.Root);

            string text = await context.GetTextAsync();

            string converterVariable = await cSharpKernel.CreatePrivateVariableAsync(converter, typeof(IConverterDeserializerAdapter<TSettings>));
            string textVariable = await cSharpKernel.CreatePrivateVariableAsync(text, typeof(string));
            string settingsVariable = await cSharpKernel.CreatePrivateVariableAsync(settings, typeof(TSettings));

            await cSharpKernel.SubmitCodeAsync($@"{schema.Code}

{type} {context.VariableName} = {converterVariable}.Deserialize<{type}>({textVariable}, {settingsVariable});", verbose);
        }

        
    }

    internal class IntermediateSourceAdapter<TSourceOptions>
        : ISourceAdapter<TSourceOptions>
    {
        private readonly IIntermediateSource<TSourceOptions> _source;

        public string Name => _source.Name;
        public IReadOnlyList<string> Aliases => _source.Aliases;

        public IntermediateSourceAdapter(IIntermediateSource<TSourceOptions> source)
        {
            _source = source;
        }

        public void Configure(Command command)
        {
            if (_source is IConfigurableSource configurableSource)
            {
                configurableSource.Configure(command);
            }
        }

        public async Task DeserializeAsync<TSettings, TCommandParameters>(TSourceOptions sourceOptions,
            ConverterAdapter<TSettings, TCommandParameters> converter,
            TCommandParameters converterParameters,
            string variableName,
            bool verbose,
            TSettings settings,
            CSharpKernel cSharpKernel)
        {
            Dictionary<string, string> textById = new Dictionary<string, string>();

            IDomType rootType = await _source.ProvideTypeAsync(sourceOptions, async args =>
            {
                ConverterContext<TSettings> context = new ConverterContext<TSettings>(variableName, settings, () => Task.FromResult(args.Text));
                IDomType inferedType = await converter.ProvideTypeAsync(context, converterParameters);

                textById[args.Id] = args.Text;
                return inferedType;
            });

            Func<DeserializeArguments, object?> deserializeChild = args =>
            {
                string text = textById[args.Id];
                IConverterDeserializerAdapter<TSettings> deserializerAdapter = converter;
                return deserializerAdapter.Deserialize(args.Type, text, settings);
            };


            SchemaSerialization schema = converter.DomCompiler.Compile(rootType);
            string type = converter.DomCompiler.Serializer.PrettyPrint(schema.Root);

            string sourceVariable = await cSharpKernel.CreatePrivateVariableAsync(_source, typeof(IIntermediateSource<TSourceOptions>));
            string sourceOptionsVariable = await cSharpKernel.CreatePrivateVariableAsync(sourceOptions, typeof(TSourceOptions));
            string deserializeChildVariable = await cSharpKernel.CreatePrivateVariableAsync(deserializeChild, typeof(Func<DeserializeArguments, object?>));

            await cSharpKernel.SubmitCodeAsync($@"{schema.Code}

{type} {variableName} = await {sourceVariable}.GetAsync<{type}>({sourceOptionsVariable}, {deserializeChildVariable});", verbose);
        }
    }


}
