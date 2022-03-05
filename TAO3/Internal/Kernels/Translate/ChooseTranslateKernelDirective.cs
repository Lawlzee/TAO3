using Microsoft.DotNet.Interactive;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using TAO3.Translation;

namespace TAO3.Internal.Kernels.Translate;

internal class ChooseTranslateKernelDirective : ChooseKernelDirective
{
    public Argument<Language> Source { get; }
    public Argument<Language> Target { get; }

    public ChooseTranslateKernelDirective(Kernel kernel)
        : base(kernel, "Translate text from source language to target language")
    {
        Source = new Argument<Language>("source");
        Target = new Argument<Language>("target");

        Add(Source);
        Add(Target);
    }
}

internal record TranslateOptions(
    Language Source,
    Language Target)
{
    public static TranslateOptions Create(ParseResult parseResult, ChooseTranslateKernelDirective directive)
    {
        return new TranslateOptions(
            parseResult.GetValueForArgument(directive.Source),
            parseResult.GetValueForArgument(directive.Target));
    }
}
