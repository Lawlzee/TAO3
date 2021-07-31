using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;
using TAO3.Translation;

namespace TAO3.Internal.Kernels
{
    internal class TranslateKernel : Kernel, IKernelCommandHandler<SubmitCode>
    {
        private readonly ITranslationService _translationService;

        public TranslateKernel(ITranslationService translationService) : base("translate")
        {
            _translationService = translationService;
        }

        public async Task HandleAsync(SubmitCode command, KernelInvocationContext context)
        {
            ParseResult parseResult = command.GetKernelNameDirectiveNode().GetDirectiveParseResult();
            TranslateOptions options = TranslateOptions.Create(parseResult);
            string? result = await _translationService.TranslateAsync(options.Source, options.Target, command.Code);
            (result ?? "null").Display();
        }

        protected override ChooseKernelDirective CreateChooseKernelDirective()
        {
            return new ChooseKernelDirective(this, "Translate text from source language to target language")
            {
                new Argument<Language>("source"),
                new Argument<Language>("target")
            };
        }

        private class TranslateOptions
        {
            private static readonly ModelBinder<TranslateOptions> _modelBinder = new();

            public Language Source { get; set; }
            public Language Target { get; set; }

            public static TranslateOptions Create(ParseResult parseResult) => (_modelBinder.CreateInstance(new BindingContext(parseResult)) as TranslateOptions)!;
        }
    }
}
