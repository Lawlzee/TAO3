using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Commands;
using Microsoft.DotNet.Interactive.CSharp;
using Microsoft.DotNet.Interactive.Events;
using Microsoft.DotNet.Interactive.Formatting;
using RazorLight;
using RazorLight.Generation;
using RazorLight.Razor;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TAO3.Internal.Extensions;
using InteractiveDiagnostic = Microsoft.DotNet.Interactive.Diagnostic;

namespace TAO3.Internal.Kernels.Razor
{
    internal class RazorKernel
        : Kernel,
        IKernelCommandHandler<RequestCompletions>,
        IKernelCommandHandler<RequestDiagnostics>,
        IKernelCommandHandler<RequestHoverText>,
        IKernelCommandHandler<RequestSignatureHelp>,
        IKernelCommandHandler<SubmitCode>
    {
        private static readonly MethodInfo _hasReturnValueMethod = typeof(Script)
            .GetMethod("HasReturnValue", BindingFlags.Instance | BindingFlags.NonPublic)!;

        private readonly RazorLightEngine _engine;

        public RazorKernel(RazorLightEngine engine) : base("razor")
        {
            _engine = engine;
        }

        public async Task HandleAsync(RequestCompletions command, KernelInvocationContext context)
        {
            await DoHandleAsync(command, context, failContextIfGenerationFails: true, async (args, linePosition) =>
            {
                await args.CSharpKernel.SendAsync(new RequestCompletions(args.GeneratedCode, linePosition), context.CancellationToken);
            });
        }

        //Same implementation as the C# kernel, but diagnostic position takes into acount the #line directive
        public async Task HandleAsync(RequestDiagnostics command, KernelInvocationContext context)
        {
            await DoHandleAsync(command.Code, command, context, failContextIfGenerationFails: false, async args =>
            {
                await args.CSharpKernel.EnsureWorkspaceIsInitializedAsync(context);

                Document document = args.CSharpKernel.ForkDocumentForLanguageServices(args.GeneratedCode);
                SemanticModel semanticModel = (await document.GetSemanticModelAsync(context.CancellationToken))!;
                ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> diagnostics = semanticModel.GetDiagnostics(cancellationToken: context.CancellationToken);
                context.Publish(GetDiagnosticsProduced(command, diagnostics));
            });

            DiagnosticsProduced GetDiagnosticsProduced(
                KernelCommand command,
                ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> diagnostics)
            {
                var kernelDiagnostics = diagnostics.Select(FromCodeAnalysisDiagnostic).ToImmutableArray();
                var formattedDiagnostics =
                    diagnostics
                        .Select(d => d.ToString())
                        .Select(text => new FormattedValue(PlainTextFormatter.MimeType, text))
                        .ToImmutableArray();

                return new DiagnosticsProduced(kernelDiagnostics, command, formattedDiagnostics);
            }
        }

        public async Task HandleAsync(RequestHoverText command, KernelInvocationContext context)
        {
            await DoHandleAsync(command, context, failContextIfGenerationFails: true, async (args, linePosition) =>
            {
                await args.CSharpKernel.SendAsync(new RequestHoverText(args.GeneratedCode, linePosition), context.CancellationToken);
            });
        }

        public async Task HandleAsync(RequestSignatureHelp command, KernelInvocationContext context)
        {
            await DoHandleAsync(command, context, failContextIfGenerationFails: true, async (args, linePosition) =>
            {
                await args.CSharpKernel.SendAsync(new RequestSignatureHelp(args.GeneratedCode, linePosition), context.CancellationToken);
            });
        }

        public async Task HandleAsync(SubmitCode command, KernelInvocationContext context)
        {
            await DoHandleAsync(command.Code, command, context, failContextIfGenerationFails: true, async args =>
            {
                ParseResult parseResult = command.GetKernelNameDirectiveNode().GetDirectiveParseResult();
                RazorOptions options = RazorOptions.Create(parseResult);

                if (options.Verbose)
                {
                    args.GeneratedCode.Display();
                }

                await HandleAsync(args.CSharpKernel, command, args.GeneratedCode, context);

                string variableName = options.Name ?? "__internal_razorResult";
                await args.CSharpKernel.SetVariableAsync("__razorEngine", _engine);
                await args.CSharpKernel.SubmitCodeAsync($@"string {variableName} = await __razorEngine.RenderTemplateAsync(new GeneratedTemplate((Microsoft.DotNet.Interactive.CSharp.CSharpKernel)Microsoft.DotNet.Interactive.KernelExtensions.FindKernel(Microsoft.DotNet.Interactive.Kernel.Root, ""csharp"")), new System.Dynamic.ExpandoObject());");

                if (!options.Suppress)
                {
                    args.CSharpKernel.TryGetValue(variableName, out string razorResult);

                    object diplayRazorResult = options.MimeType == "text/html"
                        ? new HtmlString(razorResult)
                        : razorResult;

                    context.Publish(new ReturnValueProduced(razorResult, command, FormattedValue.FromObject(diplayRazorResult, options.MimeType)));
                }
            });

            //Same implementation as CSharpKernel.HandleAsync(SubmitCode submitCode, KernelInvocationContext context), but diagnostic position takes into acount the #line directive
            async Task HandleAsync(
                CSharpKernel kernel, 
                SubmitCode submitCode,
                string code,
                KernelInvocationContext context)
            {
                var codeSubmissionReceived = new CodeSubmissionReceived(submitCode);

                context.Publish(codeSubmissionReceived);

                var isComplete = await kernel.IsCompleteSubmissionAsync(code);

                if (isComplete)
                {
                    context.Publish(new CompleteCodeSubmissionReceived(submitCode));
                }
                else
                {
                    context.Publish(new IncompleteCodeSubmissionReceived(submitCode));
                }

                if (submitCode.SubmissionType == SubmissionType.Diagnose)
                {
                    return;
                }

                Exception? exception = null;
                string? message = null;

                if (!context.CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await kernel.RunAsync(
                            code,
                            context.CancellationToken,
                            e =>
                            {
                                exception = e;
                                return true;
                            });
                    }
                    catch (CompilationErrorException cpe)
                    {
                        exception = new CodeSubmissionCompilationErrorException(cpe);
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                }

                if (!context.CancellationToken.IsCancellationRequested)
                {
                    ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> diagnostics;

                    // Check for a compilation failure
                    if (exception is CodeSubmissionCompilationErrorException { InnerException: CompilationErrorException innerCompilationException })
                    {
                        diagnostics = innerCompilationException.Diagnostics;
                        // In the case of an error the diagnostics get attached to both the
                        // DiagnosticsProduced and CommandFailed events.
                        message =
                            string.Join(Environment.NewLine,
                                        innerCompilationException.Diagnostics.Select(d => d.ToString()));
                    }
                    else
                    {
                        diagnostics = kernel.ScriptState?.Script.GetCompilation().GetDiagnostics() ?? ImmutableArray<Microsoft.CodeAnalysis.Diagnostic>.Empty;
                    }

                    // Publish the compilation diagnostics. This doesn't include the exception.
                    var kernelDiagnostics = diagnostics.Select(FromCodeAnalysisDiagnostic).ToImmutableArray();

                    if (kernelDiagnostics.Length > 0)
                    {
                        var formattedDiagnostics =
                            diagnostics
                                .Select(d => d.ToString())
                                .Select(text => new FormattedValue(PlainTextFormatter.MimeType, text))
                                .ToImmutableArray();

                        context.Publish(new DiagnosticsProduced(kernelDiagnostics, submitCode, formattedDiagnostics));
                    }

                    // Report the compilation failure or exception
                    if (exception is not null)
                    {
                        context.Fail(submitCode, exception, message);
                    }
                    else
                    {
                        if (kernel.ScriptState is not null && HasReturnValue())
                        {
                            var formattedValues = FormattedValue.FromObject(kernel.ScriptState.ReturnValue);
                            context.Publish(
                                new ReturnValueProduced(
                                    kernel.ScriptState.ReturnValue,
                                    submitCode,
                                    formattedValues));
                        }
                    }
                }
                else
                {
                    context.Fail(submitCode, null, "Command cancelled");
                }

                bool HasReturnValue()
                {
                    return kernel.ScriptState is not null &&
                        (bool)_hasReturnValueMethod.Invoke(kernel.ScriptState.Script, null)!;
                }
            }
        }

        private InteractiveDiagnostic FromCodeAnalysisDiagnostic(Microsoft.CodeAnalysis.Diagnostic diagnostic)
        {
            //FileLinePositionSpan lineSpan = diagnostic.Location.GetLineSpan();
            FileLinePositionSpan lineSpan = diagnostic.Location.SourceTree!.GetMappedLineSpan(diagnostic.Location.SourceSpan);

            return new InteractiveDiagnostic(
                new LinePositionSpan(
                    LinePosition.FromCodeAnalysisLinePosition(lineSpan.StartLinePosition),
                    LinePosition.FromCodeAnalysisLinePosition(lineSpan.EndLinePosition)),
                diagnostic.Severity,
                diagnostic.Id,
                diagnostic.GetMessage());

        }

        private record HandleContext(
            string GeneratedCode, 
            CSharpKernel CSharpKernel, 
            Dictionary<string, Type> Variables);

        private async Task DoHandleAsync(
            LanguageServiceCommand command, 
            KernelInvocationContext context,
            bool failContextIfGenerationFails,
            Func<HandleContext, LinePosition, Task> handle)
        {
            await DoHandleAsync(command.Code, command, context, failContextIfGenerationFails, async args =>
            {
                LinePosition position = GetLinePositionInGeneratedCode(args.GeneratedCode, command.LinePosition);
                await handle(args, position);
            });
        }

        private async Task DoHandleAsync(
            string code, 
            KernelCommand command, 
            KernelInvocationContext context,
            bool failContextIfGenerationFails,
            Func<HandleContext, Task> handle)
        {
            CSharpKernel cSharpKernel = context.GetCSharpKernel();
            Dictionary<string, Type> variables = GetVariables(cSharpKernel);

            string? generatedCode = await GetGeneratedCodeAsync(code, command, context, failContextIfGenerationFails, variables);
            if (generatedCode is null)
            {
                return;
            }

            await handle(new HandleContext(generatedCode, cSharpKernel, variables));

            Dictionary<string, Type> GetVariables(CSharpKernel cSharpKernel)
            {
                return cSharpKernel.ScriptState
                    .Variables
                    .GroupBy(x => x.Name)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Last().Type);
            }
        }

        private async Task<string?> GetGeneratedCodeAsync(
            string code, 
            KernelCommand command, 
            KernelInvocationContext context,
            bool failContextIfGenerationFails,
            Dictionary<string, Type> variables)
        {
            RazorSourceGenerator sourceGenerator = _engine.Handler.Compiler.GetSourceGenerator();

            try
            {
                IGeneratedRazorTemplate template = await sourceGenerator.GenerateCodeAsync(new TextSourceRazorProjectItem(ComputeKey(code), code));
                if (failContextIfGenerationFails)
                {
                    context.Publish(new DiagnosticsProduced(Array.Empty<InteractiveDiagnostic>(), command));
                }
                return RazorGeneratedCodeCleaner.Clean(template.GeneratedCode, variables);
            }
            catch (TemplateGenerationException ex)
            {
                List<InteractiveDiagnostic> diagnostics = ex.Diagnostics
                    .Select(ConvertRazorDiagnostic)
                    .ToList();

                context.Publish(new DiagnosticsProduced(diagnostics, command));

                if (failContextIfGenerationFails)
                {
                    context.Fail(command, ex);
                }

                return null;
            }

            string ComputeKey(string str)
            {
                return SHA256.Create()
                    .ComputeHash(Encoding.UTF8.GetBytes(str))
                    .Aggregate(
                        new StringBuilder(),
                        (sb, b) => sb.Append(b.ToString("X2")))
                    .ToString();
            }

            InteractiveDiagnostic ConvertRazorDiagnostic(RazorDiagnostic diagnostic)
            {
                var position = new LinePositionSpan(
                    new LinePosition(
                        diagnostic.Span.LineIndex,
                        diagnostic.Span.CharacterIndex),
                    new LinePosition(
                        diagnostic.Span.LineIndex,
                        diagnostic.Span.LineIndex + diagnostic.Span.Length));

                var severity = diagnostic.Severity == RazorDiagnosticSeverity.Error
                    ? DiagnosticSeverity.Error
                    : DiagnosticSeverity.Warning;

                return new InteractiveDiagnostic(
                    position,
                    severity,
                    code,
                    diagnostic.GetMessage());
            }
        }

        //todo: handle lines with multiple #line directive with the same line number
        //todo: offset the column index of the position when necessary
        private LinePosition GetLinePositionInGeneratedCode(string generatedCode, LinePosition position)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(generatedCode);

            var candidats = tree.GetRoot()
                .DescendantNodes(descendIntoTrivia: true)
                .OfType<LineDirectiveTriviaSyntax>()
                .Select(x => new
                {
                    OriginalLineText = x.Line.ValueText,
                    GeneratedLine = tree.GetLineSpan(x.Span).StartLinePosition.Line + 1
                })
                .Where(x => int.TryParse(x.OriginalLineText, out _))
                .Select(x => new
                {
                    OriginalLine = int.Parse(x.OriginalLineText) - 1,
                    x.GeneratedLine
                })
                .ToList();

            int line = candidats
                .Where(x => x.OriginalLine <= position.Line)
                .Select(x => x.GeneratedLine + (position.Line - x.OriginalLine))
                .Last();

            return new LinePosition(line, position.Character);
        }

        protected override ChooseKernelDirective CreateChooseKernelDirective()
        {
            Option<string> mimeTypeOption = new Option<string>(new[] { "--mimeType", "-m" }, "Mime type used to display the resulting document");

            mimeTypeOption.AddSuggestions(
                "text/html",
                "text/plain",
                "application/json",
                "text/markdown",
                "text/x-csharp",
                "text/x-javascript",
                "text/x-fsharp",
                "text/x-sql",
                "text/x-powershell");

            return new ChooseKernelDirective(this)
            {
                mimeTypeOption,
                new Option<string>(new[] { "--name", "-n" }, "Name of the variable that will containt the resulting document"),
                new Option<bool>(new[] { "--suppress", "-s" }, "Suppress the displaying of the resulting document"),
                new Option<bool>(new[] { "--verbose", "-v" }, "Print debugging information")
            };
        }

        private class RazorOptions
        {
            private static readonly ModelBinder<RazorOptions> _modelBinder = new();

            public string? MimeType { get; set; }
            public string? Name { get; set; }
            public bool Suppress { get; set; }
            public bool Verbose { get; set; }

            public static RazorOptions Create(ParseResult parseResult) => (_modelBinder.CreateInstance(new BindingContext(parseResult)) as RazorOptions)!;
        }
    }
}
