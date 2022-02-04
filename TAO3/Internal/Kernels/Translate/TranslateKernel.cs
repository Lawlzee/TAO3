using Microsoft.CodeAnalysis.Tags;
using Microsoft.CodeAnalysis.Text;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.Events;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;
using TAO3.NaturalLanguage;
using TAO3.Translation;
using LinePosition = Microsoft.DotNet.Interactive.LinePosition;
using LinePositionSpan = Microsoft.DotNet.Interactive.LinePositionSpan;

namespace TAO3.Internal.Kernels.Translate
{
    internal class TranslateKernel :
        Kernel,
        IKernelCommandHandler<SubmitCode>,
        IKernelCommandHandler<RequestCompletions>,
        IKernelCommandHandler<RequestDiagnostics>
    {
        private readonly ITranslationService _translationService;
        private readonly ChooseTranslateKernelDirective _chooseKernelDirective;

        public TranslateKernel(ITranslationService translationService) : base("translate")
        {
            _translationService = translationService;
            _chooseKernelDirective = new(this);
        }

        public override ChooseKernelDirective ChooseKernelDirective => _chooseKernelDirective;

        public async Task HandleAsync(SubmitCode command, KernelInvocationContext context)
        {
            ParseResult parseResult = command.GetKernelNameDirectiveNode().GetDirectiveParseResult();
            TranslateOptions options = TranslateOptions.Create(parseResult, _chooseKernelDirective);
            string? result = await _translationService.TranslateAsync(options.Source, options.Target, command.Code);
            (result ?? "null").Display();
        }

        public Task HandleAsync(RequestCompletions command, KernelInvocationContext context)
        {
            ParseResult parseResult = command.GetKernelNameDirectiveNode().GetDirectiveParseResult();
            TranslateOptions options = TranslateOptions.Create(parseResult, _chooseKernelDirective);

            ILanguageDictionary? dictionary = _translationService.GetDictionary(options.Source);
            if (dictionary == null)
            {
                context.Publish(new CompletionsProduced(Array.Empty<CompletionItem>(), command));
                return Task.CompletedTask;
            }

            string line = SourceText.From(command.Code).Lines[command.LinePosition.Line].ToString();

            if (command.LinePosition.Character == 0
                || IsDelimiter(line[command.LinePosition.Character - 1]))
            {
                List<CompletionItem> completionList = dictionary.GetWords("")
                    .Distinct()
                    .Take(1000)
                    .Select(word => new CompletionItem(
                        word,
                        kind: WellKnownTags.Local,
                        "",
                        word,
                        word))
                    .ToList();

                context.Publish(new CompletionsProduced(completionList, command));
                return Task.CompletedTask;
            }

            string token = GetToken(command.LinePosition, line);

            List<CompletionItem> completion = dictionary.GetSimilarWords(token)
                .Select(x => x.Word)
                .Concat(dictionary.GetWords(prefix: token))
                .Distinct()
                .Take(1000)
                .Select(word => new CompletionItem(
                    word,
                    kind: WellKnownTags.Local,
                    token,
                    word,
                    word,
                    null))
                .ToList();

            context.Publish(new CompletionsProduced(completion, command));
            return Task.CompletedTask;
        }

        private string GetToken(LinePosition linePosition, string line)
        {
            List<int> spacePositions = line
                .Select((c, i) => IsDelimiter(c) ? i : -1)
                .Where(x => x != -1)
                .ToList();

            int startIndex = spacePositions
                .Where(x => x < linePosition.Character)
                .DefaultIfEmpty(-1)
                .Max();

            int endIndex = spacePositions
                .Where(x => x > linePosition.Character)
                .DefaultIfEmpty(line.Length)
                .Max();

            return line.Substring(startIndex + 1, endIndex - startIndex - 1);
        }

        public Task HandleAsync(RequestDiagnostics command, KernelInvocationContext context)
        {
            ParseResult parseResult = command.GetKernelNameDirectiveNode().GetDirectiveParseResult();
            TranslateOptions options = TranslateOptions.Create(parseResult, _chooseKernelDirective);

            ILanguageDictionary? dictionary = _translationService.GetDictionary(options.Source);
            if (dictionary == null)
            {
                context.Publish(new DiagnosticsProduced(Array.Empty<Diagnostic>(), command));
                return Task.CompletedTask;
            }

            List<Token> tokens = GetTokens(command.Code).ToList();

            List<Diagnostic> diagnostics = tokens
                .Where(token => !dictionary.ContainsWord(token.Text))
                .Select(token => new Diagnostic(
                    token.Span,
                    Microsoft.CodeAnalysis.DiagnosticSeverity.Warning,
                    code: $"TAO3{options.Source}",
                    message: $"Unknown word '{token.Text}'"))
                .ToList();

            context.Publish(new DiagnosticsProduced(diagnostics, command));
            return Task.CompletedTask;
        }

        private record Token(string Text, LinePositionSpan Span);

        private IEnumerable<Token> GetTokens(string code)
        {
            TextLineCollection lines = SourceText.From(code).Lines;
            foreach (var line in lines)
            {
                string lineText = line.ToString();
                int wordStartIndex = 0;
                for (int i = 0; i < lineText.Length; i++)
                {
                    char c = lineText[i];
                    if (IsDelimiter(c))
                    {
                        if (wordStartIndex != i)
                        {
                            yield return new Token(
                                lineText.Substring(wordStartIndex, i - wordStartIndex),
                                new LinePositionSpan(
                                    new LinePosition(
                                        line: line.LineNumber,
                                        character: wordStartIndex),
                                    new LinePosition(
                                        line: line.LineNumber,
                                        character: i)));

                        }

                        wordStartIndex = i + 1;
                    }
                }

                if (wordStartIndex != lineText.Length)
                {
                    yield return new Token(
                        lineText.Substring(wordStartIndex, lineText.Length - wordStartIndex),
                        new LinePositionSpan(
                            new LinePosition(
                                line: line.LineNumber,
                                character: wordStartIndex),
                            new LinePosition(
                                line: line.LineNumber,
                                character: lineText.Length)));

                }
            }
        }

        private bool IsDelimiter(char character)
        {
            switch (CharUnicodeInfo.GetUnicodeCategory(character))
            {
                case UnicodeCategory.SpaceSeparator:
                case UnicodeCategory.LineSeparator:
                case UnicodeCategory.Control:
                case UnicodeCategory.Format:
                case UnicodeCategory.Surrogate:
                case UnicodeCategory.ConnectorPunctuation:
                case UnicodeCategory.DashPunctuation:
                case UnicodeCategory.OpenPunctuation:
                case UnicodeCategory.ClosePunctuation:
                case UnicodeCategory.InitialQuotePunctuation:
                case UnicodeCategory.FinalQuotePunctuation:
                case UnicodeCategory.OtherPunctuation:
                case UnicodeCategory.MathSymbol:
                case UnicodeCategory.CurrencySymbol:
                case UnicodeCategory.ModifierSymbol:
                case UnicodeCategory.OtherSymbol:
                case UnicodeCategory.OtherNotAssigned:
                    return true;
                default:
                    return false;
            }
        }
    }
}
